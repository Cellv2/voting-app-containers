# What is this?

This is just a simple project meant to help solidifying knowledge on Docker

## Architecture

The planned architecture is to have two individual web apps, one with a Redis cache and one with a MongoDB database. There will then be a third app which will act as a go-between for Redis and the MongoDB database.

## Goals

The overall goals of this is to:

-   Be able to easily spin up a set of containers with the several separate elements of the app via docker-compose
-   Learn a little more .NET Core/.NET 6 (used as the Redis-MonboDB interface)
-   Use this as a basis of a follow up project which will aim to put this into Kubernetes
