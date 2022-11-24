using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Inventory.Repositories;
using Opplat.Domain.Inventory.Entities;
using Opplat.Shared.Repositories;
using Opplat.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Opplat.Infrastructure.Inventory.Repositories;

public class ProductMovementRepository : BaseRepository<ProductMovement>, IMovementsRepository
{
    public ProductMovementRepository(DbContext db, ILogger<IMovementsRepository> logger) : base(db, logger)
    {
    }

    public async Task<RepositoryResponse> CheckIfPosible(ProductMovement entity, int movementFactor, string unit)
    {
        var stock = await _db.Set<ProductInventory>()
            .SingleOrDefaultAsync(p => p.ProductId == entity.ProductId && p.StorageId == entity.StorageId);
        stock ??= new ProductInventory()
        {
            StorageId = entity.StorageId,
            ProductId = entity.ProductId,
            Quantity = 0,
            UpdatedOn = DateTime.UtcNow,
        };
        if (stock.Quantity + (entity.Quantity * movementFactor) < 0)
        {
            var remainAmount = Math.Abs(stock.Quantity + (entity.Quantity * movementFactor));
            var message = $"You need {remainAmount} {unit} to make the movement";
            return new RepositoryResponse
            {
                IsOk = false,
                Message = message
            };
        }
        return new RepositoryResponse
        {
            IsOk = true
        };
    }

    public async Task<RepositoryResponse> Create(ProductMovement entity, int movementFactor, string unit)
    {
        string message = "";
        try
        {
            using var transaction = _db.Database.BeginTransaction();
            var stock = await _db.Set<ProductInventory>()
                .SingleOrDefaultAsync(p => p.ProductId == entity.ProductId && p.StorageId == entity.StorageId);
            if (stock == null)
            {
                stock = new ProductInventory()
                {
                    StorageId = entity.StorageId,
                    ProductId = entity.ProductId,
                    Quantity = 0,
                    UpdatedOn = DateTime.UtcNow,
                };
                var response = _db.Set<ProductInventory>().Add(stock);
                await _db.SaveChangesAsync();
            }
            _logger.LogDebug("In stock: {}, To move: {}, Factor: {}", stock.Quantity, entity.Quantity, movementFactor);
            if (stock.Quantity + (entity.Quantity * movementFactor) < 0)
            {
                var remainAmount = Math.Abs(stock.Quantity + (entity.Quantity * movementFactor));
                message = $"You need {remainAmount} {unit} to make the movement";
                return new RepositoryResponse
                {
                    IsOk = false,
                    Message = message
                };
            }
            stock.Quantity += entity.Quantity * movementFactor;
            _db.Update(stock);
            _db.Add(entity);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return new RepositoryResponse
            {
                IsOk = true,
            };
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error creating product movement.");
            return new RepositoryResponse
            {
                IsOk = false,
                Message = message + "\n\r" + ex.Message
            };
        }
    }

    public async Task<IEnumerable<ProductMovement>> GetByStorage(Guid id)
    {
        var data = await _db.Set<ProductMovement>()
            .Include(m => m.Storage)
            .Include(m => m.Product)
            .Where(m => m.StorageId == id)
            .ToListAsync();
        return data;
    }

    public override async Task<IEnumerable<ProductMovement>> List(int page = 0, int pageSize = 0)
    {
        var data = await _db.Set<ProductMovement>()
            .Include(m => m.Storage)
            .Include(m => m.Product)
            .ToListAsync();
        return data;
    }
}
