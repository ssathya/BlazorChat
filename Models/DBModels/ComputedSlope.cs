namespace Models.DBModels;

public class ComputedSlope
{
    public DateTime Date { get; set; }
    public double? Slope { get; set; }
    public double? Intercept { get; set; }
    public double? StdDev { get; set; }
    public double? RSquared { get; set; }
    public decimal? Line { get; set; }

}