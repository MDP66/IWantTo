using IWantTo.Responder.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class SetNewTaskPrivacyAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            try
            {
                var messageText = context.Update.Message.Text;
                var chatId = context.Update.Message.Chat.Id;
                if (messageText != "بلی" && messageText != "خیر")
                {
                    await context.Client.SendTextMessageAsync(chatId, "بلی یا خیر لطفا !!!");
                    return;
                }
                var yesOrNo = messageText == "بلی" ? true : false;
                var usersCollection = context.Db.GetCollection<Entities.User>("Users");

                var filter = Builders<Entities.User>.Filter;
                var userTaskFilter = filter.And(
                    filter.Eq(c => c.UserID, context.User.UserID),
                    filter.ElemMatch(c => c.Tasks, d => !d.IsPersonal.HasValue)
                );

                UpdateDefinitionBuilder<Entities.User> update = Builders<Entities.User>.Update;
                var updateTaskDate = update.Set("Tasks.$.IsPersonal", yesOrNo);


                await usersCollection.UpdateOneAsync(userTaskFilter, updateTaskDate);
            }
            catch (Exception e)
            {
            }
        }
    }
}
