FROM node:13.7.0-alpine

WORKDIR /app
COPY . .
# COPY package.json /app/
# COPY lib /app/lib
# COPY index.js /app/
# COPY config.js /app/config.js
RUN npm install
EXPOSE 389

CMD [ "node","index.js" ]