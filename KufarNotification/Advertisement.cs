using Telegram.Bot.Types;

namespace KufarNotification;

public class Advertisement
{
    public string Name { get; }
    public string Price { get; }
    public string Location { get; }
    public string Link { get; }

    public Advertisement(string name, string price, string location, string link)
    {
        Name = name;
        Price = price;
        Location = location;
        Link = link;
    }
}