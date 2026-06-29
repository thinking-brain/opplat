namespace Opplat.Api.Catalog.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this WebApplication app)
    {
        var catalog = app.MapGroup("/catalog").WithTags("Catalog");
        ProductEndpoints.MapProducts(catalog);
        ProductClassificationEndpoints.MapProductClassifications(catalog);
        ProductGroupEndpoints.MapProductGroups(catalog);
        UnitOfMeasurementEndpoints.MapUnitsOfMeasurement(catalog);
    }
}