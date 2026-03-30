using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Repositories.Inventory;
using Opplat.Application.Features.Inventory.Common;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Features.Inventory.ProductGroups;

public sealed record GetProductGroupQuery(int Id) : IQuery<ProductGroup?>;

public sealed class GetProductGroupQueryHandler(IProductGroupRepository repository)
    : IQueryHandler<GetProductGroupQuery, ProductGroup?>
{
    public Task<ProductGroup?> Handle(GetProductGroupQuery request, CancellationToken cancellationToken)
        => repository.GetWithClassification(request.Id);
}

public sealed record ListProductGroupsQuery() : IQuery<IReadOnlyList<ProductGroup>>;

public sealed class ListProductGroupsQueryHandler(IProductGroupRepository repository)
    : IQueryHandler<ListProductGroupsQuery, IReadOnlyList<ProductGroup>>
{
    public async Task<IReadOnlyList<ProductGroup>> Handle(ListProductGroupsQuery request, CancellationToken cancellationToken)
        => (await repository.ListWithClassification()).ToList();
}

public sealed record CreateProductGroupCommand(ProductGroup Group, string? User) : ICommand<InventoryCommandResult>;

public sealed class CreateProductGroupCommandHandler(IProductGroupRepository repository)
    : ICommandHandler<CreateProductGroupCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(CreateProductGroupCommand request, CancellationToken cancellationToken)
    {
        request.Group.Classification = null!;
        var response = await repository.Create(request.Group);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record UpdateProductGroupCommand(ProductGroup Group, string? User) : ICommand<InventoryCommandResult>;

public sealed class UpdateProductGroupCommandHandler(IProductGroupRepository repository)
    : ICommandHandler<UpdateProductGroupCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(UpdateProductGroupCommand request, CancellationToken cancellationToken)
    {
        request.Group.Classification = null!;
        var response = await repository.Update(request.Group);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record DeleteProductGroupCommand(int Id, string? User) : ICommand<InventoryCommandResult>;

public sealed class DeleteProductGroupCommandHandler(IProductGroupRepository repository)
    : ICommandHandler<DeleteProductGroupCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(DeleteProductGroupCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Delete(request.Id);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

