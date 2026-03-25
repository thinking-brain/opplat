FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
# Use npm install because the current Vite dependency tree resolves optional peers
# differently across npm versions and breaks npm ci in Docker.
RUN npm install --no-audit --no-fund

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

    location /admin/ {
        proxy_pass http://admin-api:8080;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

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
  VITE_ADMIN_API_URL: "${VITE_ADMIN_API_URL:-}"
};
CONFIG
EOF
RUN chmod +x /docker-entrypoint.d/40-runtime-config.sh

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
