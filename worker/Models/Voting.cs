using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VotingAppContainers.Worker.Models
{
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
        public Vote(string voteOption, int count)
        {
            this.voteOption = voteOption;
            this.count = count;
        }
    }
}
