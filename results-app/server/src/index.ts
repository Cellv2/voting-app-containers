import { MongoClient } from "mongodb";

import "dotenv/config";

console.log("hiya!");

const uri = `mongodb://${process.env.DEV_MONGODB_USER}:${process.env.DEV_MONGODB_PASS}@localhost:27017`;
const client = new MongoClient(uri);

async function run() {
    try {
        const database = client.db("votes");
        const collection = database.collection("votes");
        const cursor = collection.find({});
        await cursor.forEach((item) => console.log(item));

        // const queryForOne = { voteOption: "1" };
        // const queryForTwo = { voteOption: "2" };
    } catch (err) {
        console.error(err);
    } finally {
        // Ensures that the client will close when you finish/error
        await client.close();
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
