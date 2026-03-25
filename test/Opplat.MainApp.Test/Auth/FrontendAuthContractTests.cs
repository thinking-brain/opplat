namespace Opplat.MainApp.Test.Auth;

public class FrontendAuthContractTests
{
    [Fact]
    public void ClientOidcHelpers_RequestConfiguredScopeAndUseSafeCallbackRedirects()
    {
        var clientOidc = TestRepository.ReadAllText("src", "opplat-react", "src", "auth", "oidc.ts");

        Assert.Contains("scope: buildScope(appConfig.authScope)", clientOidc);
        Assert.Contains("requiredScopes = ['openid']", clientOidc);
        Assert.Contains("const state = user?.state;", clientOidc);
        Assert.Contains("typeof returnTo === 'string' && returnTo.startsWith('/')", clientOidc);
        Assert.Contains("window.location.replace(getReturnTo(user));", clientOidc);
        Assert.DoesNotContain("window.history.replaceState", clientOidc);
    }

    [Fact]
    public void ClientClaimHelpers_PreserveTenantAndRoleExtraction()
    {
        var clientClaims = TestRepository.ReadAllText("src", "opplat-react", "src", "auth", "claims.ts");

        Assert.Contains("'tenant_id'", clientClaims);
        Assert.Contains("'tenant_identifier'", clientClaims);
        Assert.Contains("'https://opplat.com/tenant_id'", clientClaims);
        Assert.Contains("'https://opplat.com/tenant_identifier'", clientClaims);
        Assert.Contains("'https://opplat.com/roles'", clientClaims);
        Assert.Contains("claims.realm_access", clientClaims);
        Assert.Contains("getRolesFromClaims", clientClaims);
    }

    [Fact]
    public void ClientRouteGuards_AlignWithDocumentedRoles()
    {
        var clientApp = TestRepository.ReadAllText("src", "opplat-react", "src", "App.tsx");
        var clientHomePage = TestRepository.ReadAllText("src", "opplat-react", "src", "pages", "HomePage.tsx");

        Assert.Contains("requiredRoles={appConfig.accessControl.tenantUserManagementRoles}", clientApp);
        Assert.Contains("TenantUser", clientHomePage);
    }

    [Fact]
    public void AdminFrontend_UsesDirectFeatureRoutesWithoutAuthBootstrap()
    {
        var adminApp = TestRepository.ReadAllText("src", "opplat-admin", "src", "App.tsx");
        var mainEntry = TestRepository.ReadAllText("src", "opplat-admin", "src", "main.tsx");
        var tenantsPage = TestRepository.ReadAllText("src", "opplat-admin", "src", "pages", "TenantsPage.tsx");
        var settingsPage = TestRepository.ReadAllText("src", "opplat-admin", "src", "pages", "SettingsPage.tsx");

        Assert.Contains("import { Layout } from './components/Layout';", adminApp);
        Assert.Contains("import { DashboardPage } from './pages/DashboardPage';", adminApp);
        Assert.Contains("import { TenantsPage } from './pages/TenantsPage';", adminApp);
        Assert.Contains("import { SettingsPage } from './pages/SettingsPage';", adminApp);
        Assert.Contains("<Route path=\"/\" element={<Layout />}", adminApp);
        Assert.Contains("<Route index element={<DashboardPage />} />", adminApp);
        Assert.Contains("<Route path=\"tenants\" element={<TenantsPage />} />", adminApp);
        Assert.Contains("<Route path=\"settings\" element={<SettingsPage />} />", adminApp);
        Assert.DoesNotContain("UsersPage", adminApp);
        Assert.DoesNotContain("path=\"users\"", adminApp);
        Assert.DoesNotContain("AuthCallbackPage", adminApp);
        Assert.DoesNotContain("ProtectedRoute", adminApp);
        Assert.DoesNotContain("TemporaryAdminShell", adminApp);
        Assert.Contains("Gestiona el catálogo multitenant", tenantsPage);
        Assert.Contains("La gestión de usuarios pertenece", settingsPage);

        Assert.DoesNotContain("AuthProvider", mainEntry);
    }

    [Fact]
    public void FrontendExamples_ReflectClientOidcAndAdminApiConfiguration()
    {
        var readme = TestRepository.ReadAllText("README.md");
        var clientExample = TestRepository.ReadAllText("src", "opplat-react", ".env.example");
        var adminExample = TestRepository.ReadAllText("src", "opplat-admin", ".env.example");

        Assert.Contains("VITE_AUTH_SCOPE=openid profile email offline_access", readme);
        Assert.Contains("VITE_AUTH_SCOPE=openid profile email offline_access", clientExample);

        Assert.Contains("VITE_ADMIN_API_URL=", readme);
        Assert.Contains("VITE_DEV_PROXY_TARGET=http://localhost:8084", readme);
        Assert.Contains("VITE_ADMIN_API_URL=", adminExample);
        Assert.DoesNotContain("VITE_AUTH_SCOPE=", adminExample);
        Assert.DoesNotContain("VITE_AUTH_AUTHORITY=", adminExample);
        Assert.DoesNotContain("VITE_AUTH_CLIENT_ID=", adminExample);
    }

    [Fact]
    public void FrontendRuntimeConfig_DefaultsMatchClientOidcAndAdminApiContracts()
    {
        var clientRuntimeConfig = TestRepository.ReadAllText("src", "opplat-react", "src", "runtimeConfig.ts");
        var adminRuntimeConfig = TestRepository.ReadAllText("src", "opplat-admin", "src", "runtimeConfig.ts");

        Assert.Contains("readConfig('VITE_AUTH_SCOPE', 'openid profile email offline_access')", clientRuntimeConfig);
        Assert.DoesNotContain("readConfig('VITE_AUTH_SCOPE', 'openid profile email roles')", clientRuntimeConfig);

        Assert.Contains("readConfig('VITE_ADMIN_API_URL', '')", adminRuntimeConfig);
        Assert.Contains("adminApiUrl: adminApiBaseUrl,", adminRuntimeConfig);
        Assert.DoesNotContain("apiUrl:", adminRuntimeConfig);
    }

    [Fact]
    public void DockerAndComposeRuntimeInjection_KeepClientOidcAndAdminApiSettingsSeparated()
    {
        var compose = TestRepository.ReadAllText("docker-compose.yml");
        var composeOverride = TestRepository.ReadAllText("docker-compose.override.yml");
        var clientDockerfile = TestRepository.ReadAllText("src", "opplat-react", "Dockerfile");
        var adminDockerfile = TestRepository.ReadAllText("src", "opplat-admin", "Dockerfile");

        Assert.Contains("VITE_AUTH_SCOPE=${VITE_AUTH_SCOPE:-openid profile email offline_access}", compose);
        Assert.Contains("VITE_ADMIN_API_URL=${VITE_ADMIN_API_URL:-}", compose);

        Assert.Contains("VITE_AUTH_SCOPE=openid profile email offline_access", composeOverride);
        Assert.Contains("VITE_DEV_PROXY_TARGET=http://admin-api:8080", composeOverride);
        Assert.Contains("VITE_ADMIN_API_URL=http://localhost:8084", composeOverride);

        Assert.Contains("VITE_AUTH_SCOPE: \"${VITE_AUTH_SCOPE:-openid profile email offline_access}\"", clientDockerfile);
        Assert.Contains("VITE_ADMIN_API_URL: \"${VITE_ADMIN_API_URL:-}\"", adminDockerfile);
    }

    [Fact]
    public void ClientAuthContext_UsesResolvedUserToClearLoadingAndStayAuthenticatedDuringRedirectRecovery()
    {
        var clientAuthContext = TestRepository.ReadAllText("src", "opplat-react", "src", "auth", "AuthContext.tsx");

        Assert.Contains("const hasResolvedUser = Boolean(oidc.user && !oidc.user.expired);", clientAuthContext);
        Assert.Contains("const isAuthenticated = oidc.isAuthenticated || hasResolvedUser;", clientAuthContext);
        Assert.Contains("const isNavigating = Boolean(oidc.activeNavigator) && !isAuthenticated;", clientAuthContext);
        Assert.Contains("loading: oidc.isLoading || isNavigating,", clientAuthContext);
        Assert.DoesNotContain("loading: oidc.isLoading || Boolean(oidc.activeNavigator),", clientAuthContext);
    }

    [Fact]
    public void AdminAxiosClient_KeepsTenantIsolationWithoutBrowserBearerTokens()
    {
        var adminAxiosClient = TestRepository.ReadAllText("src", "opplat-admin", "src", "api", "axiosClient.ts");
        var adminApi = TestRepository.ReadAllText("src", "opplat-admin", "src", "api", "admin.api.ts");
        var adminTypes = TestRepository.ReadAllText("src", "opplat-admin", "src", "types", "index.ts");

        Assert.Contains("setHeader(config, 'X-Tenant-Identifier', tenantIdentifier);", adminAxiosClient);
        Assert.DoesNotContain("Authorization", adminAxiosClient);
        Assert.Contains("adminAxiosClient.get<AdminTenant[]>('/admin/tenants')", adminApi);
        Assert.DoesNotContain("withTenantConfig(tenantIdentifier)", adminApi);
        Assert.DoesNotContain("/admin/users", adminApi);
        Assert.Contains("databaseName: string;", adminTypes);
        Assert.Contains("databaseSchema: string;", adminTypes);
        Assert.Contains("userCount: number;", adminTypes);
        Assert.DoesNotContain("connectionString", adminTypes, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AdminDashboard_BootstrapsFromTenantCatalogWithoutLegacyUserEndpoints()
    {
        var dashboardPage = TestRepository.ReadAllText("src", "opplat-admin", "src", "pages", "DashboardPage.tsx");

        Assert.Contains("const tenantResponse = await adminApi.listTenants();", dashboardPage);
        Assert.Contains("tenants.reduce((sum, tenant) => sum + tenant.userCount, 0)", dashboardPage);
        Assert.Contains("tenant.databaseName", dashboardPage);
        Assert.Contains("tenant.databaseSchema", dashboardPage);
        Assert.DoesNotContain("adminApi.listUsers()", dashboardPage);
        Assert.DoesNotContain("adminApi.listTenantUsers", dashboardPage);
        Assert.DoesNotContain("tenant.connectionString", dashboardPage);
    }

    [Fact]
    public void AdminTenantsPage_UsesDatabaseMetadataContractForCrud()
    {
        var tenantsPage = TestRepository.ReadAllText("src", "opplat-admin", "src", "pages", "TenantsPage.tsx");

        Assert.Contains("databaseName: string;", tenantsPage);
        Assert.Contains("databaseSchema: string;", tenantsPage);
        Assert.Contains("databaseName: tenant.databaseName,", tenantsPage);
        Assert.Contains("databaseSchema: tenant.databaseSchema,", tenantsPage);
        Assert.Contains("databaseName: formState.databaseName.trim(),", tenantsPage);
        Assert.Contains("databaseSchema: formState.databaseSchema.trim(),", tenantsPage);
        Assert.Contains("<TableCell>{tenant.databaseName}</TableCell>", tenantsPage);
        Assert.Contains("<TableCell>{tenant.databaseSchema}</TableCell>", tenantsPage);
        Assert.Contains("<TableCell>{tenant.userCount}</TableCell>", tenantsPage);
        Assert.DoesNotContain("connectionString", tenantsPage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ClientCallbackAndProtectedRoutes_PrioritizeResolvedSessionOverTransientAuthErrors()
    {
        var clientCallbackPage = TestRepository.ReadAllText("src", "opplat-react", "src", "auth", "AuthCallbackPage.tsx");
        var clientProtectedRoute = TestRepository.ReadAllText("src", "opplat-react", "src", "auth", "ProtectedRoute.tsx");

        AssertCallbackRecoveryContract(clientCallbackPage);
        AssertProtectedRouteRecoveredSessionContract(clientProtectedRoute);
    }

    private static void AssertCallbackRecoveryContract(string source)
    {
        Assert.Contains("const { error, isAuthenticated, loading, login", source);
        Assert.Contains("const navigate = useNavigate();", source);
        Assert.Contains("const hasRedirected = useRef(false);", source);
        Assert.Contains("if (isAuthenticated && !loading && !hasRedirected.current)", source);
        Assert.Contains("window.location.replace(", source);
        Assert.Contains("const showError = error && !isAuthenticated && !loading;", source);
        Assert.Contains("void login(", source);
    }

    private static void AssertProtectedRouteRecoveredSessionContract(string source)
    {
        Assert.Contains("const { isAuthenticated, loading, error, login, logout, roles } = useAuth();", source);
        Assert.Contains("if (error && !isAuthenticated)", source);
        Assert.DoesNotContain("if (error) {", source);
    }
}
