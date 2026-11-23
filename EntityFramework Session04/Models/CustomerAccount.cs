using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework_Session04.Models;



public enum OwnershipType
{
    Primary = 1,
    CoHolder = 2
}

public class CustomerAccount
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string AccountNumber { get; set; } = null!;
    public Account Account { get; set; } = null!;

    public OwnershipType OwnershipType { get; set; }
    public DateTime OwnershipStartDate { get; set; }
    public AccountStatus AccountStatus { get; set; }
}
