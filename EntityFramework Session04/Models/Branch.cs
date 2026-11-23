using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework_Session04.Models;

public class Branch
{
    public string Code { get; set; } = null!;              // PK
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    // One-to-One: Branch managed by one Manager
    public int ManagerId { get; set; }
    public Manager Manager { get; set; } = null!;

    // One-to-Many: Branch has many Accounts
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
