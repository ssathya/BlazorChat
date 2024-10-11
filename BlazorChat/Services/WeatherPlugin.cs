using Microsoft.SemanticKernel;
using Models.AppModels;
using System.ComponentModel;
using System.Text.Json;

namespace BlazorChat.Services;

public class WeatherPlugin(ILogger<WeatherPlugin> logger, IConfiguration configuration, HttpClient httpClient)
{
    private readonly ILogger<WeatherPlugin> logger = logger;
    private readonly IConfiguration configuration = configuration;
    private readonly HttpClient httpClient = httpClient;
    private const string WeatherApiUrl = "https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={City}&days=3&aqi=yes&alerts=yes";

    [KernelFunction("get_weather")]
    [Description("Get the current weather in a given location")]
    [return: Description("The current weather")]
    public async Task<string> GetWeather(Kernel kernel, string city)
    {
        var apiKey = configuration["WeatherApiKey"];
        var urlToUse = WeatherApiUrl.Replace("{apiKey}", apiKey).Replace("{City}", city);
        HttpResponseMessage responseMessage = await httpClient.GetAsync(urlToUse);
        if (!responseMessage.IsSuccessStatusCode)
        {
            logger.LogError("Error getting weather for {City}: {Error}", city, responseMessage.ReasonPhrase);
            return "Error fetching weather";
        }
        Weather? weather = await responseMessage.Content.ReadFromJsonAsync<Weather>();
        if (weather == null)
        {
            logger.LogError("Error deserializing weather for {City}", city);
            return "Error fetching weather";
        }
        WeatherForCity weatherForCity = new WeatherForCity
        {
            CityLocation = $"{weather.Location.Name}, {weather.Location.Region} {weather.Location.Country}",
            CurrentTemperature = weather.Current.TempF,
            CurrentCondition = weather.Current.Condition.Text,
            DayConditions = BuildDayConationsLst(weather)
        };
        var returnMsg = JsonSerializer.Serialize(weatherForCity, new JsonSerializerOptions { WriteIndented = true });
        return returnMsg;
    }

    [KernelFunction("city_help_for_weather")]
    [Description("Help message to provide city name")]
    public string GetWeatherCityHelp(Kernel kernel)
    {
        return "To get weather information provide either City,State,Country or Zip Code,Country; Country is optional but " +
            "yields better result";
    }

    private static List<DayCondition> BuildDayConationsLst(Weather weather)
    {
        List<DayCondition> dayConditions = new List<DayCondition>();
        foreach (var forecast in weather.Forecast.Forecastday)
        {
            dayConditions.Add(new DayCondition
            {
                Condition = forecast.Day.Condition.Text,
                Humidity = forecast.Day.Avghumidity,
                MaxTemp = forecast.Day.MaxtempF,
                MinTemp = forecast.Day.MintempF,
                ReportingDate = (DateTimeOffset.FromUnixTimeSeconds(forecast.DateEpoch)).DateTime,
                TotalRainfallDay = forecast.Day.TotalprecipIn,
                TotalSnowfallDay = forecast.Day.TotalsnowCm * 0.393701,
                WillItRain = forecast.Day.DailyWillItRain != 0,
                WillItSnow = forecast.Day.DailyWillItSnow != 0
            });
        }
        return dayConditions;
    }
}