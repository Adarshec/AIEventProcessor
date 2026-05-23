using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Device>()
            .HasIndex(x => x.DeviceIdText)
            .IsUnique();
        builder.Entity<Event>()
            .Property(x => x.Temperature)
            .HasPrecision(10, 2);
        builder.Entity<Event>()
            .HasIndex(x => x.Timestamp);

        builder.Entity<Event>()
            .HasIndex(x => new { x.DeviceId, x.Timestamp });

        builder.Entity<Event>()
            .Property(x => x.Metadata);

        builder.Entity<Event>()
        .HasOne(x => x.Device)
        .WithMany(x => x.Events)
        .HasForeignKey(x => x.DeviceId);
    }
}