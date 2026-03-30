using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Repositories.Inventory;
using Opplat.Application.Features.Inventory.Common;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Features.Inventory.Storages;

public sealed record GetStorageQuery(string Id) : IQuery<Storage?>;

public sealed class GetStorageQueryHandler(IStorageRepository repository)
    : IQueryHandler<GetStorageQuery, Storage?>
{
    public Task<Storage?> Handle(GetStorageQuery request, CancellationToken cancellationToken)
        => repository.Find(request.Id);
}

public sealed record ListStoragesQuery() : IQuery<IReadOnlyList<Storage>>;

public sealed class ListStoragesQueryHandler(IStorageRepository repository)
    : IQueryHandler<ListStoragesQuery, IReadOnlyList<Storage>>
{
    public async Task<IReadOnlyList<Storage>> Handle(ListStoragesQuery request, CancellationToken cancellationToken)
        => (await repository.List()).ToList();
}

public sealed record CreateStorageCommand(Storage Storage, string? User) : ICommand<InventoryCommandResult>;

public sealed class CreateStorageCommandHandler(IStorageRepository repository)
    : ICommandHandler<CreateStorageCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(CreateStorageCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Create(request.Storage);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record UpdateStorageCommand(Storage Storage, string? User) : ICommand<InventoryCommandResult>;

public sealed class UpdateStorageCommandHandler(IStorageRepository repository)
    : ICommandHandler<UpdateStorageCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(UpdateStorageCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Update(request.Storage);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

public sealed record DeleteStorageCommand(string Id, string? User) : ICommand<InventoryCommandResult>;

public sealed class DeleteStorageCommandHandler(IStorageRepository repository)
    : ICommandHandler<DeleteStorageCommand, InventoryCommandResult>
{
    public async Task<InventoryCommandResult> Handle(DeleteStorageCommand request, CancellationToken cancellationToken)
    {
        var response = await repository.Delete(request.Id);
        return InventoryCommandResult.From(response.IsOk, response.Message);
    }
}

