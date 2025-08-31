using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Context;

public class MeterContext(DbContextOptions<MeterContext> options) : DbContext(options)
{
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<MeterRead> MeterReads => Set<MeterRead>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure UserAccount primary key
        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId);
            entity.Property(e => e.AccountId).ValueGeneratedNever();
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.ToTable("UserAccounts");
        });

        // Configure MeterRead composite key (Account + ReadingDate)
        modelBuilder.Entity<MeterRead>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.MeterReadingDateTime });
            entity.Property(e => e.AccountId).ValueGeneratedNever();
            entity.Property(e => e.MeterReadingDateTime).ValueGeneratedNever();
            entity.ToTable("MeterReads");
        });

        base.OnModelCreating(modelBuilder);
    }
}
