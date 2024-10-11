namespace Models.DBModels;

public partial class IndexComponent
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public IndexNames ListedIndexes { get; set; }

    public string? Sector { get; set; }

    public string? SubSector { get; set; }

    public string Ticker { get; set; } = null!;

    public float SnPWeight { get; set; }

    public float NasdaqWeight { get; set; }

    public float DowWeight { get; set; }

    public DateTime? LastUpdated { get; set; }
}

[Flags]
public enum IndexNames
{
    None = 0b_0000_0000,
    SnP = 0b_0000_0001,
    Nasdaq = 0b_0000_0010,
    Dow = 0b_0000_0100
}