using Microsoft.SemanticKernel;
using Swftx.CurrencyConverter;
using System.ComponentModel;
using Swftx.CurrencyConverter.Models.Primitives;

namespace BlazorChat.Services;

public class CurrencyService(ILogger<CurrencyService> logger, HttpClient client, IConfiguration configuration)
{
    private readonly ILogger<CurrencyService> logger = logger;
    private readonly HttpClient client = client;
    private readonly IConfiguration configuration = configuration;
    private readonly CurrecyCoverterClient ccc = new CurrecyCoverterClient(client);
    private const string fxRateAPIURL = @"https://api.fxratesapi.com/latest?base={fromCurrency}&symbols={toCurrency}&amount={amount}&format=JSON&api_key%3D={apiKey}";

    //[KernelFunction("convert_currency_value")]
    //[Description("Convert an amount from one currency to another")]
    //public async Task<string> ConvertAmount(Kernel kernel
    //   , [Description("The amount to convert")] string amount
    //   , [Description("The starting currency code")] string fromCurrency
    //   , [Description("The target currency code")] string toCurrency)
    //{
    //    bool isAmountValid = decimal.TryParse(amount, out decimal amountValue);
    //    if (!isAmountValid)
    //    {
    //        logger.LogInformation("Invalid amount value");
    //        return "Invalid amount value";
    //    }
    //    if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency))
    //    {
    //        logger.LogInformation("Invalid currency value");
    //        return "Invalid currency value";
    //    }
    //    Response<Swftx.CurrencyConverter.Models.CurrencyRates> conversionResult = await ccc.GetLatestRatesAsync(fromCurrency, new string[] { toCurrency });
    //    if (conversionResult.IsFailure)
    //    {
    //        logger.LogInformation("Conversion failed");
    //        return "Conversion failed";
    //    }
    //    decimal convertedValue = conversionResult.Value.Rates[toCurrency] * amountValue;
    //    string returnMsg = $@"{amount} {fromCurrency} is equal to {convertedValue} {toCurrency}";
    //    return returnMsg;
    //}
    //Better using FXRatesAPI Directly.
    [KernelFunction("convert_currency_value")]
    [Description("Convert an amount from one currency to another")]
    public async Task<string> ConvertAmount(Kernel kernel
        , [Description("The amount to convert")] string amount
        , [Description("The starting currency code")] string fromCurrency
        , [Description("Array of string of currency codes to convert to")] string[] toCurrency)
    {
        bool isAmountValid = decimal.TryParse(amount, out decimal amountValue);
        if (!isAmountValid)
        {
            logger.LogInformation("Invalid amount value");
            return "Invalid amount value";
        }
        if (string.IsNullOrEmpty(fromCurrency) || toCurrency.Length == 0)
        {
            logger.LogInformation("Invalid currency value");
            return "Invalid currency value";
        }
        string combinedCountires = string.Join(",", toCurrency);
        var urlToUse = new Uri(fxRateAPIURL
            .Replace("{fromCurrency}", fromCurrency)
            .Replace("{toCurrency}", combinedCountires)
            .Replace("{amount}", amount)
            .Replace("{apiKey}", configuration["FXRatesAPI"]));
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = urlToUse
        };
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return body;
    }    
}
