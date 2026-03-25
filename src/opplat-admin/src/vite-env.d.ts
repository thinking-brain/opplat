/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_APP_NAME?: string;
  readonly VITE_ADMIN_API_URL?: string;
  readonly VITE_DEV_PROXY_TARGET?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}

interface Window {
  __OPPLAT_RUNTIME_CONFIG__?: Partial<ImportMetaEnv>;
}

declare module '@mui/icons-material';
