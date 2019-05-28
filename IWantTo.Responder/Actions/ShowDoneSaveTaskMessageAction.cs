using IWantTo.Responder.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IWantTo.Responder.Actions
{
    public class ShowDoneSaveTaskMessageAction : IAction
    {
        public async Task ExecuteAsync(Context context)
        {
            var chatId = context.Update.Message.Chat.Id;
            await context.Client.SendTextMessageAsync(chatId, "حله.حالا از این به بعد من می دونم با تو.باید کارتو تموم کنی.یادت نره که من بدجور آبرو برم.اگر می‌خوای لیست کاراتو ببینی می‌تونی از دستور /mytasks استفاده کنی.اگر دوست داری ببینی کجا قراره برم جار بزنم می تونی لیست گروه هایی که از تو میشناسم رو با این دستور ببینی : /mygroups");
        }
    }
}
