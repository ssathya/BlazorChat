namespace Models.DBModels;

public partial class TickerSlope
{
    public Guid Id { get; set; }

    public Period Period { get; set; }

    public string Ticker { get; set; } = null!;

    public List<ComputedSlope> SlopeResults { get; set; } = [];
}

public enum Period
{
    Yearly,
    HalfYearly,
    Quarterly,
    Monthly
}