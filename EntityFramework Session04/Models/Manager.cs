using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework_Session04.Models;

public class Manager
{
    public int Id { get; set; }                            // PK
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime HireDate { get; set; }

    // One-to-One (inverse)
    public Branch? Branch { get; set; }
}
