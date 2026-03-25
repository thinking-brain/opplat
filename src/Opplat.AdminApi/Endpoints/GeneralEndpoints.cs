using Opplat.AdminApi.Hosting;

namespace Opplat.AdminApi.Endpoints;

public static class GeneralEndpoints
{
    public static void MapGeneralEndpoints(this WebApplication app)
    {
        app.MapOpplatHealthEndpoints("admin-api", "/healthcheck");
    }
}
