using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework_Session04.Models;


public enum TransactionType
{
    Deposit = 1,
    Withdrawal = 2,
    Transfer = 3
}

public class Transaction
{
    public int TransactionNumber { get; set; }             // PK
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? Note { get; set; }

    // FK: Account
    public string AccountNumber { get; set; } = null!;
    public Account Account { get; set; } = null!;
}