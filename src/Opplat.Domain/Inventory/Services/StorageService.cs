using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Domain.Inventory.Services;

public interface IStorageService: IService<Storage, string>
{
    Task<IEnumerable<ProductInventory>> Inventory();
    Task<IEnumerable<ReleaseVoucher>> ReleaseVouchers(DateTime date);
    Task<IEnumerable<ReleaseVoucher>> ReleaseVouchers(DateTime startDate, DateTime endDate);
    Task<bool> ReleaseFromStorage(ReleaseVoucher voucher, string user);
    // TODO: Cambiar la clase que se pasa para la merma.
    Task<bool> ReleaseByWaste(ReleaseVoucher voucher, string user);
}

public class StorageService : BaseService<Storage, string>, IStorageService
{
    public StorageService(IStorageRepository repo, ILogger<Storage> logger) : base(repo, logger)
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
