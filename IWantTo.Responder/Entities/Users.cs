
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IWantTo.Responder.Entities
{
    public class Users
    {
        public Users()
        {
            Tasks = new List<ToDoTask>();
            Groups = new List<Group>();
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("LastName")]
        public string LastName { get; set; }
        [BsonElement("UserID")]
        public string UserID { get; set; }
        [BsonElement("Username")]
        public string Username { get; set; }
        [BsonElement("State")]
        public string State { get; set; }

        [BsonElement("Tasks")]
        public IList<ToDoTask> Tasks { get; set; }

        [BsonElement("Groups")]
        public IList<Group> Groups { get; set; }


    }
}
