using System.Text;
using HtmlAgilityPack;

namespace ERokytne.Infrastructure.Adapters.FuelApi.Helpers;

public static class FuelParserHelper
{
    private static readonly List<string> FuelType = new()
    {
        "ДП готівка",
        "ДП талони картки",
        "А-95 готівка",
        "А-95 талони картки",
        "Газ продаж",
        "Газ талони картки"
    };

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

    public static string ParseFuelResponse(string response)
    {
        var fuelText = new StringBuilder();
        
        var fuelStationId = response.IndexOf("АВІАС", StringComparison.Ordinal);
        var fuelStations = response[fuelStationId..].Split(new[] { Environment.NewLine }, 
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var fuelStation in fuelStations)
        {
            var existIndex = fuelStation.Length - 11;

            var fuelStationName = fuelStation[..existIndex].Trim();
            fuelText.Append($"<b>{fuelStationName}</b>\n");
            
            var inStock = fuelStation[existIndex..].Split(' ');

            var counter = 0;
            foreach (var inStockInfo in inStock)
            {
                fuelText.Append($"{FuelType[counter]}: {inStockInfo}\n");
                counter++;
            }

            fuelText.Append(" \n");
        }
        
        return fuelText.ToString();
    }
}