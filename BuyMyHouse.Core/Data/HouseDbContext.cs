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

        var stringListComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        modelBuilder.Entity<House>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Bedrooms).HasColumnType("int");
            entity.Property(e => e.Bathrooms).HasColumnType("int");
            entity.Property(e => e.SquareMeters).HasColumnType("int");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrls)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .Metadata.SetValueComparer(stringListComparer);
            entity.HasIndex(e => e.Price);
            entity.HasIndex(e => e.City);
        });

        modelBuilder.Entity<House>().HasData(
            new House
            {
                Id = 1,
                Address = "Dam 1",
                City = "Amsterdam",
                Price = 450000m,
                Bedrooms = 3,
                Bathrooms = 2,
                SquareMeters = 120,
                Description = "Beautiful canal house in the heart of Amsterdam. Features stunning views of the canals.",
                ListedDate = new DateTime(2024, 9, 27, 0, 0, 0, DateTimeKind.Utc),
                IsAvailable = true
            },
            new House
            {
                Id = 2,
                Address = "Zeeweg 45",
                City = "Haarlem",
                Price = 350000m,
                Bedrooms = 2,
                Bathrooms = 1,
                SquareMeters = 95,
                Description = "Modern apartment near the beach. Walking distance to all amenities.",
                ListedDate = new DateTime(2024, 10, 12, 0, 0, 0, DateTimeKind.Utc),
                IsAvailable = true
            }
        );
    }
}
