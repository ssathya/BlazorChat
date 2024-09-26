using BlazorChat.Models;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace BlazorChat.Services;

public class StockServicePlugin(ILogger<StockServicePlugin> logger, IConfiguration configuration, HttpClient httpClient)
{
    private readonly ILogger<StockServicePlugin> logger = logger;
    private readonly IConfiguration configuration = configuration;
    private readonly HttpClient httpClient = httpClient;

    private const string FinnhubIoQuoteUrl = "https://finnhub.io/api/v1/quote?symbol={ticker}&token={apiKey}";

    [KernelFunction("get_stock_quote")]
    [Description("Get the latest stock quote for a given ticker")]
    [return: Description("Current trading price for the given ticker")]
    public async Task<string> GetLatestStockPrice(Kernel kernel, string ticker)

    {
        var apiKey = configuration["FinnHub"];
        if (apiKey == null)
        {
            return "Could not find api key for Quotes provider";
        }
        var urlToUse = FinnhubIoQuoteUrl.Replace("{apiKey}", apiKey).Replace("{ticker}", ticker);
        HttpResponseMessage responseMessage = await httpClient.GetAsync(urlToUse);
        if (!responseMessage.IsSuccessStatusCode)
        {
            logger.LogError("Error getting quotes for ticker {ticker}: {Error}", ticker, responseMessage.ReasonPhrase);
            return "Error getting quotes";
        }
        Quote? quote = await responseMessage.Content.ReadFromJsonAsync<Quote>();
        if (quote == null)
        {
            logger.LogError("Error deserializing quotes for {ticker}", ticker);
            return "Error parsing values from vendor";
        }
        QuotesVM quotesVM = quote;
        quotesVM.Ticker = ticker;
        var returnMsg = JsonSerializer.Serialize(quotesVM, new JsonSerializerOptions { WriteIndented = true });
        return returnMsg;
    }
}