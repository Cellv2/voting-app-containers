using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

void ConfigureServices(IServiceCollection services)
{
    var multiplexer = ConnectionMultiplexer.Connect("localhost");
    // ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
    //     new ConfigurationOptions
    //     {
    //         EndPoints = { "localhost:6379" }
    //     });
    services.AddSingleton<IConnectionMultiplexer>(multiplexer);
}

IServiceCollection serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);

var services = serviceCollection.BuildServiceProvider();
var redisConnectionMultiplexer = services.GetRequiredService<IConnectionMultiplexer>();

var x = new RedisTest(redisConnectionMultiplexer);
await x.ListKeysEverySecond();

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
