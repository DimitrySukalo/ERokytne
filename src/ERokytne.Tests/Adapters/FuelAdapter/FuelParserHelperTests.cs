using System;
using System.Net;
using System.Threading.Tasks;
using ERokytne.Infrastructure.Adapters.FuelApi.Helpers;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Xunit;

namespace ERokytne.Tests.Adapters.FuelAdapter;

public static class FuelParserHelperTests
{
    [Fact]
    private static void ParseFuelDataReturnsExceptions()
    {
        //Arrange
        var data = string.Empty;

        //Act and Assert
        Assert.ThrowsAny<Exception>(() => FuelParserHelper.ParseFuelData(data));
    }
    
    [Fact]
    private static void ParseFuelDataReturnsFileUrl()
    {
        //Arrange
        using var webClient = new WebClient();
        var webHtml = webClient.DownloadString(FuelUrlHelper.FuelListPage);

        //Act
        var fileUrl = FuelParserHelper.ParseFuelData(webHtml);
        
        //Assert
        Assert.NotNull(fileUrl);
    }
        
        
    [Fact]
    private static void ParseFuelResponseReturnsExceptions()
    {
        //Arrange
        var data = string.Empty;

        //Act and Assert
        Assert.ThrowsAny<Exception>(() => FuelParserHelper.ParseFuelResponse(data));
    }
    
    [Fact]
    private static async Task ParseFuelResponseReturnsTextData()
    {
        //Arrange
        using var webClient = new WebClient();
        var webHtml = webClient.DownloadString(FuelUrlHelper.FuelListPage);
        var fileUrl = FuelParserHelper.ParseFuelData(webHtml);


        await using var documentStream = webClient.OpenRead(new Uri($"{FuelUrlHelper.BaseUrl}{fileUrl}"));
        var reader = new PdfReader(documentStream);
        var text = PdfTextExtractor.GetTextFromPage(reader, 1, new SimpleTextExtractionStrategy());

        //Act
        var textData = FuelParserHelper.ParseFuelResponse(text);
        
        //Assert
        Assert.NotNull(textData);
    }
}