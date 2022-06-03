using System.Net;
using ERokytne.Application.Ports.Adapters.Fuel;
using ERokytne.Infrastructure.Adapters.FuelApi.Helpers;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace ERokytne.Infrastructure.Adapters.FuelApi;

public class FuelApiAdapter : IFuelAdapter
{
    public async Task<string> GetFuelInfo()
    {
        using var webClient = new WebClient();
        var webHtml = webClient.DownloadString(FuelUrlHelper.FuelListPage);
        var fileUrl = FuelParserHelper.ParseFuelData(webHtml);


        await using var documentStream = webClient.OpenRead(new Uri($"{FuelUrlHelper.BaseUrl}{fileUrl}"));
        var reader = new PdfReader(documentStream);
        var text = PdfTextExtractor.GetTextFromPage(reader, 1, new SimpleTextExtractionStrategy());

        return FuelParserHelper.ParseFuelResponse(text);
    }
}