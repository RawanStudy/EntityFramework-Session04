using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework_Session04.Models;

public enum CustomerType
{
    Individual = 1,
    Business = 2
}

public class Customer
{
    public int Id { get; set; }                            // PK
    public string FullName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public CustomerType CustomerType { get; set; }

    // Many-to-Many with Account (via CustomerAccount)
    public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
}