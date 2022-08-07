FROM node:alpine

WORKDIR /usr/app
COPY ./ /usr/app
RUN npm i

# https://docs.docker.com/engine/reference/builder/#cmd
# shell is not used, so vars will not be present (i.e. npm ....)
# this leads to things like "exec: "npm": executable file not found in $PATH: unknown"
CMD [ "sh", "-c", "npm start"]
