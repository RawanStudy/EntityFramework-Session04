using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework_Session04.Models;

public enum AccountType
{
    Savings = 1,
    Current = 2,
    Business = 3
}

public enum AccountStatus
{
    Active = 1,
    Closed = 2
}

public class Account
{
    public string AccountNumber { get; set; } = null!;     // PK
    public decimal CurrentBalance { get; set; }
    public AccountType AccountType { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public DateTime OpeningDate { get; set; }

    // FK: Branch
    public string BranchCode { get; set; } = null!;
    public Branch Branch { get; set; } = null!;

    // Many-to-Many with Customer
    public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();

    // One-to-Many: Transactions
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}