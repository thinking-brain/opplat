using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Sales.Common;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Repositories;

namespace Opplat.Application.Sales.ProductTags;

public sealed record GetProductTagQuery(string Id) : IQuery<ProductTag?>;

public sealed class GetProductTagQueryHandler(IProductTagRepository repository)
    : IQueryHandler<GetProductTagQuery, ProductTag?>
{
    public Task<ProductTag?> Handle(GetProductTagQuery request, CancellationToken cancellationToken)
        => repository.Find(Guid.Parse(request.Id));
}

public sealed record ListProductTagsQuery() : IQuery<IReadOnlyList<ProductTag>>;

public sealed class ListProductTagsQueryHandler(IProductTagRepository repository)
    : IQueryHandler<ListProductTagsQuery, IReadOnlyList<ProductTag>>
{
    public async Task<IReadOnlyList<ProductTag>> Handle(ListProductTagsQuery request, CancellationToken cancellationToken)
        => (await repository.List()).ToList();
}

public sealed record CreateProductTagCommand(ProductTag ProductTag, string? User) : ICommand<SalesCommandResult>;

public sealed class CreateProductTagCommandHandler(IProductTagRepository repository)
    : ICommandHandler<CreateProductTagCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(CreateProductTagCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Create(request.ProductTag);
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record UpdateProductTagCommand(ProductTag ProductTag, string? User) : ICommand<SalesCommandResult>;

public sealed class UpdateProductTagCommandHandler(IProductTagRepository repository)
    : ICommandHandler<UpdateProductTagCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(UpdateProductTagCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Update(request.ProductTag);
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record DeleteProductTagCommand(string Id, string? User) : ICommand<SalesCommandResult>;

public sealed class DeleteProductTagCommandHandler(IProductTagRepository repository)
    : ICommandHandler<DeleteProductTagCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(DeleteProductTagCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Delete(Guid.Parse(request.Id));
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

