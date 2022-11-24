using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Services;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Repositories;
using Microsoft.Extensions.Logging;

namespace Opplat.Domain.Inventory.Services;

public interface IProductClassificationService : IService<ProductClassification, int>
{

}

public class ProductClassificationService : BaseService<ProductClassification, int>, IProductClassificationService
{
    public ProductClassificationService(IProductClassificationRepository repo, ILogger<ProductClassification> logger) : base(repo, logger)
    {
    }
}

public interface IProductGroupService : IService<ProductGroup, int>
{

}

public class ProductGroupService : BaseService<ProductGroup, int>, IProductGroupService
{
    public ProductGroupService(IProductGroupRepository repo, ILogger<ProductGroup> logger) : base(repo,logger)
    {
    }

    public override async Task<ServiceResponse<ProductGroup>> Get(int id)
    {
        var repo = base._repo as IProductGroupRepository;
        var entity = await repo.GetWithClassification(id);
        return new ServiceResponse<ProductGroup>
        {
            Value = entity,
            Message = "Entity found.",
            Status = ServiceStatus.Ok,
        };
    }

    public override async Task<ServiceResponse<ProductGroup>> List(int? page, int? itemsPerPage)
    {
        var repo = base._repo as IProductGroupRepository;
        var list = await repo.ListWithClassification();
        var result = new ServiceResponse<ProductGroup>
        {
            Status = ServiceStatus.Ok,
            Message = "List of entities.",
            List = list,
        };
        return result;
    }
}

public interface IProductService : IService<Product, string>
{

}

public class ProductService : BaseService<Product, string>, IProductService
{
    public ProductService(IProductRepository repo, ILogger<Product> logger) : base(repo, logger)
    {
    }

    public override async Task<ServiceResponse<Product>> Get(string id)
    {
        var repo = base._repo as IProductRepository;
        var entity = await repo.GetWithGroup(id);
        return new ServiceResponse<Product>
        {
            Value = entity,
            Message = "Entity found.",
            Status = ServiceStatus.Ok,
        };
    }

    public override async Task<ServiceResponse<Product>> List(int? page, int? itemsPerPage)
    {
        var repo = base._repo as IProductRepository;
        var list = await repo.ListWithGroup();
        var result = new ServiceResponse<Product>
        {
            Status = ServiceStatus.Ok,
            Message = "List of entities.",
            List = list,
        };
        return result;
    }
}
