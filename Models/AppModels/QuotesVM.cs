namespace Models.AppModels;

public class QuotesVM
{
    public string? Ticker { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal DayChange { get; set; }
    public float DayChangePercent { get; set; }
    public decimal DayHigh { get; set; }
    public decimal DayLow { get; set; }
    public decimal OpenPrice { get; set; }
    public decimal PreviousClose { get; set; }
    public DateTime ReportingTime { get; set; }

    public static implicit operator QuotesVM(Quote quote)
    {
        QuotesVM quotesVM = new()
        {
            CurrentPrice = (decimal)(quote.CurrentPrice ?? 0.0),
            DayChange = (decimal)(quote.DayChange ?? 0.0),
            DayChangePercent = (float)(quote.DayChangePercent ?? 0.0),
            DayHigh = (decimal)(quote.High ?? 0.0),
            DayLow = (decimal)(quote.Low ?? 0.0),
            OpenPrice = (decimal)(quote.OpenPrice ?? 0.0),
            PreviousClose = (decimal)(quote.PreviousClose ?? 0.0),
            ReportingTime = DateTime.UnixEpoch.AddSeconds(quote.EpochTimeStamp).ToLocalTime()
        };
        return quotesVM;
    }


}
