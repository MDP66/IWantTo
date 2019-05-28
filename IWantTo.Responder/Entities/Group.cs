using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace IWantTo.Responder.Entities
{
    public class Group
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("CustomId")]
        public string CustomId { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }


        [BsonElement("ChatId")]
        public long ChatId { get; set; }


        [BsonElement("SendNotification")]
        public bool SendNotification { get; set; }

    }
}
