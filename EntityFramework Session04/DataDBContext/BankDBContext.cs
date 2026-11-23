using System;
using System.Collections.Generic;
using System.Text;
using EntityFramework_Session04.Models;
using Microsoft.EntityFrameworkCore;
namespace EntityFramework_Session04.DataDBContext;





public class BankContext : DbContext
{
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<CustomerAccount> CustomerAccounts => Set<CustomerAccount>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(
            "Server=.;Database=NationalBankDB;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ================= Branch =================
        modelBuilder.Entity<Branch>(e =>
        {
            e.HasKey(b => b.Code);
            e.Property(b => b.Code).HasMaxLength(20);
            e.Property(b => b.Name).HasMaxLength(100).IsRequired();
            e.Property(b => b.Address).HasMaxLength(200).IsRequired();
            e.Property(b => b.PhoneNumber).HasMaxLength(20).IsRequired();
        });

        // ================= Manager =================
        modelBuilder.Entity<Manager>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.FullName).HasMaxLength(100).IsRequired();
            e.Property(m => m.Email).HasMaxLength(150).IsRequired();
            e.Property(m => m.PhoneNumber).HasMaxLength(20).IsRequired();
        });

        // ============ One-to-One: Branch <-> Manager ============
        modelBuilder.Entity<Branch>()
            .HasOne(b => b.Manager)
            .WithOne(m => m.Branch)
            .HasForeignKey<Branch>(b => b.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ================= Customer =================
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.FullName).HasMaxLength(100).IsRequired();
            e.Property(c => c.NationalId).HasMaxLength(20).IsRequired();
            e.HasIndex(c => c.NationalId).IsUnique();
            e.Property(c => c.Email).HasMaxLength(150).IsRequired();
            e.Property(c => c.PhoneNumber).HasMaxLength(20).IsRequired();
            e.Property(c => c.Address).HasMaxLength(200).IsRequired();
            e.Property(c => c.CustomerType).HasConversion<int>();
        });

        // ================= Account =================
        modelBuilder.Entity<Account>(e =>
        {
            e.HasKey(a => a.AccountNumber);
            e.Property(a => a.AccountNumber).HasMaxLength(20);
            e.Property(a => a.CurrentBalance).HasColumnType("decimal(18,2)");
            e.Property(a => a.AccountType).HasConversion<int>();
            e.Property(a => a.AccountStatus).HasConversion<int>();

            e.HasOne(a => a.Branch)
             .WithMany(b => b.Accounts)
             .HasForeignKey(a => a.BranchCode)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ============ Many-to-Many: Customer <-> Account ============
        modelBuilder.Entity<CustomerAccount>(e =>
        {
            e.HasKey(ca => new { ca.CustomerId, ca.AccountNumber });

            e.HasOne(ca => ca.Customer)
             .WithMany(c => c.CustomerAccounts)
             .HasForeignKey(ca => ca.CustomerId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(ca => ca.Account)
             .WithMany(a => a.CustomerAccounts)
             .HasForeignKey(ca => ca.AccountNumber)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(ca => ca.OwnershipType).HasConversion<int>();
            e.Property(ca => ca.AccountStatus).HasConversion<int>();
        });

        // ================= Transaction =================
        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(t => t.TransactionNumber);
            e.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            e.Property(t => t.TransactionType).HasConversion<int>();
            e.Property(t => t.Note).HasMaxLength(250);

            e.HasOne(t => t.Account)
             .WithMany(a => a.Transactions)
             .HasForeignKey(t => t.AccountNumber)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ================= Seeding =================
        modelBuilder.Entity<Manager>().HasData(
            new Manager
            {
                Id = 1,
                FullName = "Ahmed Hassan",
                Email = "ahmed.hassan@nbank.com",
                PhoneNumber = "01000000001",
                HireDate = new DateTime(2015, 3, 1)
            },
            new Manager
            {
                Id = 2,
                FullName = "Sara Mostafa",
                Email = "sara.mostafa@nbank.com",
                PhoneNumber = "01000000002",
                HireDate = new DateTime(2018, 7, 15)
            }
        );

        modelBuilder.Entity<Branch>().HasData(
            new Branch
            {
                Code = "CAI-01",
                Name = "Cairo Main Branch",
                Address = "Tahrir Square, Cairo",
                PhoneNumber = "0223456789",
                ManagerId = 1
            },
            new Branch
            {
                Code = "ALX-01",
                Name = "Alexandria Branch",
                Address = "Corniche Road, Alexandria",
                PhoneNumber = "0345678901",
                ManagerId = 2
            }
        );
    }
}