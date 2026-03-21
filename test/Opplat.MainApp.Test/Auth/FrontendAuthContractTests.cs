using System.IO;

namespace Opplat.MainApp.Test.Auth;

public class FrontendAuthContractTests
{
    [Fact]
    public void FrontendOidcClients_RequestConfiguredScopePlusRealmSafeDefaults()
    {
        var clientOidc = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "oidc.ts"));
        var adminOidc = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "oidc.ts"));

        AssertRequestedScopeContract(clientOidc);
        AssertRequestedScopeContract(adminOidc);
    }

    [Fact]
    public void FrontendCallbackHandlers_UseHardRedirectToLeaveTheCallbackRoute()
    {
        var clientOidc = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "oidc.ts"));
        var adminOidc = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "oidc.ts"));

        AssertCallbackNavigationContract(clientOidc);
        AssertCallbackNavigationContract(adminOidc);
    }

    [Fact]
    public void FrontendCallbackHandlers_OnlyReuseSafeRelativeReturnTargets()
    {
        var clientOidc = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "oidc.ts"));
        var adminOidc = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "oidc.ts"));

        AssertCallbackReturnTargetContract(clientOidc);
        AssertCallbackReturnTargetContract(adminOidc);
    }

    [Fact]
    public void FrontendClaimParsing_ReadsTenantAndRealmRoleClaimsFromKeycloakTokens()
    {
        var clientClaims = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "claims.ts"));
        var adminClaims = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "claims.ts"));

        AssertClaimParsingContract(clientClaims);
        AssertClaimParsingContract(adminClaims);
    }

    [Fact]
    public void FrontendRouteGuards_AlignWithDocumentedRoles()
    {
        var adminApp = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "App.tsx"));
        var clientApp = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "App.tsx"));
        var adminUsersPage = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "pages", "UsersPage.tsx"));
        var clientHomePage = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "pages", "HomePage.tsx"));

        Assert.Contains("requiredRoles={appConfig.accessControl.adminPortalRoles}", adminApp);
        Assert.Contains("requiredRoles={appConfig.accessControl.tenantUserManagementRoles}", clientApp);
        Assert.Contains("TenantUser", clientHomePage);
        Assert.Contains("Roles válidos:", adminUsersPage);
        Assert.Contains("TENANT_ADMIN_ROLE", adminUsersPage);
        Assert.Contains("TENANT_USER_ROLE", adminUsersPage);
        Assert.DoesNotContain("AdminOnly", adminUsersPage);
    }

    [Fact]
    public void ScopeDocumentation_AlignsAcrossReadmeAndFrontendExamples()
    {
        const string expectedScope = "openid profile email offline_access";
        var readme = File.ReadAllText(ResolveRepoFile("README.md"));
        var clientExample = File.ReadAllText(ResolveRepoFile("src", "opplat-react", ".env.example"));
        var adminExample = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", ".env.example"));

        Assert.Contains(expectedScope, readme);
        Assert.Contains($"VITE_AUTH_SCOPE={expectedScope}", clientExample);
        Assert.Contains($"VITE_AUTH_SCOPE={expectedScope}", adminExample);
    }

    [Fact]
    public void FrontendRuntimeConfig_DefaultsMatchTheDocumentedKeycloakScopeContract()
    {
        const string expectedScope = "openid profile email offline_access";
        const string forbiddenScope = "openid profile email roles";

        var clientRuntimeConfig = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "runtimeConfig.ts"));
        var adminRuntimeConfig = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "runtimeConfig.ts"));

        AssertRuntimeScopeContract(clientRuntimeConfig, expectedScope, forbiddenScope);
        AssertRuntimeScopeContract(adminRuntimeConfig, expectedScope, forbiddenScope);
    }

    [Fact]
    public void DockerAndComposeRuntimeInjection_KeepTheKeycloakSafeScopeContract()
    {
        const string expectedScope = "openid profile email offline_access";
        const string forbiddenScope = "openid profile email roles";

        var compose = File.ReadAllText(ResolveRepoFile("docker-compose.yml"));
        var composeOverride = File.ReadAllText(ResolveRepoFile("docker-compose.override.yml"));
        var clientDockerfile = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "Dockerfile"));
        var adminDockerfile = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "Dockerfile"));

        Assert.Contains($"VITE_AUTH_SCOPE=${{VITE_AUTH_SCOPE:-{expectedScope}}}", compose);
        Assert.Contains($"VITE_AUTH_SCOPE=${{VITE_ADMIN_AUTH_SCOPE:-{expectedScope}}}", compose);
        Assert.DoesNotContain(forbiddenScope, compose);

        Assert.Contains($"VITE_AUTH_SCOPE={expectedScope}", composeOverride);
        Assert.DoesNotContain(forbiddenScope, composeOverride);

        Assert.Contains($"VITE_AUTH_SCOPE: \"${{VITE_AUTH_SCOPE:-{expectedScope}}}\"", clientDockerfile);
        Assert.Contains($"VITE_AUTH_SCOPE: \"${{VITE_AUTH_SCOPE:-{expectedScope}}}\"", adminDockerfile);
        Assert.DoesNotContain(forbiddenScope, clientDockerfile);
        Assert.DoesNotContain(forbiddenScope, adminDockerfile);
    }

    [Fact]
    public void AuthContexts_StopShowingLoadingOnceTheUserIsResolvedAfterSignin()
    {
        var clientAuthContext = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "AuthContext.tsx"));
        var adminAuthContext = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "AuthContext.tsx"));

        AssertResolvedUserLoadingContract(clientAuthContext);
        AssertResolvedUserLoadingContract(adminAuthContext);
    }

    [Fact]
    public void AuthContexts_TreatARestoredNonExpiredUserAsAuthenticatedDuringRedirectRecovery()
    {
        var clientAuthContext = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "AuthContext.tsx"));
        var adminAuthContext = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "AuthContext.tsx"));

        AssertResolvedUserAuthenticationContract(clientAuthContext);
        AssertResolvedUserAuthenticationContract(adminAuthContext);
    }

    [Fact]
    public void CallbackPages_PrioritizeResolvedSessionOverTransientAuthErrors()
    {
        var clientCallbackPage = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "AuthCallbackPage.tsx"));
        var adminCallbackPage = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "AuthCallbackPage.tsx"));

        AssertCallbackPageRecoveryContract(clientCallbackPage);
        AssertCallbackPageRecoveryContract(adminCallbackPage);
    }

    [Fact]
    public void ReactOidcProviders_WireSigninCallbackAndPersistUsersInBrowserStorage()
    {
        var clientOidc = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "oidc.ts"));
        var adminOidc = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "oidc.ts"));
        var clientAuthContext = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "AuthContext.tsx"));
        var adminAuthContext = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "AuthContext.tsx"));

        AssertRestoredSessionStorageContract(clientOidc);
        AssertRestoredSessionStorageContract(adminOidc);
        AssertOidcProviderCallbackContract(clientAuthContext);
        AssertOidcProviderCallbackContract(adminAuthContext);
    }

    [Fact]
    public void FrontendApps_ExposeDedicatedSigninAndSilentRenewCallbackRoutes()
    {
        var clientApp = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "App.tsx"));
        var adminApp = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "App.tsx"));

        AssertCallbackRouteContract(clientApp);
        AssertCallbackRouteContract(adminApp);
    }

    [Fact]
    public void ProtectedRoutes_DoNotLetTransientAuthErrorsOverrideRestoredSessions()
    {
        var clientProtectedRoute = File.ReadAllText(ResolveRepoFile("src", "opplat-react", "src", "auth", "ProtectedRoute.tsx"));
        var adminProtectedRoute = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "ProtectedRoute.tsx"));

        AssertProtectedRouteRecoveredSessionContract(clientProtectedRoute);
        AssertProtectedRouteRecoveredSessionContract(adminProtectedRoute);
    }

    [Fact]
    public void AdminAuthProvider_UsesOfficialSettingsDrivenReactOidcContextPattern()
    {
        var adminAuthContext = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "AuthContext.tsx"));

        Assert.Contains("import { oidcSettings, onSigninCallback } from './oidc';", adminAuthContext);
        Assert.Contains("<OidcProvider {...oidcSettings} onSigninCallback={onSigninCallback}>", adminAuthContext);
        Assert.DoesNotContain("<OidcProvider userManager={oidcUserManager} onSigninCallback={onSigninCallback}>", adminAuthContext);
    }

    [Fact]
    public void AdminAxiosAuthHelpers_ReadTheStoredOidcUserWithoutSharingAUserManagerInstance()
    {
        var adminOidc = File.ReadAllText(ResolveRepoFile("src", "opplat-admin", "src", "auth", "oidc.ts"));

        Assert.Contains("return User.fromStorageString(serializedUser);", adminOidc);
        Assert.Contains("readStorage()?.removeItem(oidcUserStorageKey);", adminOidc);
        Assert.DoesNotContain("new UserManager(oidcSettings)", adminOidc);
        Assert.DoesNotContain("oidcUserManager.getUser()", adminOidc);
        Assert.DoesNotContain("oidcUserManager.removeUser()", adminOidc);
    }

    private static void AssertRequestedScopeContract(string source)
    {
        Assert.Contains("scope: buildScope(appConfig.authScope)", source);
        Assert.Contains("requiredScopes = ['openid']", source);
        Assert.DoesNotContain("requiredScopes = ['openid', 'profile', 'email', 'roles']", source);
        Assert.DoesNotContain("scopes.add('roles')", source);
    }

    private static void AssertCallbackNavigationContract(string source)
    {
        Assert.True(
            source.Contains("if (window.self !== window.top)", StringComparison.Ordinal) ||
            source.Contains("if (globalThis.self !== globalThis.top)", StringComparison.Ordinal),
            "Expected signin callback to skip iframe silent-renew navigations.");
        Assert.True(
            source.Contains("window.location.replace(getReturnTo(user));", StringComparison.Ordinal) ||
            source.Contains("globalThis.location.replace(getReturnTo(user));", StringComparison.Ordinal),
            "Expected signin callback to hard-redirect away from the callback route.");
        Assert.DoesNotContain("window.history.replaceState", source);
        Assert.DoesNotContain("PopStateEvent", source);
    }

    private static void AssertCallbackReturnTargetContract(string source)
    {
        Assert.Contains("const state = user?.state;", source);
        Assert.Contains("typeof returnTo === 'string' && returnTo.startsWith('/')", source);
        Assert.Contains("return '/';", source);
    }

    private static void AssertRuntimeScopeContract(string source, string expectedScope, string forbiddenScope)
    {
        Assert.Contains($"readConfig('VITE_AUTH_SCOPE', '{expectedScope}')", source);
        Assert.DoesNotContain($"readConfig('VITE_AUTH_SCOPE', '{forbiddenScope}')", source);
    }

    private static void AssertClaimParsingContract(string source)
    {
        Assert.Contains("'tenant_id'", source);
        Assert.Contains("'tenant_identifier'", source);
        Assert.Contains("'https://opplat.com/tenant_id'", source);
        Assert.Contains("'https://opplat.com/tenant_identifier'", source);
        Assert.Contains("'https://opplat.com/roles'", source);
        Assert.Contains("claims.realm_access", source);
        Assert.Contains("getRolesFromClaims", source);
    }

    private static void AssertResolvedUserLoadingContract(string source)
    {
        Assert.Contains("const hasResolvedUser = Boolean(oidc.user && !oidc.user.expired);", source);
        Assert.Contains("const isAuthenticated = oidc.isAuthenticated || hasResolvedUser;", source);
        Assert.Contains("const isNavigating = Boolean(oidc.activeNavigator) && !isAuthenticated;", source);
        Assert.Contains("loading: oidc.isLoading || isNavigating,", source);
        Assert.DoesNotContain("const isNavigating = Boolean(oidc.activeNavigator) && !oidc.isAuthenticated && !oidc.user;", source);
        Assert.DoesNotContain("loading: oidc.isLoading || Boolean(oidc.activeNavigator),", source);
    }

    private static void AssertResolvedUserAuthenticationContract(string source)
    {
        Assert.Contains("const hasResolvedUser = Boolean(oidc.user && !oidc.user.expired);", source);
        Assert.Contains("const isAuthenticated = oidc.isAuthenticated || hasResolvedUser;", source);
        Assert.Contains("isAuthenticated,", source);
        Assert.DoesNotContain("isAuthenticated: oidc.isAuthenticated,", source);
    }

    private static void AssertCallbackPageRecoveryContract(string source)
    {
        Assert.Contains("const { error, isAuthenticated, loading, login } = useAuth();", source);
        Assert.Contains("const navigate = useNavigate();", source);
        Assert.Contains("const hasRedirected = useRef(false);", source);
        Assert.Contains("if (isAuthenticated && !loading && !hasRedirected.current)", source);
        Assert.Contains("navigate('/', { replace: true });", source);
        Assert.Contains("window.location.replace('/');", source);
        Assert.Contains("if (isAuthenticated && !loading)", source);
        Assert.Contains("return null;", source);
        Assert.Contains("const showError = error && !isAuthenticated && !loading;", source);
        Assert.Contains("void login('/');", source);
    }

    private static void AssertRestoredSessionStorageContract(string source)
    {
        Assert.Contains("WebStorageStateStore", source);
        Assert.Contains("userStore: new WebStorageStateStore", source);
        Assert.True(
            source.Contains("store: window.localStorage", StringComparison.Ordinal) ||
            source.Contains("store: globalThis.localStorage", StringComparison.Ordinal),
            "Expected OIDC user state to be persisted in browser localStorage for restored sessions.");
    }

    private static void AssertOidcProviderCallbackContract(string source)
    {
        Assert.Contains("onSigninCallback={onSigninCallback}", source);
        Assert.True(
            source.Contains("userManager={oidcUserManager}", StringComparison.Ordinal) ||
            source.Contains("{...oidcSettings}", StringComparison.Ordinal),
            "Expected AuthProvider to wire the OIDC configuration together with onSigninCallback.");
    }

    private static void AssertCallbackRouteContract(string source)
    {
        Assert.Contains("path=\"/auth/callback\"", source);
        Assert.Contains("path=\"/auth/silent-renew\"", source);
        Assert.Contains("AuthCallbackPage", source);
    }

    private static void AssertProtectedRouteRecoveredSessionContract(string source)
    {
        Assert.Contains("const { isAuthenticated, loading, error, login, logout, roles } = useAuth();", source);
        Assert.Contains("if (error && !isAuthenticated)", source);
        Assert.DoesNotContain("if (error) {", source);
    }

    private static string ResolveRepoFile(params string[] segments)
    {
        var current = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(current))
        {
            if (File.Exists(Path.Combine(current, "opplat.sln")))
            {
                return Path.Combine(new[] { current }.Concat(segments).ToArray());
            }

            current = Directory.GetParent(current)?.FullName!;
        }

        throw new DirectoryNotFoundException("Could not locate repository root from test output directory.");
    }
}
