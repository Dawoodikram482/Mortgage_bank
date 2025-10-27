using BuyMyHouse.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BuyMyHouse.Core.Data;

public class HouseDbContext: DbContext
{
    public HouseDbContext(DbContextOptions<HouseDbContext> options) : base(options)
    {
    }

    public DbSet<House> Houses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<House>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrls)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .HasColumnType("nvarchar(max)");
            entity.HasIndex(e => e.Price);
            entity.HasIndex(e => e.City);
        });

        modelBuilder.Entity<House>().HasData(
            new House
            {
                Id = 1,
                Address = "Dam 1",
                City = "Amsterdam",
                Price = 450000,
                Bedrooms = 3,
                Bathrooms = 2,
                SquareMeters = 120,
                Description = "Beautiful canal house in the heart of Amsterdam. Features stunning views of the canals.",
                ListedDate = DateTime.UtcNow.AddDays(-30),
                IsAvailable = true
            },
            new House
            {
                Id = 2,
                Address = "Zeeweg 45",
                City = "Haarlem",
                Price = 350000,
                Bedrooms = 2,
                Bathrooms = 1,
                SquareMeters = 95,
                Description = "Modern apartment near the beach. Walking distance to all amenities.",
                ListedDate = DateTime.UtcNow.AddDays(-15),
                IsAvailable = true
            }
        );
    }
}
