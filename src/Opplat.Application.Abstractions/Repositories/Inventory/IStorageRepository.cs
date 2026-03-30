
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Application.Abstractions.Repositories.Inventory;

public interface IStorageRepository : IRepository<Storage>
{
    Task<RepositoryResponse> AddMovement(ProductMovement movememnt);
    Task<Storage> GetPrimaryStorage();

    Task<IEnumerable<ReleaseVoucher>> GetValesDeSalida(DateTime fechaInicial, DateTime fechaFinal);

    Task<IEnumerable<VoucherDetail>> GetVoucherDetails(Guid voucherId);

    Task<IEnumerable<ProductMovement>> GetWastes(Guid storageId);
}
