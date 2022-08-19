using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using MongoDB.Driver;
using VotingAppContainers.Worker.Services;
using VotingAppContainers.Worker.App;


IServiceCollection serviceCollection = new ServiceCollection();
Startup.ConfigureServices(serviceCollection);

var services = serviceCollection.BuildServiceProvider();

// https://mongodb.github.io/mongo-csharp-driver/2.17/
var mongodbClient = services.GetRequiredService<IMongoClient>();
var mongoService = new MongoDbService(mongodbClient, "votes", "votes");


var redisConnectionMultiplexer = services.GetRequiredService<IConnectionMultiplexer>();
var redisService = new RedisService(redisConnectionMultiplexer);

var sync = new DatabaseSync(mongoService, redisService);
await sync.StreamRedisUpdatesIntoMongoDb();


public class DatabaseSync
{
    private readonly MongoDbService _mongoDbService;
    private readonly RedisService _redisService;

    public DatabaseSync(MongoDbService mongoDbService, RedisService redisService)
    {
        _mongoDbService = mongoDbService;
        _redisService = redisService;
    }

    public async Task StreamRedisUpdatesIntoMongoDb()
    {
        var subs = _redisService.GetKeyspaceChannelMessageQueues();

        // TODO: is there a nicer way to do this? rather than a wildcard keyspace notifiation?
        if (subs.Channel.ToString() == "__keyspace@0__:*")
        {
            subs.OnMessage(async message =>
            {
                // keyspace notification messages follow this format: __keyspace@0__:1:incrby
                // the channel is the first part of the message (__keyspace@0__:1), with the key itself being the second part
                var updatedKey = message.Channel.ToString().Split(":")[1];
                var votes = await _redisService.GetVoteCounts(updatedKey);
                Console.WriteLine($"key {updatedKey} was updated, the new value is {votes}");
                _mongoDbService.UpdateVoteDoc(updatedKey, votes);
            });
        }

        while (true)
        {
            // TODO: make the heartbeat better somehow?
            Console.WriteLine("Worker keep alive heartbeat");
            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }
}

