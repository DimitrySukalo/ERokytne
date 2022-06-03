using HtmlAgilityPack;

namespace ERokytne.Infrastructure.Adapters.Helpers;

public static class FuelParserHelper
{
    public static string ParseFuelData(string data)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(data);

        var dataNodes = htmlDocument.DocumentNode
            .SelectNodes("//div").FirstOrDefault(e =>
                e.Attributes.FirstOrDefault(a => a.Name == "class")?.Value ==
                "news__item--inner editor-content")?
            .ChildNodes.Where(e => e.Name == "p").FirstOrDefault(e => e.InnerText.Equals("Сарненський район"))?
            .ChildNodes.FirstOrDefault(e => e.Attributes.FirstOrDefault(a => a.Name == "class")?.Value == "fr-file")
            ?? throw new ArgumentNullException("Fuel node is not found");

        var fileUrl = dataNodes.Attributes.FirstOrDefault(e => e.Name == "href")?.Value;
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            throw new ArgumentNullException($"File url is empty: {fileUrl}");
        }
        
        return fileUrl;
    }
}