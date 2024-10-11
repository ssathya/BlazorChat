namespace Models.DBModels;

public class TickerSlopeView
{
    public string Ticker { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public IndexNames ListedIndexes { get; set; }
    public string? Sector { get; set; }
    public float SnPWeight { get; set; }
    public float NasdaqWeight { get; set; }
    public float DowWeight { get; set; }
    public ComputedSlope ComputedSlope { get; set; } = new();

    public static implicit operator TickerSlopeView(TickerSlope tickerSlope)
    {
        return new TickerSlopeView
        {
            Ticker = tickerSlope.Ticker,
            ComputedSlope = tickerSlope.SlopeResults.OrderBy(r => r.Date).LastOrDefault() ?? new()
        };
    }
}