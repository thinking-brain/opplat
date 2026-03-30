using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Repositories.Inventory;
using Opplat.Application.Features.Inventory.Common;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Features.Inventory.ProductClassifications;

public sealed record GetProductClassificationQuery(int Id) : IQuery<ProductClassification?>;

public sealed class GetProductClassificationQueryHandler(IProductClassificationRepository repository)
    : IQueryHandler<GetProductClassificationQuery, ProductClassification?>
{
    public Task<ProductClassification?> Handle(GetProductClassificationQuery request, CancellationToken cancellationToken)
        => repository.Find(request.Id);
}

public sealed record ListProductClassificationsQuery() : IQuery<IReadOnlyList<ProductClassification>>;

public sealed class ListProductClassificationsQueryHandler(IProductClassificationRepository repository)
    : IQueryHandler<ListProductClassificationsQuery, IReadOnlyList<ProductClassification>>
{
    public async Task<IReadOnlyList<ProductClassification>> Handle(ListProductClassificationsQuery request, CancellationToken cancellationToken)
        => (await repository.List()).ToList();
}

public sealed record CreateProductClassificationCommand(ProductClassification Classification, string? User) : ICommand<InventoryCommandResult>;

public sealed class CreateProductClassificationCommandHandler(IProductClassificationRepository repository)
    : ICommandHandler<CreateProductClassificationCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(CreateProductClassificationCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Create(request.Classification);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record UpdateProductClassificationCommand(ProductClassification Classification, string? User) : ICommand<InventoryCommandResult>;

public sealed class UpdateProductClassificationCommandHandler(IProductClassificationRepository repository)
    : ICommandHandler<UpdateProductClassificationCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(UpdateProductClassificationCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Update(request.Classification);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record DeleteProductClassificationCommand(int Id, string? User) : ICommand<InventoryCommandResult>;

public sealed class DeleteProductClassificationCommandHandler(IProductClassificationRepository repository)
    : ICommandHandler<DeleteProductClassificationCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(DeleteProductClassificationCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Delete(request.Id);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

