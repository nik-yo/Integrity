using Integrity.Banking.Domain.Models.Config;
using Integrity.Banking.Domain.Models.Database;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Integrity.Banking.Infrastructure.Database
{
    /// <summary>
    /// Create migration: dotnet ef migrations add InitialCreate
    /// Update migration: dotnet ef database update
    /// </summary>
    public class BankingDbContext : DbContext
    {
        private readonly DbConfig _dbConfig;
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public BankingDbContext() //For EF migration
        {
            _dbConfig = new DbConfig();
        }

        public BankingDbContext(DbConfig dbConfig)
        {
            _dbConfig = dbConfig;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_dbConfig.ConnectionString, 
                            options => options.EnableRetryOnFailure());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Accounts)
                .WithMany(e => e.Customers);



        }
    }
}
