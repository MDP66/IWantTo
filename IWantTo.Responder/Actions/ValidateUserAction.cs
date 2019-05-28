using IWantTo.Responder.Entities;
using IWantTo.Responder.Model;

using MongoDB.Driver;

using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class ValidateUserAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var usersCollection = context.Db.GetCollection<Entities.User>("Users");
            var messageFrom = context.Update.Message.From;
            var userId = messageFrom.Id.ToString();

            var user = await RetriveUser(usersCollection, userId);
            if (user == null)
            {
                user = await InsertNewUserAsync(usersCollection, messageFrom);
            }

            context.SetUser(user);
        }

        private async Task<User> InsertNewUserAsync(IMongoCollection<User> usersCollection, Telegram.Bot.Types.User messageFrom)
        {
            var user = new User()
            {
                Username = messageFrom.Username,
                Name = messageFrom.FirstName,
                LastName = messageFrom.LastName,
                UserID = messageFrom.Id.ToString(),
                State = TaskStateTypes.Empty.ToString()
            };
            await usersCollection.InsertOneAsync(user);
            return user;
        }

        private async Task<Entities.User> RetriveUser(IMongoCollection<User> usersCollection, string userId)
        {
            var user = await usersCollection.FindAsync(c => c.UserID == userId);
            return user.FirstOrDefault();
        }
    }
}
