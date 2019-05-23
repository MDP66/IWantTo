using System;
using MongoDB.Bson.Serialization.Attributes;

namespace IWantTo.Responder.Entities
{
    public class ToDoTask
    {
        public Guid Id { get; set; }
        
        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonElement("DoneDate")]
        public DateTime? DoneDate { get; set; }

        [BsonElement("SendDate")]
        public DateTime SendDate { get; set; }


        [BsonElement("IsDone")]
        public bool IsDone { get; set; }


        [BsonElement("IsDoneDate")]
        public DateTime? IsDoneDate { get; set; }

    }
}
