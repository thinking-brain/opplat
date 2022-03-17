using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Inventario.Domain.Repositories;
using Opplat.Inventario.Domain.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Infrastructure.Inventario.Repositories;

public class AlmacenRepository : IAlmacenRepository
{

    public async Task<Almacen> Find(Guid id)
    {
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<Almacen>> List(int page = 0, int pageSize = 0)
    {
        throw new NotImplementedException();
    }
    public async Task<RepositoryResponse> Create(Almacen entity)
    {
        throw new NotImplementedException();
    }
    public async Task<RepositoryResponse> Update(Almacen entity)
    {
        throw new NotImplementedException();
    }
    public async Task<RepositoryResponse> Delete(Guid id)
    {
        throw new NotImplementedException();
    }
    public async Task<RepositoryResponse> Delete(Almacen entity)
    {
        throw new NotImplementedException();
    }

    public Task<RepositoryResponse> AddMovimiento(MovimientoDeProducto movimiento)
    {
        throw new NotImplementedException();
    }

    public Task<Almacen> GetPrimaryStorage()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ValeDeSalida>> GetValesDeSalida(DateTime fechaInicial, DateTime fechaFinal)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<DetalleDeVale>> GetDetallesDeVale(Guid valeId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovimientoDeProducto>> GetMermas(Guid almacenId)
    {
        throw new NotImplementedException();
    }

    public Task<Almacen> Find(string id)
    {
        throw new NotImplementedException();
    }

    public Task<RepositoryResponse> Delete(string id)
    {
        throw new NotImplementedException();
    }
}
