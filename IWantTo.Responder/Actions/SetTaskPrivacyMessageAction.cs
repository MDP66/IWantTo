using IWantTo.Responder.Model;

using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class SetTaskPrivacyMessageAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var chatId = context.Update.Message.Chat.Id;
            await context.Client.SendTextMessageAsync(chatId, "در جریانی که؟ من آبرو برم.توی تمام گروه هایی که عضو باشی و منم باشم میام هر شب کاراتو جار می زنم.");
            await context.Client.SendTextMessageAsync(chatId, "حالا این کارت خصوصی که نیست ؟ اگر هست میتونی بنویس : 'بلی'");
            await context.Client.SendTextMessageAsync(chatId, "اگر خصوصی باشه فقط هر شب برای خودت می فرستمشون.نگران نباش.");
        }
    }
}
