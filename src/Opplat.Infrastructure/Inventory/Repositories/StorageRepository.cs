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

public class StorageRepository : BaseRepository<Storage>, IStorageRepository
{
    public StorageRepository(DbContext db, ILogger<IStorageRepository> logger) : base(db, logger)
    {
    }

    public async Task<RepositoryResponse> Delete(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<RepositoryResponse> AddMovement(ProductMovement movememnt)
    {
        throw new NotImplementedException();
    }

    public async Task<Storage> GetPrimaryStorage()
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<ReleaseVoucher>> IStorageRepository.GetValesDeSalida(DateTime fechaInicial, DateTime fechaFinal)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<VoucherDetail>> GetVoucherDetails(Guid voucherId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductMovement>> GetWastes(Guid storageId)
    {
        throw new NotImplementedException();
    }
}
