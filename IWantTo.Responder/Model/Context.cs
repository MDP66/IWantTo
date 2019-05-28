using MongoDB.Driver;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace IWantTo.Responder.Model
{
    public class Context
    {
        public Context(Update update, TelegramBotClient client, IMongoDatabase db)
        {
            Update = update;
            Client = client;
            Db = db;
        }
        /// <summary>
        /// Message sended by Telegram webhook
        /// </summary>
        public Update Update { get; }

        /// <summary>
        /// User info from Database
        /// </summary>
        public Entities.User User { get; private set; }

        /// <summary>
        /// Telegram bot client ( for sending message and etc ... )
        /// </summary>
        public TelegramBotClient Client { get; }

        /// <summary>
        /// MongoDB database object
        /// </summary>
        public IMongoDatabase Db { get; }

        /// <summary>
        /// Extra arguments to execute
        /// </summary>
        public object Args { get; private set; }
        /// <summary>
        /// Set user to base object
        /// </summary>
        /// <param name="user">User</param>
        public void SetUser(Entities.User user)
        {
            if (User == null)
            {
                User = user;
            }
        }

        public void SetArgs(object args)
        {
            Args = args;
        }
    }
}
