using KufarNotification;
using Telegram.Bot;

class Program
{
    static void Main(string[] args)
    {
        var bot = new TelegramBotClient(Bot.token);
        AdvertisementManager.Update(bot);
    }
}