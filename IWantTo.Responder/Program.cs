using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IWantTo.Responder.Bot;
using IWantTo.Responder.Entities;
using IWantTo.Responder.Model;
using Microsoft.Extensions.Configuration;
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

        public static Timer EveryNightTimer;

        static void Main(string[] args)
        {
            try
            {
                var config = new ConfigurationBuilder();
                config.AddEnvironmentVariables();
                var variables = config.Build().AsEnumerable();

                var rabbitMQ_URI = GetEnviromentValue(variables, "RABBITMQ_URI");
                var databaseConnectionString = GetEnviromentValue(variables, "DB_CSTR");
                var BotToken = GetEnviromentValue(variables, "BOTTOKEN");

                client = new TelegramBotClient(BotToken);
                                
                var ended = new ManualResetEventSlim();
                var starting = new ManualResetEventSlim();

                mongoClient = new MongoClient(databaseConnectionString);
                mongoDatabase = mongoClient.GetDatabase("IWantToDb");

                RunEveryNightJob();
               

                 var factory = new ConnectionFactory()
                {
                    Uri = new Uri(rabbitMQ_URI)
                };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += async (model, ea) =>
                       {
                           var body = ea.Body;
                           var str = Encoding.UTF8.GetString(body);
                           var message = JsonConvert.DeserializeObject<Update>(str);
                           var context = new Context(message, client, mongoDatabase);
                           var stateProcessor = new StateProcessor();
                           await stateProcessor.ProcessAsync(context);
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

        private static void RunEveryNightJob()
        {
            EveryNightTimer= new Timer(jobRunner, null, TimeSpan.Zero, TimeSpan.FromHours(12));
        }

        private static void jobRunner(object state)
        {
            var notificationSender = new NotificationSender(client, mongoDatabase) ;
            notificationSender.SendGlobalNotifications();
            notificationSender.SendPrivateNotifications();
        }

        private static string GetEnviromentValue(IEnumerable<KeyValuePair<string, string>> variables, string key)
        {
            var value = "";
            if (variables.Any(c => c.Key == key))
            {
                value = variables.First(c => c.Key == key).Value;
            }
            return value;
        }
    }
}
