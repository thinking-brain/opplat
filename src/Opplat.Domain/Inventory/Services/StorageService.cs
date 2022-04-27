using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Repositories;

namespace Opplat.Domain.Inventory.Services;

public interface IStorageService
{
    Task<IEnumerable<ProductInventory>> Inventory();
    Task<IEnumerable<ReleaseVoucher>> ReleaseVouchers(DateTime date);
    Task<IEnumerable<ReleaseVoucher>> ReleaseVouchers(DateTime startDate, DateTime endDate);
    Task<bool> ReleaseFromStorage(ReleaseVoucher voucher, string user);
    // TODO: Cambiar la clase que se pasa para la merma.
    Task<bool> ReleaseByWaste(ReleaseVoucher voucher, string user);
}

public class StorageService : IStorageService
{
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
