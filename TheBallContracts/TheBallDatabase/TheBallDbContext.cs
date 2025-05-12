
using TheBallContracts.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TheBallDatabase.Models;

namespace TheBallDatabase;

internal class TheBallDbContext(IConfigurationDatabase configurationDatabase) : DbContext
{
    private readonly IConfigurationDatabase _configurationDatabase = configurationDatabase;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configurationDatabase?.ConnectionString, o => o.SetPostgresVersion(16, 2));
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Buyer>().HasIndex(x => x.PhoneNumber).IsUnique();

        modelBuilder.Entity<Manufacturer>().HasIndex(x => x.ManufacturerName).IsUnique();

        modelBuilder.Entity<Post>()
            .HasIndex(x => new { x.PostName, x.IsActual })
            .IsUnique()
            .HasFilter($"\"{nameof(Post.IsActual)}\" = TRUE");

        modelBuilder.Entity<Post>()
            .HasIndex(x => new { x.PostId, x.IsActual })
            .IsUnique()
            .HasFilter($"\"{nameof(Post.IsActual)}\" = TRUE");

        modelBuilder.Entity<Gift>()
            .HasIndex(x => new { x.Name, x.IsDeleted })
            .IsUnique()
            .HasFilter($"\"{nameof(Gift.IsDeleted)}\" = FALSE");

        modelBuilder
            .Entity<Gift>()
            .HasOne(x => x.Manufacturer)
            .WithMany(x => x.Gifts)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SaleGift>().HasKey(x => new { x.SaleId, x.GiftId });
    }
    public DbSet<Buyer> Buyers { get; set; }    
    public DbSet<Gift> Gifts { get; set; }
    public DbSet<GiftHistory> GiftHistories { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<Sale> Sales {  get; set; }
    public DbSet<SaleGift> SalesGifts { get; set; }
    public DbSet<Worker> Workers { get; set; }
}
