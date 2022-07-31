using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;

namespace VotingAppContainers.Worker.App
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // var multiplexer = ConnectionMultiplexer.Connect("localhost");
            var multiplexer = ConnectionMultiplexer.Connect("redis");
            // ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
            //     new ConfigurationOptions
            //     {
            //         EndPoints = { "localhost:6379" }
            //     });
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.AddSingleton<IMongoClient>(sp =>
            {
                var connString = System.Environment.GetEnvironmentVariable("MONGODB_CONNSTRING");
                var mongodbSettings = MongoClientSettings.FromConnectionString(connString);
                mongodbSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
                IMongoClient client = new MongoClient(mongodbSettings);
                return client;
            });
        }
    }
}
