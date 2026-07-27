using Opplat.Api.Account.Endpoints;
using Opplat.Api.Admin.Endpoints;
using Opplat.Api.Catalog.Endpoints;
using Opplat.Api.Common.Hosting;
using Opplat.Api.Inventory.Endpoints;
using Opplat.Api.Invoicing.Endpoints;
using Opplat.Api.Sales.Endpoints;

namespace Opplat.Api.Main.Extensions;

public static class RegisterEndpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapOpplatHealthEndpoints("main-api");
        app.MapAdminEndpoints();
        app.MapAccountEndpoints();
        app.MapInventoryEndpoints();
        app.MapInvoicingEndpoints();
        app.MapSalesEndpoints();
        app.MapCatalogEndpoints();
    }
}