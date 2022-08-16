import cors from "cors";
import express from "express";
import path from "path";
import ws, { WebSocket } from 'ws'

import "dotenv/config";

import mongodbSvc from "./services/mongodb";


const app = express();
app.use(cors());
app.use(express.static(path.join(__dirname, "public")));

app.get('/');
app.get("/votes", async (req, res) => {
    try {
        const data = await mongodbSvc.findAllVotes();
        const formatted = data
            .map((datum) => `<p>${datum.voteOption} : ${datum.count}</p>`)
            .join("");

        res.send(formatted);
    } catch (err) {
        console.error(err);
    }
});


const generateRandomMessage = () => {
    return "" + Math.floor(Math.random() * 100);

}

const wss = new WebSocket.Server({ port: 8085 });

const clients = new Map();

let interval: NodeJS.Timer;

wss.on('connection', (ws) => {

    // set up the interval before adding any clients
    if (!clients.size){
        interval = setInterval(() => {
            // multicast example
            [...clients.keys()].forEach((client) => {
                client.send(JSON.stringify(generateRandomMessage()));
            });
        }, 1000);
    }

    const id = Math.random();
    const metadata = { id };

    clients.set(ws, metadata);
});

wss.on("close", () => {
  clients.delete(ws);
  if (!clients.size) {
    clearInterval(interval);
  }
});


console.log("hiya!");

const dbName = "votes";
// dbName is required, else mongoose defaults to 'test'
// authSource is required else auth against the dbName won't be allowed, as auth has not been configured against that db
// https://www.mongodb.com/docs/manual/reference/connection-string/#components
const uri = `mongodb://${process.env.DEV_MONGODB_USER}:${process.env.DEV_MONGODB_PASS}@localhost:27017/${dbName}?authSource=admin`;
// const uri = process.env.MONGODB_CONNSTRING + `/${dbName}?authSource=admin`;

// TODO: remove, this was for testing
const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

async function run() {
    try {
        // TODO: remove, this was for testing
        for (let i = 0; i < 5; i++) {
            await delay(5000);

            // await mongodbSvc.findAllVotes();

            // TODO: remove - this was testing async initialisation
            const [votes1, votes2] = await Promise.all([
                mongodbSvc.findAllVotes(),
                mongodbSvc.findAllVotes()

            ])

            console.log(votes1)
            console.log(votes2)
        }
    } catch (err) {
        console.error(err);
    } finally {
        // Ensures that the client will close when you finish/error
        // TODO: need some kind of connection close on shutdown
        // await mongoose.connection.close();
    }
}

run().catch(console.dir);

app.listen(8081, () => {
    console.log(`server started at http://localhost:8081`);
});

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
