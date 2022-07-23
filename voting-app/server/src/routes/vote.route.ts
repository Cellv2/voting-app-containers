import express from "express";
import { incrementRedisVote } from "../services/redis.service";

const router = express.Router();

// please note that this is limited to 1 or 2 as the params
router.post("/:option(1|2)", async (req, res) => {
    console.log("POST - we called the redis route");
    const { option } = req.params;
    console.log(`the option was ${option}`);
    try {
        await incrementRedisVote(option);
    } catch (err) {
        console.error(err);
    }

    res.sendStatus(204);
});

export default router;
