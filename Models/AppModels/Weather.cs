using System.Text.Json.Serialization;

namespace Models.AppModels;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class AirQuality
{
    [JsonPropertyName("co")]
    public double Co { get; set; }

    [JsonPropertyName("no2")]
    public double No2 { get; set; }

    [JsonPropertyName("o3")]
    public double O3 { get; set; }

    [JsonPropertyName("so2")]
    public double So2 { get; set; }

    [JsonPropertyName("pm2_5")]
    public double Pm25 { get; set; }

    [JsonPropertyName("pm10")]
    public double Pm10 { get; set; }

    [JsonPropertyName("us-epa-index")]
    public int UsEpaIndex { get; set; }

    [JsonPropertyName("gb-defra-index")]
    public int GbDefraIndex { get; set; }
}

public class Alerts
{
    [JsonPropertyName("alert")]
    public List<object> Alert { get; set; } = [];
}

public class Astro
{
    [JsonPropertyName("sunrise")]
    public string Sunrise { get; set; } = string.Empty;

    [JsonPropertyName("sunset")]
    public string Sunset { get; set; } = string.Empty;

    [JsonPropertyName("moonrise")]
    public string Moonrise { get; set; } = string.Empty;

    [JsonPropertyName("moonset")]
    public string Moonset { get; set; } = string.Empty;

    [JsonPropertyName("moon_phase")]
    public string MoonPhase { get; set; } = string.Empty;

    [JsonPropertyName("moon_illumination")]
    public int MoonIllumination { get; set; }

    [JsonPropertyName("is_moon_up")]
    public int IsMoonUp { get; set; }

    [JsonPropertyName("is_sun_up")]
    public int IsSunUp { get; set; }
}

public class Condition
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }
}

public class Current
{
    [JsonPropertyName("last_updated_epoch")]
    public int LastUpdatedEpoch { get; set; }

    [JsonPropertyName("last_updated")]
    public string LastUpdated { get; set; } = string.Empty;

    [JsonPropertyName("temp_c")]
    public double TempC { get; set; }

    [JsonPropertyName("temp_f")]
    public double TempF { get; set; }

    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }

    [JsonPropertyName("condition")]
    public Condition Condition { get; set; } = new();

    [JsonPropertyName("wind_mph")]
    public double WindMph { get; set; }

    [JsonPropertyName("wind_kph")]
    public double WindKph { get; set; }

    [JsonPropertyName("wind_degree")]
    public int WindDegree { get; set; }

    [JsonPropertyName("wind_dir")]
    public string WindDir { get; set; } = string.Empty;

    [JsonPropertyName("pressure_mb")]
    public double PressureMb { get; set; }

    [JsonPropertyName("pressure_in")]
    public double PressureIn { get; set; }

    [JsonPropertyName("precip_mm")]
    public double PrecipMm { get; set; }

    [JsonPropertyName("precip_in")]
    public double PrecipIn { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("cloud")]
    public int Cloud { get; set; }

    [JsonPropertyName("feelslike_c")]
    public double FeelslikeC { get; set; }

    [JsonPropertyName("feelslike_f")]
    public double FeelslikeF { get; set; }

    [JsonPropertyName("windchill_c")]
    public double WindchillC { get; set; }

    [JsonPropertyName("windchill_f")]
    public double WindchillF { get; set; }

    [JsonPropertyName("heatindex_c")]
    public double HeatindexC { get; set; }

    [JsonPropertyName("heatindex_f")]
    public double HeatindexF { get; set; }

    [JsonPropertyName("dewpoint_c")]
    public double DewpointC { get; set; }

    [JsonPropertyName("dewpoint_f")]
    public double DewpointF { get; set; }

    [JsonPropertyName("vis_km")]
    public double VisKm { get; set; }

    [JsonPropertyName("vis_miles")]
    public double VisMiles { get; set; }

    [JsonPropertyName("uv")]
    public double Uv { get; set; }

    [JsonPropertyName("gust_mph")]
    public double GustMph { get; set; }

    [JsonPropertyName("gust_kph")]
    public double GustKph { get; set; }

    [JsonPropertyName("air_quality")]
    public AirQuality AirQuality { get; set; } = new();
}

public class Day
{
    [JsonPropertyName("maxtemp_c")]
    public double MaxtempC { get; set; }

    [JsonPropertyName("maxtemp_f")]
    public double MaxtempF { get; set; }

    [JsonPropertyName("mintemp_c")]
    public double MintempC { get; set; }

    [JsonPropertyName("mintemp_f")]
    public double MintempF { get; set; }

    [JsonPropertyName("avgtemp_c")]
    public double AvgtempC { get; set; }

    [JsonPropertyName("avgtemp_f")]
    public double AvgtempF { get; set; }

    [JsonPropertyName("maxwind_mph")]
    public double MaxwindMph { get; set; }

    [JsonPropertyName("maxwind_kph")]
    public double MaxwindKph { get; set; }

    [JsonPropertyName("totalprecip_mm")]
    public double TotalprecipMm { get; set; }

    [JsonPropertyName("totalprecip_in")]
    public double TotalprecipIn { get; set; }

    [JsonPropertyName("totalsnow_cm")]
    public double TotalsnowCm { get; set; }

    [JsonPropertyName("avgvis_km")]
    public double AvgvisKm { get; set; }

    [JsonPropertyName("avgvis_miles")]
    public double AvgvisMiles { get; set; }

    [JsonPropertyName("avghumidity")]
    public int Avghumidity { get; set; }

    [JsonPropertyName("daily_will_it_rain")]
    public int DailyWillItRain { get; set; }

    [JsonPropertyName("daily_chance_of_rain")]
    public int DailyChanceOfRain { get; set; }

    [JsonPropertyName("daily_will_it_snow")]
    public int DailyWillItSnow { get; set; }

    [JsonPropertyName("daily_chance_of_snow")]
    public int DailyChanceOfSnow { get; set; }

    [JsonPropertyName("condition")]
    public Condition Condition { get; set; } = new();

    [JsonPropertyName("uv")]
    public double Uv { get; set; }
}

public class Forecast
{
    [JsonPropertyName("forecastday")]
    public List<Forecastday> Forecastday { get; set; } = new();
}

public class Forecastday
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("date_epoch")]
    public int DateEpoch { get; set; }

    [JsonPropertyName("day")]
    public Day Day { get; set; } = new();

    [JsonPropertyName("astro")]
    public Astro Astro { get; set; } = new();

    [JsonPropertyName("hour")]
    public List<Hour> Hour { get; set; } = [];
}

public class Hour
{
    [JsonPropertyName("time_epoch")]
    public int TimeEpoch { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("temp_c")]
    public double TempC { get; set; }

    [JsonPropertyName("temp_f")]
    public double TempF { get; set; }

    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }

    [JsonPropertyName("condition")]
    public Condition Condition { get; set; } = new();

    [JsonPropertyName("wind_mph")]
    public double WindMph { get; set; }

    [JsonPropertyName("wind_kph")]
    public double WindKph { get; set; }

    [JsonPropertyName("wind_degree")]
    public int WindDegree { get; set; }

    [JsonPropertyName("wind_dir")]
    public string WindDir { get; set; } = string.Empty;

    [JsonPropertyName("pressure_mb")]
    public double PressureMb { get; set; }

    [JsonPropertyName("pressure_in")]
    public double PressureIn { get; set; }

    [JsonPropertyName("precip_mm")]
    public double PrecipMm { get; set; }

    [JsonPropertyName("precip_in")]
    public double PrecipIn { get; set; }

    [JsonPropertyName("snow_cm")]
    public double SnowCm { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("cloud")]
    public int Cloud { get; set; }

    [JsonPropertyName("feelslike_c")]
    public double FeelslikeC { get; set; }

    [JsonPropertyName("feelslike_f")]
    public double FeelslikeF { get; set; }

    [JsonPropertyName("windchill_c")]
    public double WindchillC { get; set; }

    [JsonPropertyName("windchill_f")]
    public double WindchillF { get; set; }

    [JsonPropertyName("heatindex_c")]
    public double HeatindexC { get; set; }

    [JsonPropertyName("heatindex_f")]
    public double HeatindexF { get; set; }

    [JsonPropertyName("dewpoint_c")]
    public double DewpointC { get; set; }

    [JsonPropertyName("dewpoint_f")]
    public double DewpointF { get; set; }

    [JsonPropertyName("will_it_rain")]
    public int WillItRain { get; set; }

    [JsonPropertyName("chance_of_rain")]
    public int ChanceOfRain { get; set; }

    [JsonPropertyName("will_it_snow")]
    public int WillItSnow { get; set; }

    [JsonPropertyName("chance_of_snow")]
    public int ChanceOfSnow { get; set; }

    [JsonPropertyName("vis_km")]
    public double VisKm { get; set; }

    [JsonPropertyName("vis_miles")]
    public double VisMiles { get; set; }

    [JsonPropertyName("gust_mph")]
    public double GustMph { get; set; }

    [JsonPropertyName("gust_kph")]
    public double GustKph { get; set; }

    [JsonPropertyName("uv")]
    public double Uv { get; set; }
}

public class Location
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("tz_id")]
    public string TzId { get; set; } = string.Empty;

    [JsonPropertyName("localtime_epoch")]
    public int LocaltimeEpoch { get; set; }

    [JsonPropertyName("localtime")]
    public string Localtime { get; set; } = string.Empty;
}

public class Weather
{
    [JsonPropertyName("location")]
    public Location Location { get; set; } = new();

    [JsonPropertyName("current")]
    public Current Current { get; set; } = new();

    [JsonPropertyName("forecast")]
    public Forecast Forecast { get; set; } = new();

    [JsonPropertyName("alerts")]
    public Alerts Alerts { get; set; } = new();
}