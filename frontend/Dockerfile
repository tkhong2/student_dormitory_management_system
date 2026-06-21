# Build Vue/Vite app
FROM node:22 AS build

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

# Set NODE_OPTIONS to limit memory usage
ENV NODE_OPTIONS="--max-old-space-size=2048"

RUN npm run build

# Run bằng nginx
FROM nginx:stable-alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
