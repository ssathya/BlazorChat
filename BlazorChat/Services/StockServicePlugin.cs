using ClosedXML.Excel;
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
    #region Private Fields

    private const string FinnHubIoQuoteUrl = "https://finnhub.io/api/v1/quote?symbol={ticker}&token={apiKey}";
    private readonly IConfiguration configuration = configuration;
    private readonly IDbContextFactory<BlazorChatContext> contextFactory = contextFactory;
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<StockServicePlugin> logger = logger;

    #endregion Private Fields

    #region Public Methods

    [KernelFunction("list_best_performing_stocks")]
    [Description("Get a list of investment worthy stocks")]
    [return: Description("List 10 stocks selected by recursive computation")]
    public string GetBestPerformingStocks(Kernel kernel)
    {
        List<FileInfo> files = GetListOfExcelFiles();
        if (files.Count == 0)
        {
            return "System error";
        }
        files = files.Where(f => f.FullName.Contains(@"RecursiveSelection.xlsx")).ToList();
        string sheetToUse = "RecursiveSelection";
        ReadGeneratedExcelFile(files, sheetToUse, out XLWorkbook workBook,
            out List<StockExcel> stockExcels);
        var returnMsg = JsonSerializer.Serialize(stockExcels, new JsonSerializerOptions { WriteIndented = true });
        return returnMsg;
    }

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
        var urlToUse = FinnHubIoQuoteUrl.Replace("{apiKey}", apiKey).Replace("{ticker}", ticker);
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
    public string ListStocksByPerformance(Kernel kernel,
       [Description("Period to get the top performing stocks for. " +
        "Valid values are: m for monthly, q for quarterly, h for half-yearly, y for yearly")] string period,
       [Description("True to get the best performing stocks, false to get the worst performing stocks")] bool bestOrWorse)
    {
        List<FileInfo> files = GetListOfExcelFiles();
        if (files.Count == 0)
        {
            return "System error";
        }
        foreach (FileInfo file in files)
        {
            if (file.FullName.Contains(@"RecursiveSelection.xlsx"))
            {
                files.Remove(file);
                break;
            }
        }
        string periodToUse = period.ToLower().Trim() switch
        {
            "y" => "Yearly",
            "h" => "HalfYearly",
            "q" => "Quarterly",
            "m" => "Monthly",
            _ => "Monthly",
        };
        ReadGeneratedExcelFile(files, periodToUse, out XLWorkbook workBook,
            out List<StockExcel> stockExcels);
        int maxStocksToReturn = 20;
        stockExcels = bestOrWorse
            ? stockExcels.OrderByDescending(r => r.EndingSlope)
                .Take(maxStocksToReturn)
                .ToList()
            : stockExcels.OrderBy(r => r.EndingSlope).ToList()
                .Take(maxStocksToReturn)
                .ToList();
        var returnMsg = JsonSerializer.Serialize(stockExcels, new JsonSerializerOptions { WriteIndented = true });
        return returnMsg;
    }

    #endregion Public Methods

    #region Private Methods

    private static List<FileInfo> GetListOfExcelFiles()
    {
        var files = new List<FileInfo>();
        string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string excelDirectory = Path.Combine(homeFolder, "excelDirectory");
        if (!Directory.Exists(excelDirectory))
        {
            return [];
        }
        DirectoryInfo directoryInfo = new(excelDirectory);
        files = directoryInfo.GetFiles("*.xlsx")
            .OrderByDescending(f => f.CreationTime)
            .ToList();
        return files;
    }

    private static void ReadGeneratedExcelFile(List<FileInfo> files, string sheetToUse,
            out XLWorkbook workBook, out List<StockExcel> stockExcels)
    {
        string fileNameToUse = files.First().FullName;
        workBook = new(fileNameToUse);
        IXLWorksheet workSheet = workBook.Worksheet(sheetToUse);
        IXLRangeRows rows = workSheet.RangeUsed()!.RowsUsed();

        stockExcels = [];
        foreach (var row in rows.Skip(1))
        {
            stockExcels.Add(new StockExcel()
            {
                Ticker = row.Cell(1).Value.ToString()!,
                CompanyName = row.Cell(2).Value.ToString()!,
                //Period = row.Cell(3).Value.ToString(),
                StartingSlope = row.Cell(4).GetValue<double>(),
                EndingSlope = row.Cell(5).GetValue<double>()
            });
        }
    }

    #endregion Private Methods
}