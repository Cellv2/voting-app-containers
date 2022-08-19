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

        // TODO: constant the keyspaces and channels to make them easy to update
        public void CreateSubscriptions2()
        {
            ISubscriber subscriber = _redis.GetSubscriber();
            // subscriber.Subscribe("__keyspace@0__:*", (channel, value) =>
            // {
            //     if (channel.ToString() == "__keyspace@0__:1")
            //     {
            //         Console.WriteLine($"key 1 was updated, the new value is {value}");
            //     }
            //     else if (channel.ToString() == "__keyspace@0__:2")
            //     {
            //         Console.WriteLine($"key 2 was updated, the new value is {value}");
            //     }
            // });

            var x = subscriber.Subscribe("__keyspace@0__:1");

            subscriber.Subscribe("__keyspace@0__:1").OnMessage(message =>
            {
                Console.WriteLine($"key 1 was updated, the new value is {GetVoteCounts(VoteOption.One).Result}");
                // GetVoteCounts()
            });

            subscriber.Subscribe("__keyspace@0__:2").OnMessage(message =>
            {
                Console.WriteLine($"key 2 was updated, the new value is {GetVoteCounts(VoteOption.Two).Result}");
            });
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

        public async Task<string> GetVoteCounts(string option)
        {
            IDatabase db = _redis.GetDatabase();
            // IServer server = _redis.GetServer(_redis.GetEndPoints()[0]);
            // var keys = server.Keys();

            // foreach (var key in keys)
            // {
            //     var val = db.StringGet(key);
            //     Console.WriteLine($"{key} : {val.ToString()}");
            // }

            return (await db.StringGetAsync(option)).ToString();

            // var votesForOne = await db.StringGetAsync(VoteOption.One);
            // var votesForTwo = await db.StringGetAsync(VoteOption.Two);

            // var res = (votesForOne, votesForTwo);
            // return res;
        }
    }
}
