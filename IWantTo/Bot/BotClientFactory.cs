using Telegram.Bot;

namespace IWantTo.Bot
{
    public class BotClientFactory
    {
        public static TelegramBotClient CreateClient()
        {
            //KISS
            return  new TelegramBotClient("772431272:AAEIYr8rKdmJ33WzJ2YSfly_Vwk8C9M8NfM");
        }
    }
}
