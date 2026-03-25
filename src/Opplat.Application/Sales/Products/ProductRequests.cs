using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Sales.Common;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Repositories;

namespace Opplat.Application.Sales.Products;

public sealed record ListProductsQuery() : IQuery<IReadOnlyList<ProductForSale>>;

public sealed class ListProductsQueryHandler(IProductRepository repository)
    : IQueryHandler<ListProductsQuery, IReadOnlyList<ProductForSale>>
{
    public async Task<IReadOnlyList<ProductForSale>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
        => (await repository.List()).ToList();
}

public sealed record CreateProductCommand(ProductForSale Product, string? User) : ICommand<SalesCommandResult>;

public sealed class CreateProductCommandHandler(IProductRepository repository)
    : ICommandHandler<CreateProductCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Create(request.Product);
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record UpdateProductCommand(ProductForSale Product, string? User) : ICommand<SalesCommandResult>;

public sealed class UpdateProductCommandHandler(IProductRepository repository)
    : ICommandHandler<UpdateProductCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Update(request.Product);
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record DeleteProductCommand(string Id, string? User) : ICommand<SalesCommandResult>;

public sealed class DeleteProductCommandHandler(IProductRepository repository)
    : ICommandHandler<DeleteProductCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Delete(Guid.Parse(request.Id));
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

