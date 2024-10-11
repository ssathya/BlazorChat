namespace Models.AppModels;

public class WeatherForCity
{
    public string CityLocation { get; set; } = string.Empty;
    public double CurrentTemperature { get; set; }
    public string CurrentCondition { get; set; } = string.Empty;
    public List<DayCondition> DayConditions { get; set; } = [];
}

public class DayCondition
{
    public DateTime ReportingDate { get; set; }
    public double MaxTemp { get; set; }
    public double MinTemp { get; set; }
    public int Humidity { get; set; }
    public bool WillItRain { get; set; }
    public bool WillItSnow { get; set; }
    public double TotalRainfallDay { get; set; }
    public double TotalSnowfallDay { get; set; }
    public string Condition { get; set; } = string.Empty;
}