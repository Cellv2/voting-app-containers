import "dotenv/config";

export const DB_NAME = "votes";

export const MONGODB_CONNSTRING =
    (process.env.MONGODB_CONNSTRING ||
        `mongodb://${process.env.DEV_MONGODB_USER}:${process.env.DEV_MONGODB_PASS}@localhost:27017`) +
    `/${DB_NAME}?authSource=admin`;
