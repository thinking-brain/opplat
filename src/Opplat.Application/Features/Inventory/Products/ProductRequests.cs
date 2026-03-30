using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Repositories.Inventory;
using Opplat.Application.Features.Inventory.Common;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Features.Inventory.Products;

public sealed record GetProductQuery(string Id) : IQuery<Product?>;

public sealed class GetProductQueryHandler(IProductRepository repository)
    : IQueryHandler<GetProductQuery, Product?>
{
    public Task<Product?> Handle(GetProductQuery request, CancellationToken cancellationToken)
        => repository.GetWithGroup(request.Id);
}

public sealed record ListProductsQuery() : IQuery<IReadOnlyList<Product>>;

public sealed class ListProductsQueryHandler(IProductRepository repository)
    : IQueryHandler<ListProductsQuery, IReadOnlyList<Product>>
{
    public async Task<IReadOnlyList<Product>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
        => (await repository.ListWithGroup()).ToList();
}

public sealed record CreateProductCommand(Product Product, string? User) : ICommand<InventoryCommandResult>;

public sealed class CreateProductCommandHandler(IProductRepository repository)
    : ICommandHandler<CreateProductCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        request.Product.Group = null;
        var response = await repository.Create(request.Product);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record UpdateProductCommand(Product Product, string? User) : ICommand<InventoryCommandResult>;

public sealed class UpdateProductCommandHandler(IProductRepository repository)
    : ICommandHandler<UpdateProductCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        request.Product.Group = null;
        var response = await repository.Update(request.Product);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record DeleteProductCommand(string Id, string? User) : ICommand<InventoryCommandResult>;

public sealed class DeleteProductCommandHandler(IProductRepository repository)
    : ICommandHandler<DeleteProductCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Delete(request.Id);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

