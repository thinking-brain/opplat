/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_APP_NAME?: string;
  readonly VITE_API_URL?: string;
  readonly VITE_AUTH_API_URL?: string;
  readonly VITE_SALES_API_URL?: string;
  readonly VITE_INVENTORY_API_URL?: string;
  readonly VITE_ADMIN_API_URL?: string;
  readonly VITE_DEV_PROXY_TARGET?: string;
  readonly VITE_AUTH_AUTHORITY?: string;
  readonly VITE_AUTH_CLIENT_ID?: string;
  readonly VITE_AUTH_AUDIENCE?: string;
  readonly VITE_AUTH_SCOPE?: string;
  readonly VITE_AUTH_USE_AUDIENCE_QUERY_PARAM?: string;
  readonly VITE_AUTH_REDIRECT_PATH?: string;
  readonly VITE_AUTH_SILENT_REDIRECT_PATH?: string;
  readonly VITE_AUTH_LOGOUT_REDIRECT_PATH?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}

interface Window {
  __OPPLAT_RUNTIME_CONFIG__?: Partial<ImportMetaEnv>;
}
