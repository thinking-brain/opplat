using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories;
using Opplat.Application.Abstractions.Repositories.Inventory;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Infrastructure.Persistance.Repositories.Inventory;

public class StorageRepository(DbContext db, ILogger<IStorageRepository> logger) : BaseRepository<Storage>(db, logger), IStorageRepository
{
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
