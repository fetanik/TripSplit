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

    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<ExpenseShare> ExpenseShares => Set<ExpenseShare>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Trip -> Participants
        modelBuilder.Entity<Participant>()
            .HasOne<Trip>()
            .WithMany()
            .HasForeignKey(participant => participant.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        // Trip -> Expenses
        modelBuilder.Entity<Expense>()
            .HasOne<Trip>()
            .WithMany()
            .HasForeignKey(expense => expense.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        // Participant -> Expenses paid by participant
        modelBuilder.Entity<Expense>()
            .HasOne<Participant>()
            .WithMany()
            .HasForeignKey(expense => expense.PaidByParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Expense -> ExpenseShares
        modelBuilder.Entity<ExpenseShare>()
            .HasOne<Expense>()
            .WithMany()
            .HasForeignKey(share => share.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Participant -> ExpenseShares
        modelBuilder.Entity<ExpenseShare>()
            .HasOne<Participant>()
            .WithMany()
            .HasForeignKey(share => share.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Один учасник може мати лише одну частку
        // в межах конкретної витрати
        modelBuilder.Entity<ExpenseShare>()
            .HasIndex(share => new
            {
                share.ExpenseId,
                share.ParticipantId
            })
            .IsUnique();

        // Грошові значення зберігаємо з точністю до копійок
        modelBuilder.Entity<Expense>()
            .Property(expense => expense.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ExpenseShare>()
            .Property(share => share.ShareAmount)
            .HasPrecision(18, 2);
    }
}