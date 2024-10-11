using System.Text.Json.Serialization;

namespace Models.AppModels;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class Quote
{
    [JsonPropertyName("c")]
    public double? CurrentPrice { get; set; }

    [JsonPropertyName("d")]
    public double? DayChange { get; set; }

    [JsonPropertyName("dp")]
    public double? DayChangePercent { get; set; }

    [JsonPropertyName("h")]
    public double? High { get; set; }

    [JsonPropertyName("l")]
    public double? Low { get; set; }

    [JsonPropertyName("o")]
    public double? OpenPrice { get; set; }

    [JsonPropertyName("pc")]
    public double? PreviousClose { get; set; }

    [JsonPropertyName("t")]
    public int EpochTimeStamp { get; set; }
}