using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IWantTo.Responder.Entities
{
    public class Group
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }


        [BsonElement("ChatId")]
        public long ChatId { get; set; }
    }
}
