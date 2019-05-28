
using IWantTo.Responder.Model;

using MongoDB.Driver;

using System;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class MarkTaskAsDeleteAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var chatId = context.Update.Message.Chat.Id;
            try
            {
                var usersCollection = context.Db.GetCollection<Entities.User>("Users");
                Guid taskId = ExtractTaskId(context);
                var userId = context.User.UserID;
                await MarkTaskAsDeleteInDatabase(usersCollection, taskId, userId);
                await context.Client.SendTextMessageAsync(chatId, "تسک مورد نظر حذف شد.");
            }
            catch
            {
                await context.Client.SendTextMessageAsync(chatId, "تغییر وضعیت تسک انجام نشد");
            }
        }

        private async Task MarkTaskAsDeleteInDatabase(IMongoCollection<Entities.User> usersCollection, Guid taskId, string userId)
        {
            var filter = Builders<Entities.User>.Filter;
            var userTaskFilter = filter.And(
                filter.Eq(c => c.UserID, userId),
                filter.ElemMatch(c => c.Tasks, d => d.Id == taskId)
            );

            var update = Builders<Entities.User>.Update;
            var updateCommand = update.PullFilter(c => c.Tasks, d => d.Id == taskId);


            await usersCollection.UpdateOneAsync(userTaskFilter, updateCommand);
        }

        private Guid ExtractTaskId(Context context)
        {
            var messageText = context.Update.Message.Text.Replace("/del_", "").Replace("_", "-");
            var taskId = Guid.Parse(messageText);
            return taskId;
        }
    }
}
