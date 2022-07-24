# What is this?

This is just a simple project meant to help solidifying knowledge on Docker. The idea is based on [this project](https://github.com/dockersamples/example-voting-app).

## Architecture

The planned architecture is to have two individual web apps, one with a Redis cache and one with a MongoDB database. There will then be a third app which will act as a go-between for Redis and the MongoDB database.

The first web app will allow a user to select an option, which will then write to Redis. This is split into two - the client (react) and the server (nodejs/express).

The worker (.NET 6) will then take the Redis key update and insert it into the MongoDB database.

The second web app will then display with votes within the MondoDB database.

This will be networked primarily through Docker networks, and could be done this way in its entirety. However, as I've wanted to learn a bit about nginx, I've added this around the first web app, allowing for different ports/URLs to access the web app itself, and for interaction from client to server as a form of proxy.

## Goals

The overall goals of this is to:

-   Be able to easily spin up a set of containers with the several separate elements of the app via docker-compose
-   Learn a little more .NET Core/.NET 6
-   Learn a little about nginx
-   Use this as a basis of a follow up project which will aim to put this into Kubernetes

## Commands

| Command                     | Action                                   |
| --------------------------- | ---------------------------------------- |
| `docker compose build`      | Builds the images                        |
| `docker compose up`         | Spins up everything                      |
| `docker compose up --build` | Rebuilds everything and then spins it up |
