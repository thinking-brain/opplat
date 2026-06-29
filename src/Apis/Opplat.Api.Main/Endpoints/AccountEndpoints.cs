namespace Opplat.Api.Main.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        Opplat.Api.Account.Endpoints.AccountEndpoints.MapAccountEndpoints(app);
    }
}