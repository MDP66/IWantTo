using IWantTo.Responder.Model;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class StartNewTaskAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var chatId = context.Update.Message.Chat.Id;
            await context.Client.SendTextMessageAsync(chatId, "می خواهی چه کاری انجام بدی که من پیگیریت کنم ؟");            
        }
    }
}
