using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Repositories;

namespace Opplat.Domain.Inventory.Services;

public interface ITFAService
{
    Task<IEnumerable<TangibleFixedAsset>> List();
    Task<TangibleFixedAsset> Find(Guid id);
    Task<bool> WriteOff(Guid id, string user);
    Task<bool> DepreciateAll(int month, int year);
    Task<bool> CreateTFAs(string description, DateTime date, decimal value, int quatity, string user);
    Task<bool> Edit(Guid id, string description, DateTime date, decimal value, string user);
}

public class TFAService : ITFAService
{
    private ITFARepository _tfaRepo;

    public TFAService(ITFARepository tfaRepo)
    {
        _tfaRepo = tfaRepo;
    }

    public async Task<IEnumerable<TangibleFixedAsset>> List()
    {
        return await _tfaRepo.List();
    }

    public async Task<TangibleFixedAsset> Find(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> WriteOff(Guid id, string user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DepreciateAll(int month, int year)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CreateTFAs(string description, DateTime date, decimal value, int quatity, string user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Edit(Guid id, string description, DateTime date, decimal value, string user)
    {
        throw new NotImplementedException();
    }

}
