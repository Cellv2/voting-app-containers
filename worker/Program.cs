using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using MongoDB.Driver;
using VotingAppContainers.Worker.Classes;
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
await sync.SyncRedisIntoMongoDb();


public class DatabaseSync {
    private readonly MongoDbService _mongoDbService;
    private readonly RedisService _redisService;

    public DatabaseSync (MongoDbService mongoDbService, RedisService redisService) {
        _mongoDbService = mongoDbService;
        _redisService = redisService;
    }

    // TODO: move the delay out of here
    public async Task SyncRedisIntoMongoDb() {
        var counter = 0;
        var max = 100;

        while (max == -1 || counter < max)
        {
            var counts = await _redisService.GetVoteCounts();
            var numVotesForOne = counts.votesForOne;
            var numVotesForTwo = counts.votesForTwo;

            _mongoDbService.UpdateVoteDoc(VoteOption.One, numVotesForOne);
            _mongoDbService.UpdateVoteDoc(VoteOption.Two, numVotesForTwo);

            // counter++;
            await Task.Delay(TimeSpan.FromMilliseconds(1_000));
        }
    }
}
