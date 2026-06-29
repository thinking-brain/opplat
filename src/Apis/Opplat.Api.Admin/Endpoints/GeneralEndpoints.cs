using Opplat.Api.Common.Hosting;

namespace Opplat.Api.Admin.Endpoints;

public static class GeneralEndpoints
{
    public static void MapGeneralEndpoints(this WebApplication app)
    {
        app.MapOpplatHealthEndpoints("admin-api", "/healthcheck");
    }
}
