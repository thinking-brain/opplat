FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
# Use npm ci for production: requires exact lock file match to ensure reproducible builds
RUN npm ci

COPY . .
RUN npm run build

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

RUN rm -rf ./*
COPY --from=build /app/dist .

RUN cat <<'EOF' > /etc/nginx/conf.d/default.conf
server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
EOF

RUN cat <<'EOF' > /docker-entrypoint.d/40-runtime-config.sh
#!/bin/sh
cat <<CONFIG > /usr/share/nginx/html/runtime-config.js
window.__OPPLAT_RUNTIME_CONFIG__ = {
  VITE_APP_NAME: "${VITE_APP_NAME:-Opplat Admin}",
  VITE_API_URL: "${VITE_API_URL:-http://localhost:5000}",
  VITE_ADMIN_API_URL: "${VITE_ADMIN_API_URL:-${VITE_API_URL:-http://localhost:5000}}",
  VITE_AUTH_AUTHORITY: "${VITE_AUTH_AUTHORITY:-http://localhost:8180/realms/opplat}",
  VITE_AUTH_CLIENT_ID: "${VITE_AUTH_CLIENT_ID:-opplat-admin}",
  VITE_AUTH_AUDIENCE: "${VITE_AUTH_AUDIENCE:-opplat-api}",
  VITE_AUTH_SCOPE: "${VITE_AUTH_SCOPE:-openid profile email offline_access}",
  VITE_AUTH_REDIRECT_PATH: "${VITE_AUTH_REDIRECT_PATH:-/auth/callback}",
  VITE_AUTH_SILENT_REDIRECT_PATH: "${VITE_AUTH_SILENT_REDIRECT_PATH:-/auth/silent-renew}",
  VITE_AUTH_LOGOUT_REDIRECT_PATH: "${VITE_AUTH_LOGOUT_REDIRECT_PATH:-/login}"
};
CONFIG
EOF
RUN chmod +x /docker-entrypoint.d/40-runtime-config.sh

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
