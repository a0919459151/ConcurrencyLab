namespace ConcurrencyLab.EFCore;

public class ConcurrencyLabDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public ConcurrencyLabDbContext(DbContextOptions<ConcurrencyLabDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}
