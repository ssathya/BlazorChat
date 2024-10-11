using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Models.AppModels;
using Models.DBModels;
using System.ComponentModel;
using System.Text.Json;

namespace BlazorChat.Services;

public class StockServicePlugin(ILogger<StockServicePlugin> logger, IConfiguration configuration
    , HttpClient httpClient
    , IDbContextFactory<BlazorChatContext> contextFactory)
{
    private const string FinnhubIoQuoteUrl = "https://finnhub.io/api/v1/quote?symbol={ticker}&token={apiKey}";
    private readonly IConfiguration configuration = configuration;
    private readonly IDbContextFactory<BlazorChatContext> contextFactory = contextFactory;
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<StockServicePlugin> logger = logger;

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

    [KernelFunction("list_stocks_by_performance")]
    [Description("Get a list of the top performing stocks for a given period")]
    [return: Description("List of the top 20 performing stocks for the given period")]
    public async Task<string> ListStocksByPerformance(Kernel kernel,
        [Description("Period to get the top performing stocks for. " +
        "Valid values are: m for montly, q for quarterly, h for half-yearly, y for yearly")] string period,
        [Description("True to get the best performing stocks, false to get the worst performing stocks")] bool bestOrWorse)
    {
        const int maxStocksToReturn = 20;
        Period periodToUse = period.ToLower().Trim() switch
        {
            "y" => Period.Yearly,
            "h" => Period.HalfYearly,
            "q" => Period.Quarterly,
            "m" => Period.Monthly,
            _ => Period.Monthly,
        };
        using BlazorChatContext context = await contextFactory.CreateDbContextAsync();
        List<TickerSlope> requestedResult = bestOrWorse
            ? await context.TickerSlopes.Where(r => r.Period == periodToUse)
            .OrderByDescending(r => r.SlopeResults.OrderBy(x => x.Date).Last().Slope)
            .Take(maxStocksToReturn)
            .ToListAsync()
            : await context.TickerSlopes.Where(r => r.Period == Period.Monthly)
            .OrderBy(r => r.SlopeResults.OrderBy(x => x.Date).Last().Slope)
            .Take(maxStocksToReturn)
            .AsNoTracking()
            .ToListAsync();
        List<string> tickers = requestedResult.Select(r => r.Ticker).ToList();
        List<IndexComponent> indexComponents = await context.IndexComponents
            .Where(r => tickers.Contains(r.Ticker))
            .AsNoTracking()
            .ToListAsync();
        List<TickerSlopeView> tickerSlopeViews = [];
        foreach (TickerSlope tickerSlope in requestedResult)
        {
            tickerSlopeViews.Add(tickerSlope);
            IndexComponent? indexComponent = indexComponents.FirstOrDefault(r => r.Ticker == tickerSlope.Ticker);
            if (indexComponent is not null)
            {
                tickerSlopeViews.Last().CompanyName = indexComponent.CompanyName;
                tickerSlopeViews.Last().Sector = indexComponent.Sector;
                tickerSlopeViews.Last().SnPWeight = indexComponent.SnPWeight;
                tickerSlopeViews.Last().NasdaqWeight = indexComponent.NasdaqWeight;
                tickerSlopeViews.Last().DowWeight = indexComponent.DowWeight;
                tickerSlopeViews.Last().ListedIndexes = indexComponent.ListedIndexes;
            }
        }
        var returnMsg = JsonSerializer.Serialize(tickerSlopeViews, new JsonSerializerOptions { WriteIndented = true });
        return returnMsg;
    }
}