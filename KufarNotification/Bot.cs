using Telegram.Bot;
namespace KufarNotification;

public class Bot
{
    public static readonly string token = Environment.GetEnvironmentVariable("BOT_TOKEN");
    
    private readonly TelegramBotClient _botClient;
    
    public Bot()
    {
        _botClient = new TelegramBotClient(token);
    } 
}