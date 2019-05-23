using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IWantTo.Responder.Bot;
using IWantTo.Responder.Entities;
using MongoDB.Driver;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IWantTo.Responder
{
    class Program
    {
        public static TelegramBotClient client;
        public static MongoClient mongoClient;
        public static IMongoDatabase mongoDatabase;

        public static ManualResetEvent _Shutdown = new ManualResetEvent(false);
        public static ManualResetEventSlim _Complete = new ManualResetEventSlim();
        static void Main(string[] args)
        {
            try
            {
                var ended = new ManualResetEventSlim();
                var starting = new ManualResetEventSlim();
                client = BotClientFactory.CreateClient();
                mongoClient = new MongoClient("mongodb://root:vwEU3c4dLm2dvcJQGwGWyMcs@s5.liara.ir:34274");
                mongoDatabase = mongoClient.GetDatabase("IWantToDb");
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri("amqp://zxmblypp:R4eBqMzeU1yBzUvWxoCajpQVXRcm99M_@albatross.rmq.cloudamqp.com/zxmblypp")
                };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var str = Encoding.UTF8.GetString(body);
                            var message = JsonConvert.DeserializeObject<Update>(str);
                            UpdateReceived(message);
                        };
                        channel.BasicConsume(queue: "messages",
                            autoAck: true,
                            consumer: consumer);
                        _Shutdown.WaitOne();
                    }

                }
            }
            catch (Exception e)
            {
            }

            Console.Write("Exiting...");
            _Complete.Set();
        }




        //private static async void RunScheduler()
        //{
        //    try
        //    {
        //        // Grab the Scheduler instance from the Factory
        //        NameValueCollection props = new NameValueCollection
        //        {
        //            { "quartz.serializer.type", "binary" }
        //        };
        //        StdSchedulerFactory factory = new StdSchedulerFactory(props);
        //        IScheduler scheduler = await factory.GetScheduler();

        //        // and start it off
        //        await scheduler.Start();

        //        // define the job and tie it to our HelloJob class
        //        IJobDetail job = JobBuilder.Create<EveryNightJob>()
        //            .WithIdentity("job1", "group1")
        //            .Build();

        //        // Trigger the job to run now, and then repeat every 10 seconds
        //        ITrigger trigger = TriggerBuilder.Create()
        //            .WithIdentity("trigger1", "group1")
        //            .StartNow()
        //            .WithSimpleSchedule(x => x
        //                .WithIntervalInHours(24)
        //                .RepeatForever())
        //            .Build();

        //        // Tell quartz to schedule the job using our trigger
        //        await scheduler.ScheduleJob(job, trigger);
        //    }
        //    catch (SchedulerException se)
        //    {
        //        Console.WriteLine(se);
        //    }
        //}


        private static void UpdateReceived(Update update)
        {
            ValidateUser(update.Message.From).GetAwaiter().GetResult();
            ValidateGroups(update.Message).GetAwaiter().GetResult();
            if (update.Message.Chat.Type != ChatType.Private) return;

            if (update.Message.Text.ToLower() == "/start")
            {
                ShowActions(update.Message.Chat.Id);
                return;
            }

            if (update.Message.Text.ToLower() == "/new")
            {
                CreateNewTask(update.Message.Chat.Id, update.Message.From.Id.ToString()).GetAwaiter().GetResult();
                return;
            }

            if (update.Message.Text.ToLower() == "/cancel")
            {
                CancelNewTask(update.Message.Chat.Id, update.Message.From.Id.ToString()).GetAwaiter().GetResult();
                ShowActions(update.Message.Chat.Id);
                return;
            }

            if (update.Message.Text.ToLower() == "/mytasks")
            {
                ShowMyTasks(update.Message.Chat.Id, update.Message.From.Id.ToString()).GetAwaiter().GetResult();
                return;
            }

            if (update.Message.Text.ToLower().StartsWith("/done_"))
            {
                DoneTask(update.Message.Chat.Id, update.Message.From.Id.ToString(), update.Message.Text).GetAwaiter().GetResult(); ;
                ShowMyTasks(update.Message.Chat.Id, update.Message.From.Id.ToString()).GetAwaiter().GetResult(); ;
                return;
            }

            if (update.Message.Text.ToLower().StartsWith("/del_"))
            {
                DeleteTask(update.Message.Chat.Id, update.Message.From.Id.ToString(), update.Message.Text).GetAwaiter().GetResult(); ;
                ShowMyTasks(update.Message.Chat.Id, update.Message.From.Id.ToString()).GetAwaiter().GetResult(); ;
                return;
            }
            ValidateUserState(update.Message, update.Message.Chat.Id).GetAwaiter().GetResult(); ;
        }

        private static async Task ValidateGroups(Message eMessage)
        {
            if (eMessage.Chat.Type == ChatType.Group || eMessage.Chat.Type == ChatType.Supergroup)
            {
                var usersCollection = mongoDatabase.GetCollection<Users>("Users");
                var filter = Builders<Users>.Filter;
                var userFilter = filter.Eq(c => c.UserID, eMessage.From.Id.ToString());
                var existenceFilter = filter.ElemMatch(c => c.Groups, d => d.ChatId == eMessage.Chat.Id);
                var userIsNotJoindThisGroupYet = (await usersCollection.FindAsync(existenceFilter)).SingleOrDefault() == null;
                if (userIsNotJoindThisGroupYet)
                {
                    var group = new Group()
                    {
                        Name = eMessage.Chat.Title,
                        ChatId = eMessage.Chat.Id
                    };

                    var update = Builders<Users>.Update;
                    var updateCommand = update.Push(c => c.Groups, group);

                    await usersCollection.UpdateOneAsync(userFilter, updateCommand);

                }
            }
        }

        private static async Task DoneTask(long chatId, string userId, string messageText)
        {
            try
            {
                var usersCollection = mongoDatabase.GetCollection<Users>("Users");
                messageText = messageText.Replace("/done_", "").Replace("_", "-");
                var taskId = Guid.Parse(messageText);
                var filter = Builders<Users>.Filter;
                var userTaskFilter = filter.And(
                    filter.Eq(c => c.UserID, userId),
                    filter.ElemMatch(c => c.Tasks, d => d.Id == taskId)
                );

                var update = Builders<Users>.Update;
                var updateTaskDate = update
                    .Set("Tasks.$.IsDone", true)
                    .Set("Tasks.$.IsDoneDate", DateTime.Now);


                await usersCollection.UpdateOneAsync(userTaskFilter, updateTaskDate);
            }
            catch (Exception e)
            {
                await client.SendTextMessageAsync(chatId, "تغییر وضعیت تسک انجام نشد");
            }
        }

        private static async Task ValidateUserState(Message message, long chatId)
        {
            var usersCollection = mongoDatabase.GetCollection<Users>("Users");
            var userList = await usersCollection.FindAsync(c => c.UserID == message.From.Id.ToString());
            var user = await userList.FirstAsync();
            switch (user.State.ToLower())
            {
                case "new":
                    await SetUserTaskMessage(message.Text, user.UserID);
                    await ShowDateMessage(chatId);
                    await MarkUserAsInDateMode(user.UserID);
                    break;
                case "date":
                    await SetUserTaskDate(message.Chat.Id, user.UserID, message.Text);
                    await ShowDoneMessage(chatId);
                    break;
                default:
                    break;
            }
        }

        private static async Task ShowDoneMessage(long chatId)
        {
            await client.SendTextMessageAsync(chatId, "حله.حالا از این به بعد من می دونم با تو.باید کارتو تموم کنی.یادت نره که من بدجور آبرو برم.اگر می‌خوای لیست کاراتو ببینی می‌تونی از دستور /mytasks استفاده کنی");
        }

        private static async Task SetUserTaskDate(long chatId, string userId, string messageText)
        {

            var date = processDate(messageText);
            if (date < DateTime.Now)
            {
                client.SendTextMessageAsync(0, "تاریخ گذشته رفیق !");
                return;
            }
            var usersCollection = mongoDatabase.GetCollection<Users>("Users");

            var filter = Builders<Users>.Filter;
            var userTaskFilter = filter.And(
                filter.Eq(c => c.UserID, userId),
                filter.ElemMatch(c => c.Tasks, d => !d.DoneDate.HasValue)
            );

            var update = Builders<Users>.Update;
            var updateTaskDate = update.Set("Tasks.$.DoneDate", date);


            await usersCollection.UpdateOneAsync(userTaskFilter, updateTaskDate);
        }

        private static DateTime processDate(string messageText)
        {
            var persianCalendar = new PersianCalendar();
            var det = '/';
            if (messageText.Contains('/'))
            {
                det = '/';
            }
            else if (messageText.Contains('\\'))
            {
                det = '\\';
            }
            else if (messageText.Contains('-'))
            {
                det = '-';
            }

            var parts = messageText.Split(det);
            int year = 0, month = 0, day = 0;
            year = int.Parse(parts[0]);
            month = int.Parse(parts[1]);
            day = int.Parse(parts[2]);
            if (year < day)
            {
                day = year;
                year = int.Parse(parts[2]);
            }

            return persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }

        private static async Task SetUserTaskMessage(string messageText, string userId)
        {
            try
            {
                var usersCollection = mongoDatabase.GetCollection<Users>("Users");
                var filter = Builders<Users>.Filter;
                var userFilter = filter.Eq(c => c.UserID, userId);

                var guid = Guid.NewGuid();
                var task = new ToDoTask
                {
                    Message = messageText,
                    Id = guid,
                    SendDate = DateTime.Now,
                    DoneDate = null
                };

                var update = Builders<Users>.Update;
                var updateCommand = update.Push(c => c.Tasks, task);

                await usersCollection.UpdateOneAsync(userFilter, updateCommand);
            }
            catch (Exception e)
            {
                // await client.SendTextMessageAsync(chatId, "یه چیزی پکید !");
            }
        }

        private static async Task ShowDateMessage(long chatId)
        {
            await client.SendTextMessageAsync(chatId, "حالا کی تموم میشه این کارت؟ فقط جون هرکی دوست داری درست بنویس تاریخ رو.");
            await client.SendTextMessageAsync(chatId, "مثل این : 1398/01/13");
        }

        private static async Task CancelNewTask(long chatId, string userId)
        {
            var usersCollection = mongoDatabase.GetCollection<Users>("Users");
            var filter = Builders<Users>.Filter.Eq(c => c.UserID, userId);
            var updateState = Builders<Users>.Update.Set(c => c.State, "");
            await usersCollection.UpdateOneAsync(filter, updateState);

            var deleteTaskUpdate = Builders<Users>.Update.PullFilter(c => c.Tasks, d => d.DoneDate == null);
            await usersCollection.UpdateOneAsync(filter, deleteTaskUpdate);
        }

        private static async Task ValidateUser(User messageFrom)
        {
            var usersCollection = mongoDatabase.GetCollection<Users>("Users");
            var user = (await usersCollection.FindAsync(c => c.UserID == messageFrom.Id.ToString())).SingleOrDefault();
            if (user == null)
            {
                await usersCollection.InsertOneAsync(new Users()
                {
                    Username = messageFrom.Username,
                    Name = messageFrom.FirstName,
                    LastName = messageFrom.LastName,
                    UserID = messageFrom.Id.ToString(),
                    State = ""
                });
            }
        }

        private static async Task CreateNewTask(long chatId, string userId)
        {
            await ShowNewTaskMessage(chatId);
            await MarkUserAsInNewMode(userId);
        }

        private static async Task ShowNewTaskMessage(long chatId)
        {
            await client.SendTextMessageAsync(chatId, "می خواهی چه کاری انجام بدی که من پیگیریت کنم ؟");
        }

        private static async Task MarkUserAsInNewMode(string userId)
        {
            var usersCollection = mongoDatabase.GetCollection<Users>("Users");
            await usersCollection.UpdateOneAsync(Builders<Users>.Filter.Eq(c => c.UserID, userId),
                Builders<Users>.Update.Set(c => c.State, "new"));
        }
        private static async Task MarkUserAsInDateMode(string userId)
        {
            var usersCollection = mongoDatabase.GetCollection<Users>("Users");
            var filter = Builders<Users>.Filter.Eq(c => c.UserID, userId);
            var updateCommand = Builders<Users>.Update.Set(c => c.State, "date");
            await usersCollection.UpdateOneAsync(filter, updateCommand);
        }

        private static async Task DeleteTask(long chatId, string userId, string messageText)
        {
            try
            {
                messageText = messageText.Replace("/del_", "").Replace("_", "-");
                var taskId = Guid.Parse(messageText);
                var usersCollection = mongoDatabase.GetCollection<Users>("Users");
                var filter = Builders<Users>.Filter;
                var userFilter = filter.And(
                     filter.Eq(c => c.UserID, userId),
                     filter.ElemMatch(c => c.Tasks, d => d.Id == taskId)
                 );
                var update = Builders<Users>.Update;
                var updateCommand = update.PullFilter(c => c.Tasks, d => d.Id == taskId);
                await usersCollection.UpdateOneAsync(userFilter, updateCommand);
                await client.SendTextMessageAsync(chatId, "تسک مورد نظر حذف شد.");
            }
            catch (Exception e)
            {
                await client.SendTextMessageAsync(chatId, "حذف انجام نشد");
            }
        }

        private static async void ShowActions(long chatId)
        {
            await client.SendTextMessageAsync(chatId, "/new تسک جدید");
            await client.SendTextMessageAsync(chatId, "/cancel برای کنسل کردن افزودن تسک جدید");
            await client.SendTextMessageAsync(chatId, "/mytasks برای نمای تسک های فعال شما");
            await client.SendTextMessageAsync(chatId, "/done_{taskid} برای اتمام تسک ");
            await client.SendTextMessageAsync(chatId, "/del_{taskid} برای حذف تسک ");
        }

        private static async Task ShowMyTasks(long chatId, string fromId)
        {
            var usersCollection = mongoDatabase.GetCollection<Users>("Users");
            var myTasks = (await usersCollection.FindAsync(Builders<Users>.Filter.Eq(c => c.UserID, fromId))).SingleOrDefault();
            if (myTasks.Tasks == null || myTasks.Tasks.All(c => c.IsDone))
                await client.SendTextMessageAsync(chatId, "شما هیچ تسک بازی ندارید");
            else
            {
                foreach (var myTask in myTasks.Tasks.ToList().Where(c => !c.IsDone))
                {
                    var taskCode = myTask.Id.ToString().Replace("-", "_");
                    var msg = "تسک : <strong>" + myTask.Message + "</strong>" + Environment.NewLine + "حذف تسک : /del_" + taskCode + Environment.NewLine + "انجام دادن /done_" + taskCode;
                    await client.SendTextMessageAsync(chatId, msg, ParseMode.Html);
                }
            }

        }
    }
}
