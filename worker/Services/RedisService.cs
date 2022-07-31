using StackExchange.Redis;
using VotingAppContainers.Worker.Classes;

namespace VotingAppContainers.Worker.Services
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisService(IConnectionMultiplexer redisMultiplexer)
        {
            _redis = redisMultiplexer;
        }

        public async Task<(string votesForOne, string votesForTwo)> GetVoteCounts()
        {
            IDatabase db = _redis.GetDatabase();
            // IServer server = _redis.GetServer(_redis.GetEndPoints()[0]);
            // var keys = server.Keys();

            // foreach (var key in keys)
            // {
            //     var val = db.StringGet(key);
            //     Console.WriteLine($"{key} : {val.ToString()}");
            // }

            var votesForOne = await db.StringGetAsync(VoteOption.One);
            var votesForTwo = await db.StringGetAsync(VoteOption.Two);

            var res = (votesForOne, votesForTwo);
            return res;
        }
    }
}
