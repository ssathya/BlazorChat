namespace Models.AppModels;

public class StockExcel
{
    public string Ticker { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    //public string Period { get; set; } = string.Empty;
    public double StartingSlope { get; set; }

    public double EndingSlope { get; set; }
}