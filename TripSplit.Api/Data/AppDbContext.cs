using Microsoft.EntityFrameworkCore;
using TripSplit.Api.Models;

namespace TripSplit.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Trip> Trips => Set<Trip>();

    public DbSet<Participant> Participants => Set<Participant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Participant>()
            .HasOne<Trip>()
            .WithMany()
            .HasForeignKey(participant => participant.TripId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}