namespace Models.DBModels;

public partial class SelectedTicker
{
    public int Id { get; set; }

    public string Ticker { get; set; } = null!;

    public DateTime Date { get; set; }

    public double Close { get; set; }

    public double AnnualPercentGain { get; set; }

    public double HalfYearlyPercentGain { get; set; }

    public double QuarterYearlyPercentGain { get; set; }

    public DateTime LastUpdated { get; set; }

    public string? CompanyName { get; set; }
}
