using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Sales.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Domain.Sales.Repositories;


public interface IProductTagRepository: IRepository<ProductTag>
{

}

public interface IProductRepository: IRepository<ProductForSale>
{

}