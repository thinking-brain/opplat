using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Modules.Inventory.Domain.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Modules.Inventory.Domain.Repositories;

public interface IStorageRepository: IRepository<Storage>
{
    Task<RepositoryResponse> AddMovement(ProductMovement movememnt);
    Task<Storage> GetPrimaryStorage();

    Task<IEnumerable<ReleaseVoucher>> GetValesDeSalida(DateTime fechaInicial, DateTime fechaFinal);

    Task<IEnumerable<VoucherDetail>> GetVoucherDetails(Guid voucherId);

    Task<IEnumerable<ProductMovement>> GetWastes(Guid storageId);
}
