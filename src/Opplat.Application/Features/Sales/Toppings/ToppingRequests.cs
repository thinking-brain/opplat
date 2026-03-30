using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Application.Features.Sales.Common;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Features.Sales.Toppings;

public sealed record GetToppingQuery(string Id) : IQuery<Topping?>;

public sealed class GetToppingQueryHandler(IToppingRepository repository)
    : IQueryHandler<GetToppingQuery, Topping?>
{
    public Task<Topping?> Handle(GetToppingQuery request, CancellationToken cancellationToken)
        => repository.Find(Guid.Parse(request.Id));
}

public sealed record ListToppingsQuery() : IQuery<IReadOnlyList<Topping>>;

public sealed class ListToppingsQueryHandler(IToppingRepository repository)
    : IQueryHandler<ListToppingsQuery, IReadOnlyList<Topping>>
{
    public async Task<IReadOnlyList<Topping>> Handle(ListToppingsQuery request, CancellationToken cancellationToken)
        => (await repository.List()).ToList();
}

public sealed record CreateToppingCommand(Topping Topping, string? User) : ICommand<SalesCommandResult>;

public sealed class CreateToppingCommandHandler(IToppingRepository repository)
    : ICommandHandler<CreateToppingCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(CreateToppingCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Create(request.Topping);
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record UpdateToppingCommand(Topping Topping, string? User) : ICommand<SalesCommandResult>;

public sealed class UpdateToppingCommandHandler(IToppingRepository repository)
    : ICommandHandler<UpdateToppingCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(UpdateToppingCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Update(request.Topping);
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record DeleteToppingCommand(string Id, string? User) : ICommand<SalesCommandResult>;

public sealed class DeleteToppingCommandHandler(IToppingRepository repository)
    : ICommandHandler<DeleteToppingCommand, SalesCommandResult>
{
    public async Task<SalesCommandResult> Handle(DeleteToppingCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Delete(Guid.Parse(request.Id));
        return SalesCommandResult.From(response.IsOk, response.Message);
    }
}

