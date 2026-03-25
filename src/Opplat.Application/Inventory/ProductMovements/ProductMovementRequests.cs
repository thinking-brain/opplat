using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Inventory.Common;
using Opplat.Modules.Inventory.Domain.Entities;
using Opplat.Modules.Inventory.Domain.Repositories;
using Opplat.Modules.Inventory.Domain.Services;
using Opplat.Shared.Helpers;

namespace Opplat.Application.Inventory.ProductMovements;

public sealed record GetProductMovementsByStorageQuery(string StorageId) : IQuery<IReadOnlyList<ProductMovement>>;

public sealed class GetProductMovementsByStorageQueryHandler(IMovementsRepository repository)
    : IQueryHandler<GetProductMovementsByStorageQuery, IReadOnlyList<ProductMovement>>
{
    public async Task<IReadOnlyList<ProductMovement>> Handle(GetProductMovementsByStorageQuery request, CancellationToken cancellationToken)
        => (await repository.GetByStorage(new Guid(request.StorageId))).ToList();
}

public sealed record ListProductMovementsQuery() : IQuery<IReadOnlyList<ProductMovement>>;

public sealed class ListProductMovementsQueryHandler(IMovementsRepository repository)
    : IQueryHandler<ListProductMovementsQuery, IReadOnlyList<ProductMovement>>
{
    public async Task<IReadOnlyList<ProductMovement>> Handle(ListProductMovementsQuery request, CancellationToken cancellationToken)
        => (await repository.List()).ToList();
}

public sealed record CreateProductMovementCommand(
    DateTime? Date,
    Guid ProductId,
    Guid StorageId,
    decimal Quantity,
    string Unit,
    MovementType Type,
    string? User,
    string Observations) : ICommand<InventoryCommandResult>;

public sealed class CreateProductMovementCommandHandler(
    IMovementsRepository repository,
    IProductRepository productRepository,
    IStorageRepository storageRepository,
    IMovementTypeService movementTypeService)
    : ICommandHandler<CreateProductMovementCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(CreateProductMovementCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.Find(request.ProductId);
        if (product is null)
        {
            return InventoryCommandResult.From(false, "Entity not found.");
        }

        var storage = await storageRepository.Find(request.StorageId);
        if (storage is null)
        {
            return InventoryCommandResult.From(false, "Entity not found.");
        }

        var unit = UnitOfMeasurementHelper.GetByAbbreviation(request.Unit);
        var productUnit = UnitOfMeasurementHelper.GetByAbbreviation(product.Unit);
        var adjustedAmount = request.Quantity * (productUnit.CovertionFactor / unit.CovertionFactor);
        var movement = new ProductMovement
        {
            StorageId = storage.Id,
            ProductId = product.Id,
            Quantity = adjustedAmount,
            Date = request.Date ?? DateTime.UtcNow,
            Type = request.Type,
            Cost = adjustedAmount * product.Cost,
            User = request.User ?? string.Empty,
            Observations = request.Observations
        };

        var response = await repository.Create(
            movement,
            movementTypeService.GetFactor(request.Type),
            unit.Abbreviation);

        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

