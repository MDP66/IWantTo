using IWantTo.Responder.Model;

using MongoDB.Driver;

using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class CancelAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var usersCollection = context.Db.GetCollection<Entities.User>("Users");
            var userId = context.User.UserID;
            var filter = Builders<Entities.User>.Filter.Eq(c => c.UserID, userId);
            var updateState = Builders<Entities.User>.Update.Set(c => c.State, TaskStateTypes.Empty.ToString());
            await usersCollection.UpdateOneAsync(filter, updateState);

            var deleteTaskUpdate = Builders<Entities.User>.Update.PullFilter(c => c.Tasks, d => d.DoneDate == null);
            await usersCollection.UpdateOneAsync(filter, deleteTaskUpdate);
        }
    }
}
