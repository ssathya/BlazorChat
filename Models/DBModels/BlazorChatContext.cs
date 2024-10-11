using Microsoft.EntityFrameworkCore;

namespace Models.DBModels;
public class BlazorChatContext(DbContextOptions<BlazorChatContext> options) : DbContext(options)
{
    public DbSet<IndexComponent> IndexComponents { get; set; }
    public DbSet<PriceByDate> PriceByDates { get; set; }
    public DbSet<SelectedTicker> SelectedTickers { get; set; }
    public DbSet<TickerSlope> TickerSlopes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TickerSlope>()
            .OwnsMany(c => c.SlopeResults, d =>
            {
                d.ToJson();
            });
        base.OnModelCreating(modelBuilder);
    }
}
