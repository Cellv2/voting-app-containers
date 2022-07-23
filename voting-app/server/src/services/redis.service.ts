import { createClient } from "redis";

// TODO: connect client on app startup - middleware this into vote.route?
// TODO: disconnect client on app shutdown
// TODO: handle client disconnects

const redisClient = createClient({
    socket: {
        host: "localhost",
        port: 6379,
    },
});
redisClient.connect();

export const incrementRedisVote = async (vote: string): Promise<void> => {
    try {
        await redisClient.incr(vote);
    } catch (err) {
        console.error(err);
    }
};
