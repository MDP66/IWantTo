using System;
using System.Linq;
using IWantTo.Responder.Extensions;
using MongoDB.Driver;
using Telegram.Bot;

namespace IWantTo.Responder
{
    internal class NotificationSender
    {
        private TelegramBotClient TelegramClient;
        private IMongoDatabase Db;

        public NotificationSender(TelegramBotClient client, IMongoDatabase mongoDatabase)
        {
            TelegramClient = client;
            Db = mongoDatabase;
        }

        internal void SendGlobalNotifications()
        {
            var usersCollection = Db.GetCollection<Entities.User>("Users");
            foreach (var user in usersCollection.Find(c => c.Name != "").ToList())
            {
                foreach (var group in user.Groups)
                {
                    var group_tasks_message = "سلام به همه، " + Environment.NewLine + "آمار کارهای " + user.Name + " " + user.LastName + " رو می‌خوام بهتون بدم." + Environment.NewLine;
                    var counter = 1;
                    foreach (var task in user.Tasks.Where(c => c.IsPersonal == false && c.IsDone == false))
                    {
                        group_tasks_message += CreateGroupMessage(counter,user, task);
                        counter += 1;
                    }

                    TelegramClient.SendTextMessageAsync(group.ChatId, group_tasks_message,Telegram.Bot.Types.Enums.ParseMode.Html);
                }
            }

        }

        private static string CreateGroupMessage(int counter ,Entities.User user, Entities.ToDoTask task)
        {
            var isLate = task.DoneDate < DateTime.Now;
            
            var message =counter + " - قراره که ";
            if (isLate)
                message = counter + " - قرار بوده که ";
            message += " تا " + task.DoneDate.Value.ToPersianDate();
            message += " یعنی " + Math.Abs((DateTime.Now - task.DoneDate.Value).Days);
            if (isLate)
                message += "روز پیش ،<strong>" + task.Message + "</strong> رو انجام بده. که نداده.";
            else
                message += "روز آینده ،<strong>" + task.Message + "</strong> رو انجام بده. تشویقش کنید که موفق بشه.";

            return message + Environment.NewLine;
        }

        internal void SendPrivateNotifications()
        {
            var usersCollection = Db.GetCollection<Entities.User>("Users");
            foreach (var user in usersCollection.Find(c => c.Name != "").ToList())
            {
                var counter = 1;
                var tasks_message = "هی رفیق، یادته که کارای زیر رو باید انجام بدی یا باید انجام می دادای و ندادی ؟" + Environment.NewLine;
                foreach (var task in user.Tasks.Where(c => c.IsDone == false))
                {
                    var remainDays = (task.DoneDate.Value - DateTime.Now).Days;
                    var remainDaysText = "";
                    if (remainDays < 0) {
                        remainDaysText = $"یعنی از سر رسیدش یه {Math.Abs(remainDays)} روزی گذشته !!!!";
                    }
                    else
                    {
                        remainDaysText = $"یعنی یه {Math.Abs(remainDays)} روزی وقت داری هنوز.";
                    }
                    tasks_message += counter+" - "+ task.Message + " رو تا <strong>" + task.DoneDate.Value.ToPersianDate() + "</strong> تموم کنی ." + remainDaysText;
                    tasks_message += Environment.NewLine;
                    counter += 1;
                }
                TelegramClient.SendTextMessageAsync(user.UserID, tasks_message, Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }
    }
}