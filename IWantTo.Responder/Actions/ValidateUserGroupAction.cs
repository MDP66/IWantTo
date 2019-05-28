using IWantTo.Responder.Entities;
using IWantTo.Responder.Model;

using MongoDB.Driver;
using System;
using System.Threading.Tasks;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IWantTo.Responder.Actions
{
    public class ValidateUserGroupAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var chat = context.Update.Message.Chat;
            var user = context.Update.Message.From;
            var usersCollection = context.Db.GetCollection<Entities.User>("Users");
            var filter = Builders<Entities.User>.Filter;
            var userFilter = filter.Eq(c => c.UserID, context.User.UserID.ToString());

            if (chat.Type != ChatType.Group && chat.Type != ChatType.Supergroup) return;

            var userIsNotJoindThisGroupYet = await IsUserJoinedThisGroupBeforeAsync(chat, user, filter, usersCollection);
            if (!userIsNotJoindThisGroupYet)
            {
                await AddGroupToUsersGroup(chat, usersCollection, userFilter);
            }
        }

        private async Task AddGroupToUsersGroup(Chat chat, IMongoCollection<Entities.User> usersCollection, FilterDefinition<Entities.User> userFilter)
        {
            var group = new Group()
            {
                Name = chat.Title,
                ChatId = chat.Id,
                CustomId = Guid.NewGuid().ToString(),
                SendNotification = true
            };

            var update = Builders<Entities.User>.Update;
            var updateCommand = update.Push(c => c.Groups, group);

            await usersCollection.UpdateOneAsync(userFilter, updateCommand);
        }

        private async Task<bool> IsUserJoinedThisGroupBeforeAsync(Chat chat, Telegram.Bot.Types.User user, FilterDefinitionBuilder<Entities.User> filter, IMongoCollection<Entities.User> usersCollection)
        {
            var existenceFilter = filter.ElemMatch(c => c.Groups, d => d.ChatId == chat.Id);
            var userGroup = await usersCollection.FindAsync(existenceFilter);

            return userGroup.SingleOrDefault() != null;
        }
    }
}
