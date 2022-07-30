using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

void ConfigureServices(IServiceCollection services)
{
    var multiplexer = ConnectionMultiplexer.Connect("localhost");
    // ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
    //     new ConfigurationOptions
    //     {
    //         EndPoints = { "localhost:6379" }
    //     });
    services.AddSingleton<IConnectionMultiplexer>(multiplexer);

    services.AddSingleton<IMongoClient>(sp => {
        var mongodbSettings = MongoClientSettings.FromConnectionString("mongodb://");
        mongodbSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        IMongoClient client = new MongoClient(mongodbSettings);
        return client;
    });

    // MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<Vote>();


    // services.AddTransient<IMongoClient>(client);
    // IMongoClient x = new MongoClient();


    // services.AddTransient<IMongoClient>(x);
    // services.AddTransient<MongoDatabaseBase>(db2);
    // services.AddTransient<IMongoDatabase>(sp =>
    // {
    //     var mongodbSettings = MongoClientSettings.FromConnectionString("mongodb://");
    //     mongodbSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
    //     IMongoClient client = new MongoClient(mongodbSettings);
    //     var db = client.GetDatabase("votes");
    //     return db;
    // });
}

IServiceCollection serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);

var services = serviceCollection.BuildServiceProvider();
var redisConnectionMultiplexer = services.GetRequiredService<IConnectionMultiplexer>();

// var x = new RedisTest(redisConnectionMultiplexer);
// await x.ListKeysEverySecond();


// https://mongodb.github.io/mongo-csharp-driver/2.17/
var mongodbClient = services.GetRequiredService<IMongoClient>();
var mongoService = new MongoDbService(mongodbClient, "votes", "votes");

try
{

    // var connString = "mongodb://@mongodb";
    // var mongodb = new MongoClient(connString);

    // var connString = System.Environment.GetEnvironmentVariable("MONGODB_CONNSTRING")
    // var settings = MongoClientSettings.FromConnectionString("mongodb://@mongodb");
    var settings = MongoClientSettings.FromConnectionString("mongodb://");
    // Set the version of the Stable API on the client.
    settings.ServerApi = new ServerApi(ServerApiVersion.V1);
    var client = new MongoClient(settings);
    var database = client.GetDatabase("votes");
    // Console.WriteLine(client.ListDatabases().ToList());
    var collection = database.GetCollection<BsonDocument>("votes");
    var collections = await database.ListCollectionNamesAsync();
    Console.WriteLine(string.Join(",", collections.ToList()));


    // https://mongodb.github.io/mongo-csharp-driver/2.17/getting_started/quick_tour/
    // var voteOptionOne = new BsonDocument
    // {
    //     { "voteOption", "1" },
    //     { "count", 0 },
    // };
    // var voteOptionTwo = new BsonDocument
    // {
    //     { "voteOption", "2" },
    //     { "count", 0 },
    // };

    // await collection.InsertOneAsync(voteOptionOne);
    // await collection.InsertOneAsync(voteOptionTwo);


    // var votesCollection = database.GetCollection("votes");

    // database.


}
catch (Exception err)
{
    Console.WriteLine(err);
}


// TODO: just make it better
public class RedisTest
{
    private readonly IConnectionMultiplexer _redis;

    public RedisTest(IConnectionMultiplexer redisMultiplexer)
    {
        _redis = redisMultiplexer;
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

public class MongoDbService
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Vote> _collection;

    public MongoDbService(IMongoClient client, string dbName, string collectionName) {
        _client = client;

        // TODO: env var on db + collection
        // TODO: move this to DI? Probs won't change
        _database = client.GetDatabase(dbName);
        _collection = _database.GetCollection<Vote>(collectionName);

        CheckBaseVoteDocsExist();
    }

    private void CheckBaseVoteDocsExist()
    {
        // _collection.Find({"voteOption": "one"});
        var filterOne = Builders<Vote>.Filter.Eq("voteOption", VoteOption.One);
        var filterTwo = Builders<Vote>.Filter.Eq("voteOption", VoteOption.Two);
        var docOne = _collection.Find(filterOne).CountDocuments();
        var docTwo = _collection.Find(filterTwo).CountDocuments();

        if (docOne == 0) {
            CreateEmptyVoteDoc(VoteOption.One);
        }

        if (docTwo == 0) {
            CreateEmptyVoteDoc(VoteOption.Two);
        }

        var resultsOne = _collection.Find(filterOne).ToList();
        // var results = _collection.Find(x => x.voteOption == "1").ToList();
        Console.WriteLine(string.Join(",", resultsOne.Select(x => x.voteOption)));
        Console.WriteLine(string.Join(",", resultsOne.Select(x => x.count)));

        var resultsTwo = _collection.Find(filterTwo).ToList();
        // var results = _collection.Find(x => x.voteOption == "2").ToList();
        Console.WriteLine(string.Join(",", resultsTwo.Select(x => x.voteOption)));
        Console.WriteLine(string.Join(",", resultsTwo.Select(x => x.count)));
    }

    private void CreateEmptyVoteDoc(string voteOption) {
        var voteOptionOne = new Vote(voteOption, 0);

        _collection.InsertOne(voteOptionOne);
    }
}
