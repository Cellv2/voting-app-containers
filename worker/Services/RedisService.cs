using StackExchange.Redis;
using VotingAppContainers.Worker.Models;

namespace VotingAppContainers.Worker.Services
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private ChannelMessageQueue _keyspaceSubscription;

        public RedisService(IConnectionMultiplexer redisMultiplexer)
        {
            _redis = redisMultiplexer;
            _keyspaceSubscription = CreateKeyspaceSubscription();
        }

        private ChannelMessageQueue CreateKeyspaceSubscription()
        {
            ISubscriber subscriber = _redis.GetSubscriber();
            return subscriber.Subscribe("__keyspace@0__:*");
        }

        public ChannelMessageQueue GetKeyspaceChannelMessageQueues()
        {
            return _keyspaceSubscription;
        }

        public async Task<string> GetVoteCounts(string option)
        {
            IDatabase db = _redis.GetDatabase();

            return (await db.StringGetAsync(option)).ToString();

        }

        public async Task<(string votesForOne, string votesForTwo)> GetVoteCounts()
        {
            IDatabase db = _redis.GetDatabase();

            var votesForOne = await db.StringGetAsync(VoteOption.One);
            var votesForTwo = await db.StringGetAsync(VoteOption.Two);

            var res = (votesForOne, votesForTwo);
            return res;
        }
    }
}
