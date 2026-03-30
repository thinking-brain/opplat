using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Application.Features.Sales.Common;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Features.Sales.CostTabs;

public sealed record GetCostTabQuery(string Id) : IQuery<CostTab?>;

public sealed class GetCostTabQueryHandler(ICostTabRepository repository)
    : IQueryHandler<GetCostTabQuery, CostTab?>
{
    public Task<CostTab?> Handle(GetCostTabQuery request, CancellationToken cancellationToken)
        => repository.Find(Guid.Parse(request.Id));
}

public sealed record ListCostTabsQuery() : IQuery<IReadOnlyList<CostTab>>;

public sealed class ListCostTabsQueryHandler(ICostTabRepository repository)
    : IQueryHandler<ListCostTabsQuery, IReadOnlyList<CostTab>>
{
    public async Task<IReadOnlyList<CostTab>> Handle(ListCostTabsQuery request, CancellationToken cancellationToken)
        => [.. await repository.List()];
}

public sealed record CreateCostTabCommand(CostTab CostTab, string? User) : ICommand<SalesCommandResult>;

public sealed class CreateCostTabCommandHandler(ICostTabRepository repository)
    : ICommandHandler<CreateCostTabCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(CreateCostTabCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Create(request.CostTab);
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record UpdateCostTabCommand(CostTab CostTab, string? User) : ICommand<SalesCommandResult>;

public sealed class UpdateCostTabCommandHandler(ICostTabRepository repository)
    : ICommandHandler<UpdateCostTabCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(UpdateCostTabCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Update(request.CostTab);
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record DeleteCostTabCommand(string Id, string? User) : ICommand<SalesCommandResult>;

public sealed class DeleteCostTabCommandHandler(ICostTabRepository repository)
    : ICommandHandler<DeleteCostTabCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(DeleteCostTabCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Delete(Guid.Parse(request.Id));
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

