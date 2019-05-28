using IWantTo.Responder.Extensions;
using IWantTo.Responder.Model;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class SetNewTaskDateAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            try
            {
                var messageText = context.Update.Message.Text;
                var chatId = context.Update.Message.Chat.Id;
                var date = messageText.ProcessDateFromString();
                if (date < DateTime.Now)
                {
                    await context.Client.SendTextMessageAsync(chatId, "تاریخ گذشته رفیق !");
                    return;
                }
                var usersCollection = context.Db.GetCollection<Entities.User>("Users");

                var filter = Builders<Entities.User>.Filter;
                var userTaskFilter = filter.And(
                    filter.Eq(c => c.UserID, context.User.UserID),
                    filter.ElemMatch(c => c.Tasks, d => !d.DoneDate.HasValue)
                );

                UpdateDefinitionBuilder<Entities.User> update = Builders<Entities.User>.Update;
                var updateTaskDate = update.Set("Tasks.$.DoneDate", date);


                await usersCollection.UpdateOneAsync(userTaskFilter, updateTaskDate);
            }
            catch (Exception e)
            {
            }
        }
    }
}
