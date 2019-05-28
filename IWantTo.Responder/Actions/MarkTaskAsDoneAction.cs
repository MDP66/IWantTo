using System;
using System.Threading.Tasks;
using IWantTo.Responder.Model;
using MongoDB.Driver;

namespace IWantTo.Responder.Actions
{
    public class MarkTaskAsDoneAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var chatId = context.Update.Message.Chat.Id;
            try
            {
                var usersCollection = context.Db.GetCollection<Entities.User>("Users");
                Guid taskId = ExtractTaskId(context);
                var userId = context.User.UserID;
                await MarkTaskAsDoneInDatabase(usersCollection, taskId, userId);
            }
            catch
            {
                await context.Client.SendTextMessageAsync(chatId, "تغییر وضعیت تسک انجام نشد");
            }
        }

        private async Task MarkTaskAsDoneInDatabase(IMongoCollection<Entities.User> usersCollection, Guid taskId, string userId)
        {
            var filter = Builders<Entities.User>.Filter;
            var userTaskFilter = filter.And(
                filter.Eq(c => c.UserID, userId),
                filter.ElemMatch(c => c.Tasks, d => d.Id == taskId)
            );

            var update = Builders<Entities.User>.Update;
            var updateTaskDate = update
                .Set("Tasks.$.IsDone", true)
                .Set("Tasks.$.IsDoneDate", DateTime.Now);


            await usersCollection.UpdateOneAsync(userTaskFilter, updateTaskDate);
        }

        private Guid ExtractTaskId(Context context)
        {
            var messageText = context.Update.Message.Text.Replace("/done_", "").Replace("_", "-");
            var taskId = Guid.Parse(messageText);
            return taskId;
        }
    }
}
