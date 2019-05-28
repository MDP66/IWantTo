using IWantTo.Responder.Model;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class MarkTaskAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var state = GetUserState(context.Args).ToString();
            var usersCollection = context.Db.GetCollection<Entities.User>("Users");
            await usersCollection.UpdateOneAsync(
                Builders<Entities.User>.Filter.Eq(c => c.UserID, context.User.UserID),
                Builders<Entities.User>.Update.Set(c => c.State, state));
        }

        private TaskStateTypes GetUserState(object state)
        {
            try
            {
                Enum.TryParse(typeof(TaskStateTypes), state.ToString(), true, out var taskState);
                return (TaskStateTypes)taskState;
            }
            catch (Exception)
            {
                return TaskStateTypes.UnKnown;
            }
        }
    }
}
