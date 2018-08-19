FROM node:9-alpine AS build

COPY package.json ./
COPY package-lock.json ./
RUN npm set progress=false && npm config set depth 0 && npm cache clean --force

## Storing node modules on a separate layer will prevent unnecessary npm installs at each build
RUN npm i && mkdir /ng-app && cp -R ./node_modules ./ng-app

WORKDIR /ng-app
COPY . .
## Build the angular app in production
RUN $(npm bin)/ng build --prod

### Setup ###
FROM nginx:1.13.3-alpine AS final
COPY nginx/default.conf /etc/nginx/conf.d/
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build /ng-app/dist/OnyxShiftScheduler /usr/share/nginx/html

CMD ["nginx", "-g", "daemon off;"]
