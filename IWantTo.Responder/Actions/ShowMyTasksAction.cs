using IWantTo.Responder.Extensions;
using IWantTo.Responder.Model;

using MongoDB.Driver;

using System;
using System.Linq;
using System.Threading.Tasks;

using Telegram.Bot.Types.Enums;

namespace IWantTo.Responder.Actions
{
    public class ShowMyTasksAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var userId = context.User.UserID;
            var chatId = context.Update.Message.Chat.Id;
            Entities.User myTasks = await GetUser(context, userId);

            if (myTasks.Tasks == null || myTasks.Tasks.All(c => c.IsDone))
                await context.Client.SendTextMessageAsync(chatId, "شما هیچ تسک بازی ندارید");
            else
            {
                foreach (var myTask in myTasks.Tasks.ToList().Where(c => !c.IsDone))
                {
                    var messageText = BuildTaskMessage(chatId, myTask);
                    await context.Client.SendTextMessageAsync(chatId, messageText, ParseMode.Html);
                }
            }
        }

        private string BuildTaskMessage(long chatId, Entities.ToDoTask myTask)
        {
            var taskCode = myTask.Id.ToString().Replace("-", "_");
            var remainDays = (myTask.DoneDate.Value - DateTime.Now).Days;
            var remainDaysText = "";
            if (remainDays > 0)
            {
                remainDaysText = " - " +  remainDays + " روز مانده است";
            }
            else
            {
                remainDaysText = " - " + remainDays + " روز گذشته است";
            }

            var message =
                "تسک : <strong>" + myTask.Message + "</strong> (" + (myTask.IsPersonal.HasValue && myTask.IsPersonal.Value == true ? "خصوصی" : "عمومی") + ")"
                + Environment.NewLine
                + "تاریخ انجام : " + myTask.DoneDate.Value.ToPersianDate() + remainDaysText
                + Environment.NewLine
                + "حذف تسک : /del_" + taskCode
                + Environment.NewLine
                + "انجام دادن /done_" + taskCode;
            return message;
        }

        private async Task<Entities.User> GetUser(Context context, string userId)
        {
            var usersCollection = context.Db.GetCollection<Entities.User>("Users");
            var myTasks = (await usersCollection.FindAsync(Builders<Entities.User>.Filter.Eq(c => c.UserID, userId))).SingleOrDefault();
            return myTasks;
        }
    }
}
