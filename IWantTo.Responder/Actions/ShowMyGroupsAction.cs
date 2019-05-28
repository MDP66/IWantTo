using IWantTo.Responder.Model;

using MongoDB.Driver;

using System;
using System.Linq;
using System.Threading.Tasks;

using Telegram.Bot.Types.Enums;

namespace IWantTo.Responder.Actions
{
    public class ShowMyGroupsAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var userId = context.User.UserID;
            var chatId = context.Update.Message.Chat.Id;

            if (context.User.Groups == null || context.User.Groups.All(c => !c.SendNotification))
                await context.Client.SendTextMessageAsync(chatId, "شما هیچ گروهی برای ارسال اطلاعیه ندارید");
            else
            {
                foreach (var myGroup in context.User.Groups.ToList().Where(c => c.SendNotification))
                {
                    var messageText = BuildTaskMessage(chatId, myGroup);
                    await context.Client.SendTextMessageAsync(chatId, messageText, ParseMode.Html);
                }
            }
        }

        private string BuildTaskMessage(long chatId, Entities.Group group)
        {
            var groupId = group.CustomId.ToString().Replace("-", "_");
            var message =
                "عنوان گروه : <strong>" + group.Name + "</strong>"
                + Environment.NewLine
                + "غیر فعال سازی اطلاع رسانی : /group_del_" + groupId;

            return message;
        }

    }
}
