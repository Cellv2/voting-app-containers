using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

void ConfigureServices(IServiceCollection services)
{
    // var multiplexer = ConnectionMultiplexer.Connect("localhost");
    var multiplexer = ConnectionMultiplexer.Connect("redis");
    // ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
    //     new ConfigurationOptions
    //     {
    //         EndPoints = { "localhost:6379" }
    //     });
    services.AddSingleton<IConnectionMultiplexer>(multiplexer);

    services.AddSingleton<IMongoClient>(sp => {
        var connString = System.Environment.GetEnvironmentVariable("MONGODB_CONNSTRING");
        var mongodbSettings = MongoClientSettings.FromConnectionString(connString);
        mongodbSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        IMongoClient client = new MongoClient(mongodbSettings);
        return client;
    });
}

IServiceCollection serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);

var services = serviceCollection.BuildServiceProvider();
// https://mongodb.github.io/mongo-csharp-driver/2.17/
var mongodbClient = services.GetRequiredService<IMongoClient>();
var mongoService = new MongoDbService(mongodbClient, "votes", "votes");


var redisConnectionMultiplexer = services.GetRequiredService<IConnectionMultiplexer>();

var x = new RedisTest(redisConnectionMultiplexer, mongoService);
await x.ListKeysEverySecond();




// TODO: just make it better
public class RedisTest
{
    private readonly IConnectionMultiplexer _redis;
    private readonly MongoDbService _mongoDbService;

    // TODO: get rid of the mongoDbService instance, it shouldn't be here
    public RedisTest(IConnectionMultiplexer redisMultiplexer, MongoDbService mongoDbService)
    {
        _redis = redisMultiplexer;
        _mongoDbService = mongoDbService;
    }

    public async Task ListKeysEverySecond()
    {
        IDatabase db = _redis.GetDatabase();
        IServer server = _redis.GetServer(_redis.GetEndPoints()[0]);

        var counter = 0;
        var max = 100;

        while (max == -1 || counter < max)
        {
            Console.WriteLine($"Counter: {++counter}");

            var keys = server.Keys();

            foreach (var key in keys)
            {
                var val = await db.StringGetAsync(key);
                if (key == "1" || key == "2") {
                    _mongoDbService.UpdateVoteDoc(key, val);
                }
                Console.WriteLine($"{key} : {val.ToString()}");
            }

            // Console.WriteLine(string.Join(",", keys.ToList()));
            await Task.Delay(TimeSpan.FromMilliseconds(1_000));
        }
    }
}

public class VoteOption
{
    public static string One => "1";
    public static string Two => "2";
}

public class Vote
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public string voteOption { get; set; }
    public int count { get; set; }

    [BsonConstructor]
    public Vote (string voteOption, int count) {
        this.voteOption = voteOption;
        this.count = count;
    }
}


// TODO: can we set up an index for the voteOptions field?
// TODO: add increments on redis updates (?) or do we just update it to the value shown in redis
// https://www.mongodb.com/docs/manual/reference/operator/update/inc/
public class MongoDbService
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Vote> _collection;

    private FilterDefinition<Vote> _filterVoteOptionOne = Builders<Vote>.Filter.Eq("voteOption", VoteOption.One);
    private FilterDefinition<Vote> _filterVoteOptionTwo = Builders<Vote>.Filter.Eq("voteOption", VoteOption.Two);

    public MongoDbService(IMongoClient client, string dbName, string collectionName) {
        _client = client;

        // TODO: env var on db + collection
        // TODO: move this to DI? Probs won't change
        _database = client.GetDatabase(dbName);
        _collection = _database.GetCollection<Vote>(collectionName);

        CheckBaseVoteDocsExist();
    }

    // TODO: look at upserting on doc update? we could probably then remove this
    // as we do not upsert, we check that the doc exists on client initialisation
    private void CheckBaseVoteDocsExist()
    {
        var docOne = _collection.Find(_filterVoteOptionOne).CountDocuments();
        var docTwo = _collection.Find(_filterVoteOptionTwo).CountDocuments();

        if (docOne == 0) {
            CreateEmptyVoteDoc(VoteOption.One);
        }

        if (docTwo == 0) {
            CreateEmptyVoteDoc(VoteOption.Two);
        }

        var resultsOne = _collection.Find(_filterVoteOptionOne).ToList();
        Console.WriteLine(string.Join(",", resultsOne.Select(x => x.voteOption)));
        Console.WriteLine(string.Join(",", resultsOne.Select(x => x.count)));

        var resultsTwo = _collection.Find(_filterVoteOptionTwo).ToList();
        Console.WriteLine(string.Join(",", resultsTwo.Select(x => x.voteOption)));
        Console.WriteLine(string.Join(",", resultsTwo.Select(x => x.count)));
    }

    private void CreateEmptyVoteDoc(string voteOption) {
        var voteOptionOne = new Vote(voteOption, 0);

        _collection.InsertOne(voteOptionOne);
    }

    public void UpdateVoteDoc(string voteOption, string newValue) {
        var update = Builders<Vote>.Update.Set("count", newValue);

        if (voteOption == "1")
        {
            _collection.UpdateOne(_filterVoteOptionOne, update);
            return;
        }

        if (voteOption == "2")
        {
            _collection.UpdateOne(_filterVoteOptionTwo, update);
            return;
        }
    }
}
