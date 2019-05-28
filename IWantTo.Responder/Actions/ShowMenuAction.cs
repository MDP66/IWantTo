using IWantTo.Responder.Model;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class ShowMenuAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var chatId = context.Update.Message.Chat.Id;
            await context.Client.SendTextMessageAsync(chatId, "/new تسک جدید");
            await context.Client.SendTextMessageAsync(chatId, "/cancel برای کنسل کردن افزودن تسک جدید");
            await context.Client.SendTextMessageAsync(chatId, "/mytasks برای نمای تسک های فعال شما");
            await context.Client.SendTextMessageAsync(chatId, "/mygroups گروه هایی که در آن برای تشویق شما اطلاعیه ارسال می شود");
            await context.Client.SendTextMessageAsync(chatId, "/done_{taskid} برای اتمام تسک ");
            await context.Client.SendTextMessageAsync(chatId, "/del_{taskid} برای حذف تسک ");
        }
    }
}
