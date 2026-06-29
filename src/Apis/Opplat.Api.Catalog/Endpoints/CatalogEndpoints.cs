namespace Opplat.Api.Catalog.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var catalog = endpoints.MapGroup("/catalog").WithTags("Catalog");
        ProductEndpoints.MapProducts(catalog);
        ProductClassificationEndpoints.MapProductClassifications(catalog);
        ProductGroupEndpoints.MapProductGroups(catalog);
        UnitOfMeasurementEndpoints.MapUnitsOfMeasurement(catalog);
    }
}