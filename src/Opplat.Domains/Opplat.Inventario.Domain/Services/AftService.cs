using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Inventario.Domain.Entities;
using Opplat.Inventario.Domain.Repositories;

namespace Opplat.Inventario.Domain.Services;

public interface IAftService
{
    Task<IEnumerable<Aft>> List();
    Task<Aft> Find(Guid id);
    Task<bool> DarBaja(Guid id, string user);
    Task<bool> DepreciarAll(int mes, int year);
    Task<bool> CreateAfts(string description, DateTime date, decimal value, int quatity, string user);
    Task<bool> Edit(Guid id, string description, DateTime date, decimal value, string user);
}

public class AftService : IAftService
{
    private IAftRepository _aftRepo;

    public AftService(IAftRepository aftRepo)
    {
        _aftRepo = aftRepo;
    }

    public async Task<IEnumerable<Aft>> List()
    {
        return await _aftRepo.List();
    }

}
