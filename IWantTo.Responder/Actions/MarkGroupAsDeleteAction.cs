using IWantTo.Responder.Model;

using MongoDB.Driver;

using System;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class MarkGroupAsDeleteAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var chatId = context.Update.Message.Chat.Id;
            try
            {
                var usersCollection = context.Db.GetCollection<Entities.User>("Users");
                var groupId = ExtractTaskId(context).ToString();
                var userId = context.User.UserID;
                await MarkTaskAsDeleteInDatabase(usersCollection, groupId, userId);
                await context.Client.SendTextMessageAsync(chatId, "ارسال اطلاعیه به گروه مد نظر حذف شد.");
            }
            catch(Exception ex)
            {
                await context.Client.SendTextMessageAsync(chatId, "حذف اطلاع رسانی انجام نشد");
            }
        }

        private async Task MarkTaskAsDeleteInDatabase(IMongoCollection<Entities.User> usersCollection, string groupId, string userId)
        {
            var filter = Builders<Entities.User>.Filter;
            var userTaskFilter = filter.And(
                filter.Eq(c => c.UserID, userId),
                filter.ElemMatch(c => c.Groups, d => d.CustomId == groupId)
            );

            var update = Builders<Entities.User>.Update;
            var updateGroupSendNotification = update
                .Set("Groups.$.SendNotification", false);


            await usersCollection.UpdateOneAsync(userTaskFilter, updateGroupSendNotification);
        }

        private Guid ExtractTaskId(Context context)
        {
            var messageText = context.Update.Message.Text.Replace("/group_del_", "").Replace("_", "-");
            var taskId = Guid.Parse(messageText);
            return taskId;
        }
    }
}
