import { createClient } from "redis";

// TODO: connect client on app startup - middleware this into vote.route?
// TODO: disconnect client on app shutdown
// TODO: handle client disconnects
// TODO: localhost / redis through env vars ?

const redisClient = createClient({
    socket: {
        host: "redis",
        port: 6379,
    },
});

try {
    redisClient.connect();
} catch (err) {
    console.error(err);
}

export const incrementRedisVote = async (vote: string): Promise<void> => {
    try {
        await redisClient.incr(vote);
    } catch (err) {
        console.error(err);
    }
};
