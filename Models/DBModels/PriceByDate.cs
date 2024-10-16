﻿namespace Models.DBModels;

public partial class PriceByDate
{
    public int Id { get; set; }

    public string Ticker { get; set; } = null!;

    public DateTime Date { get; set; }

    public double Open { get; set; }

    public double High { get; set; }

    public double Low { get; set; }

    public double Close { get; set; }

    public double AdjClose { get; set; }

    public double Volume { get; set; }
}
