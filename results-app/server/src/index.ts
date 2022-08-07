import mongoose, { model, Schema } from "mongoose";

import "dotenv/config";

console.log("hiya!");

const dbName = "votes";
// dbName is required, else mongoose defaults to 'test'
// authSource is required else auth against the dbName won't be allowed, as auth has not been configured against that db
// https://www.mongodb.com/docs/manual/reference/connection-string/#components
const uri = `mongodb://${process.env.DEV_MONGODB_USER}:${process.env.DEV_MONGODB_PASS}@localhost:27017/${dbName}?authSource=admin`;

interface Vote {
    voteOption: string;
    count: number;
}

const voteSchema = new Schema<Vote>({
    voteOption: { type: String, required: true },
    count: { type: Number, required: true },
});

// https://github.com/Automattic/mongoose/blob/master/examples/schema/schema.js

const Vote = model<Vote>("vote", voteSchema);

// TODO: remove, this was for testing
const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

async function run() {
    try {
        // TODO: remove, this was for testing
        for (let i = 0; i < 5; i++) {
            await delay(5000);

            await mongoose.connect(uri);

            const findAllVotes = await Vote.find({});
            console.log(findAllVotes);
        }
    } catch (err) {
        console.error(err);
    } finally {
        // Ensures that the client will close when you finish/error
        await mongoose.connection.close();
    }
}

run().catch(console.dir);

// we want to make this realtime, cause why not
// https://www.mongodb.com/basics/change-streams
// https://www.mongodb.com/developer/languages/javascript/nodejs-change-streams-triggers/

// conn = new Mongo("YOUR_CONNECTION_STRING");
// db = conn.getDB('blog');
// const collection = db.collection('comment');
// const changeStream = collection.watch();
// changeStream.on('change', next => {
//   // do something when there is a change in the comment collection
// });
