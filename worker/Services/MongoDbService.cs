using MongoDB.Driver;
using VotingAppContainers.Worker.Models;

namespace VotingAppContainers.Worker.Services
{
    // TODO: can we set up an index for the voteOptions field?
    // https://www.mongodb.com/docs/manual/reference/operator/update/inc/
    public class MongoDbService
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Vote> _collection;

        private FilterDefinition<Vote> _filterVoteOptionOne = Builders<Vote>.Filter.Eq("voteOption", VoteOption.One);
        private FilterDefinition<Vote> _filterVoteOptionTwo = Builders<Vote>.Filter.Eq("voteOption", VoteOption.Two);

        public MongoDbService(IMongoClient client, string dbName, string collectionName)
        {
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

            if (docOne == 0)
            {
                CreateEmptyVoteDoc(VoteOption.One);
            }

            if (docTwo == 0)
            {
                CreateEmptyVoteDoc(VoteOption.Two);
            }

            var resultsOne = _collection.Find(_filterVoteOptionOne).ToList();
            Console.WriteLine(string.Join(",", resultsOne.Select(x => x.voteOption)));
            Console.WriteLine(string.Join(",", resultsOne.Select(x => x.count)));

            var resultsTwo = _collection.Find(_filterVoteOptionTwo).ToList();
            Console.WriteLine(string.Join(",", resultsTwo.Select(x => x.voteOption)));
            Console.WriteLine(string.Join(",", resultsTwo.Select(x => x.count)));
        }

        private void CreateEmptyVoteDoc(string voteOption)
        {
            var voteOptionOne = new Vote(voteOption, 0);

            _collection.InsertOne(voteOptionOne);
        }

        public void UpdateVoteDoc(string voteOption, string newValue)
        {
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
}
