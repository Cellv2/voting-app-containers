import express from "express";
import cors from "cors";

import voteRouter from "./routes/vote.route";

const app = express();
const port = 8080; // default port to listen

app.use(cors());

// define a route handler for the default home page
app.get("/", (req, res) => {
    res.send("Hello world!");
});

app.use("/vote", voteRouter);

// start the Express server
app.listen(port, () => {
    console.log(`server started at http://localhost:${port}`);
});
