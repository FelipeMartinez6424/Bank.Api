using Bank.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Movement> Movements => Set<Movement>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Person>().ToTable("Persons");

        mb.Entity<Client>()
            .HasKey(c => c.Id);
        mb.Entity<Client>()
            .HasIndex(c => c.ClientCode).IsUnique();
        mb.Entity<Client>()
            .HasOne(c => c.Person)
            .WithOne(p => p.Client!)
            .HasForeignKey<Client>(c => c.Id)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Account>()
            .HasIndex(a => a.AccountNumber).IsUnique();
        mb.Entity<Account>()
            .Property(a => a.InitialBalance).HasPrecision(18, 2);
        mb.Entity<Account>()
            .Property(a => a.CurrentBalance).HasPrecision(18, 2);

        mb.Entity<Movement>()
            .Property(m => m.Amount).HasPrecision(18, 2);
        mb.Entity<Movement>()
            .Property(m => m.AvailableBalanceAfter).HasPrecision(18, 2);
    }
}
