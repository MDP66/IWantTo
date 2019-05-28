using IWantTo.Responder.Model;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class SetTaskDateMessageAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var chatId = context.Update.Message.Chat.Id;
            await context.Client.SendTextMessageAsync(chatId, "حالا کی تموم میشه این کارت؟ فقط جون هرکی دوست داری درست بنویس تاریخ رو.");
            await context.Client.SendTextMessageAsync(chatId, "مثل این : 1398/01/13");
        }
    }
}
