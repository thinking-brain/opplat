using Microsoft.Extensions.DependencyInjection;
using Opplat.Modules.Inventory.Domain.Repositories;
using Opplat.Modules.Inventory.Domain.Services;
using Opplat.Modules.Inventory.Infrastructure.Repositories;

namespace Opplat.Application.Inventory.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInventoryApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductClassificationService, ProductClassificationService>();
        services.AddScoped<IProductClassificationRepository, ProductClassificationRepository>();
        services.AddScoped<IProductGroupService, ProductGroupService>();
        services.AddScoped<IProductGroupRepository, ProductGroupRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductRepository, ProductsRepository>();
        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IStorageRepository, StorageRepository>();
        services.AddScoped<IMovementTypeService, MovementTypeService>();
        services.AddScoped<IProductMovementService, ProductMovementService>();
        services.AddScoped<IMovementsRepository, ProductMovementRepository>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        return services;
    }
}
