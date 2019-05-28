
using IWantTo.Responder.Entities;
using IWantTo.Responder.Model;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class SetNewTaskMessageAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            try
            {
                var usersCollection = context.Db.GetCollection<User>("Users");
                var filter = Builders<User>.Filter;
                var userFilter = filter.Eq(c => c.UserID, context.User.UserID);

                var guid = Guid.NewGuid();
                var task = new ToDoTask
                {
                    Message = context.Update.Message.Text,
                    Id = guid,
                    SendDate = DateTime.Now,
                    DoneDate = null,
                    IsPersonal = null
                };

                var update = Builders<User>.Update;
                var updateCommand = update.Push(c => c.Tasks, task);

                await usersCollection.UpdateOneAsync(userFilter, updateCommand);
            }
            catch (Exception e)
            {
                await context.Client.SendTextMessageAsync(context.Update.Message.Chat.Id, "یه چیزی پکید !");
            }
        }
    }
}
