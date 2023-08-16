using System.Web;
using HtmlAgilityPack;
using Telegram.Bot;

namespace KufarNotification;

public static class AdvertisementManager
{
    static List<Advertisement> _collection = GetCollection();
    const long ChatId = -1001959580486; //ID вашего чата


    public static void Update(TelegramBotClient bot)
    {
        while (true)
        {
            var difference = CompareCollections();

            if (difference.ToList().Count() != 0)
            {
                bot.SendTextMessageAsync(ChatId, $"Новых объявлений: {difference.Count()}");

                foreach (var advertisement in difference)
                {
                    bot.SendTextMessageAsync(ChatId, $"{advertisement.Name}\n" +
                                                         $"{advertisement.Price}\n" +
                                                         $"{advertisement.Location}\n" +
                                                         $"{advertisement.Link}");
                }
            }

            Thread.Sleep(TimeSpan.FromMinutes(1));
        }
    }

    public static void Print()
    {
        foreach (var advertisement in _collection)
        {
            Console.WriteLine(advertisement.Name);
            Console.WriteLine(advertisement.Link);
        }
    }

    private static List<Advertisement> GetCollection()
    {
        var collection = new List<Advertisement>();
        var web = new HtmlWeb();
        var doc = web.Load("https://www.kufar.by/l/monitory");

        var nodes = doc.DocumentNode.SelectNodes("//a[@class='styles_wrapper__5FoK7']");


        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                var name = node.SelectSingleNode("./div[2]/h3").InnerHtml;
                name = HttpUtility.HtmlDecode(name);
                var price = node.SelectSingleNode("./div[2]/div[1]/div[1]").InnerText;
                var link = node.Attributes["href"].Value;
                var location = node.SelectSingleNode("./div[2]/div[2]/p").InnerText;

                collection.Add(new Advertisement(name, price, location, link));
            }
        }
        else
        {
            Console.WriteLine("Заголовки не найдены. \n " +
                              "Проверьте правильность пути xpath: '//a[@class='имя класса']' ");
        }

        return collection;
    }

    private static List<Advertisement> CompareCollections()
    {
        var updatedCollection = GetCollection();

        var difference = updatedCollection.ExceptBy(_collection.Select
                (a => a.Name),
            (b => b.Name)).ToList();


        _collection = _collection.Union(difference).ToList();

        return difference;
    }
    private static async IAsyncEnumerable<Advertisement> GetCollectionAsync()
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://www.kufar.by/l/r~minsk/monitory?cmp=0&prc=r%3A10000%2C35000&sort=lst.d");
        using var content = await response.Content.ReadAsStreamAsync();

        var doc = new HtmlDocument();
        doc.Load(content);

        var nodes = doc.DocumentNode.Descendants("a")
            .Where(a => a.GetAttributeValue("class", "") == "styles_wrapper__yaLfq");

        foreach (var node in nodes)
        {
            var name = HttpUtility.HtmlDecode(node.Descendants("h3").FirstOrDefault()?.InnerText);
            var price = node.Descendants("div").FirstOrDefault(d => d.GetAttributeValue("class", "") == "price")
                ?.Descendants("div").FirstOrDefault(d => d.GetAttributeValue("class", "") == "price--converted")
                ?.InnerText;
            var link = node.GetAttributeValue("href", "");
            var location = node.Descendants("p").FirstOrDefault()?.InnerText;

            yield return new Advertisement(name, price, location, link);
        }
    }
}