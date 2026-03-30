using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Opplat.Application.Abstractions.Repositories.Inventory;
using Opplat.Domain.Entities.Inventory;
using StorageEntity = Opplat.Domain.Entities.Inventory.Storage;

namespace Opplat.Application.Services.Inventory;

public interface IStorageService: IService<StorageEntity, string>
{
    Task<IEnumerable<ProductInventory>> Inventory();
    Task<IEnumerable<ReleaseVoucher>> ReleaseVouchers(DateTime date);
    Task<IEnumerable<ReleaseVoucher>> ReleaseVouchers(DateTime startDate, DateTime endDate);
    Task<bool> ReleaseFromStorage(ReleaseVoucher voucher, string user);
    // TODO: Cambiar la clase que se pasa para la merma.
    Task<bool> ReleaseByWaste(ReleaseVoucher voucher, string user);
}

public class StorageService : BaseService<StorageEntity, string>, IStorageService
{
    public StorageService(IStorageRepository repo, ILogger<StorageEntity> logger) : base(repo, logger)
    {
    }

    public async Task<IEnumerable<ProductInventory>> Inventory(){
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<ReleaseVoucher>> ReleaseVouchers(DateTime date){
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<ReleaseVoucher>> ReleaseVouchers(DateTime startDate, DateTime endDate){
        throw new NotImplementedException();
    }
    public async Task<bool> ReleaseFromStorage(ReleaseVoucher voucher, string user){
        throw new NotImplementedException();
    }
    public async Task<bool> ReleaseByWaste(ReleaseVoucher voucher, string user){
        throw new NotImplementedException();
    }
}
