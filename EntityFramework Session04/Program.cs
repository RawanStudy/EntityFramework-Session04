using EntityFramework_Session04.DataDBContext;
using EntityFramework_Session04.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using EntityFramework_Session04.Models;
using EntityFramework_Session04.Models;
using Microsoft.EntityFrameworkCore;
namespace EntityFramework_Session04;

public class Program
{
     static async Task Main(string[] args)
    {

        var db = new BankContext();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("National Bank - Management");
            Console.WriteLine("================================");
            Console.WriteLine("1) Add a new Customer");
            Console.WriteLine("2) Open a new Account for a Customer");
            Console.WriteLine("3) Update Account Status (Active / Closed)");
            Console.WriteLine("4) Remove an Account from a Customer");
            Console.WriteLine("5) List all Customers (with accounts)");
            Console.WriteLine("0) Exit");
            Console.WriteLine("--------------------------------");
            Console.Write("Enter choice: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1": await AddCustomer(); break;
                    case "2": await OpenAccount(); break;
                    case "3": await UpdateAccountStatus(); break;
                    case "4": await RemoveAccountFromCustomer(); break;
                    case "5": await ListCustomers(); break;
                    case "0":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number from 0 to 5.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }

            if (choice != "0")
            {
                Console.WriteLine();
                Console.Write("Press any key to return to the menu...");
                Console.ReadKey();
            }
        }

        // ============================================================
        // 1) Add new Customer
        // ============================================================
        async Task AddCustomer()
        {
            Console.WriteLine("--- Add New Customer ---");

            // Full Name
            Console.Write("Full Name : ");
            string fullName = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3)
                throw new Exception("Full name must be at least 3 characters.");

            // National ID (14 digits — Egyptian format)
            Console.Write("National ID : ");
            string nationalId = Console.ReadLine()?.Trim() ?? "";
            if (!Regex.IsMatch(nationalId, @"^\d{14}$"))
                throw new Exception("National ID must be exactly 14 digits.");
            if (await db.Customers.AnyAsync(c => c.NationalId == nationalId))
                throw new Exception($"A customer with National ID '{nationalId}' already exists.");

            // Date of Birth
            Console.Write("Date of Birth : (yyyy-MM-dd) ");
            string dobInput = Console.ReadLine()?.Trim() ?? "";
            if (!DateTime.TryParseExact(dobInput, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob))
                throw new Exception("Invalid date format. Expected yyyy-MM-dd.");
            if (dob >= DateTime.Today || dob < new DateTime(1900, 1, 1))
                throw new Exception("Date of birth must be between 1900 and today.");

            // Email
            Console.Write("Email : ");
            string email = Console.ReadLine()?.Trim() ?? "";
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("Invalid email format.");

            // Phone
            Console.Write("Phone : ");
            string phone = Console.ReadLine()?.Trim() ?? "";
            if (!Regex.IsMatch(phone, @"^\d{10,15}$"))
                throw new Exception("Phone must contain 10–15 digits only.");

            // Address
            Console.Write("Address : ");
            string address = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("Address is required.");

            // Customer Type
            Console.WriteLine("Customer Type:");
            Console.WriteLine("  1) Individual");
            Console.WriteLine("  2) Business");
            Console.Write("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out int typeChoice) || typeChoice < 1 || typeChoice > 2)
                throw new Exception("Invalid customer type. Enter 1 or 2.");

            var customer = new Customer
            {
                FullName = fullName,
                NationalId = nationalId,
                DateOfBirth = dob,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CustomerType = (CustomerType)typeChoice
            };

            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            Console.WriteLine();
            Console.WriteLine($"Customer created successfully. CustomerId = {customer.Id}");
        }

        // ============================================================
        // 2) Open new Account for a Customer
        // ============================================================
        async Task OpenAccount()
        {
            Console.WriteLine("--- Open New Account ---");

            // Account Number
            Console.Write("Account Number : ");
            string accNo = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(accNo))
                throw new Exception("Account number is required.");
            if (await db.Accounts.AnyAsync(a => a.AccountNumber == accNo))
                throw new Exception($"Account '{accNo}' already exists.");

            // Account Type
            Console.WriteLine("Account Type:");
            Console.WriteLine("  1) Savings");
            Console.WriteLine("  2) Current");
            Console.WriteLine("  3) Business");
            Console.Write("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out int accTypeChoice) || accTypeChoice < 1 || accTypeChoice > 3)
                throw new Exception("Invalid account type. Enter 1, 2, or 3.");

            // Branch Code
            Console.Write("Branch Code : ");
            string branchCode = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(branchCode))
                throw new Exception("Branch code is required.");

            // Customer Id
            Console.Write("Customer Id : ");
            if (!int.TryParse(Console.ReadLine(), out int customerId) || customerId <= 0)
                throw new Exception("Invalid customer Id.");

            // Ownership Role
            Console.WriteLine("Ownership Role:");
            Console.WriteLine("  1) Primary");
            Console.WriteLine("  2) CoHolder");
            Console.Write("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out int ownerChoice) || ownerChoice < 1 || ownerChoice > 2)
                throw new Exception("Invalid ownership role. Enter 1 or 2.");

            // Verify branch & customer
            Console.WriteLine($"Validating branch '{branchCode}' and customer #{customerId}...");

            var branch = await db.Branches.FindAsync(branchCode);
            if (branch == null)
                throw new Exception($"Branch '{branchCode}' does not exist.");

            var customer = await db.Customers.FindAsync(customerId);
            if (customer == null)
                throw new Exception($"Customer #{customerId} does not exist.");

            // Create Account + link
            var account = new Account
            {
                AccountNumber = accNo,
                AccountType = (AccountType)accTypeChoice,
                AccountStatus = AccountStatus.Active,
                CurrentBalance = 0m,
                OpeningDate = DateTime.Now,
                BranchCode = branchCode
            };

            var link = new CustomerAccount
            {
                CustomerId = customerId,
                AccountNumber = accNo,
                OwnershipType = (OwnershipType)ownerChoice,
                OwnershipStartDate = DateTime.Now,
                AccountStatus = AccountStatus.Active
            };

            db.Accounts.Add(account);
            db.CustomerAccounts.Add(link);
            await db.SaveChangesAsync();

            Console.WriteLine($"Account '{accNo}' created and linked to customer {customerId} as {(OwnershipType)ownerChoice} owner.");
        }

        // ============================================================
        // 3) Update Account Status (toggle)
        // ============================================================
        async Task UpdateAccountStatus()
        {
            Console.WriteLine("--- Update Account Status ---");

            // Account Number
            Console.Write("Account Number : ");
            string accNo = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(accNo))
                throw new Exception("Account number is required.");

            // Customer Id
            Console.Write("Customer Id : ");
            if (!int.TryParse(Console.ReadLine(), out int customerId) || customerId <= 0)
                throw new Exception("Invalid customer Id.");

            // Find the link
            var link = await db.CustomerAccounts
                .FirstOrDefaultAsync(ca => ca.AccountNumber == accNo && ca.CustomerId == customerId);

            if (link == null)
                throw new Exception($"No ownership link found for account '{accNo}' and customer #{customerId}.");

            // Toggle status (Active <-> Closed)
            var oldStatus = link.AccountStatus;
            var newStatus = oldStatus == AccountStatus.Active
                            ? AccountStatus.Closed
                            : AccountStatus.Active;

            Console.WriteLine($"Current status: {oldStatus}  ->  Toggling to: {newStatus}");

            link.AccountStatus = newStatus;

            // Also update the Account master row so listing shows the correct status
            var account = await db.Accounts.FindAsync(accNo);
            if (account != null)
                account.AccountStatus = newStatus;

            await db.SaveChangesAsync();
            Console.WriteLine($"Status updated to {newStatus}.");
        }

        // ============================================================
        // 4) Remove Account from Customer
        // ============================================================
        async Task RemoveAccountFromCustomer()
        {
            Console.WriteLine("--- Remove Account From Customer ---");

            // Account Number
            Console.Write("Account Number : ");
            string accNo = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(accNo))
                throw new Exception("Account number is required.");

            // Customer Id
            Console.Write("Customer Id    : ");
            if (!int.TryParse(Console.ReadLine(), out int customerId) || customerId <= 0)
                throw new Exception("Invalid customer Id.");

            // Find link
            var link = await db.CustomerAccounts
                .FirstOrDefaultAsync(ca => ca.AccountNumber == accNo && ca.CustomerId == customerId);

            if (link == null)
                throw new Exception($"No ownership link found for account '{accNo}' and customer #{customerId}.");

            // Delete link
            db.CustomerAccounts.Remove(link);
            await db.SaveChangesAsync();
            Console.WriteLine("Ownership link deleted.");

            // If it was the last owner, delete the account too
            bool hasOtherOwners = await db.CustomerAccounts.AnyAsync(ca => ca.AccountNumber == accNo);
            if (!hasOtherOwners)
            {
                var account = await db.Accounts.FindAsync(accNo);
                if (account != null)
                {
                    db.Accounts.Remove(account);
                    await db.SaveChangesAsync();
                    Console.WriteLine($"    That was the last owner - account '{accNo}' was also removed.");
                }
            }
        }

        // ============================================================
        // 5) List all Customers (with accounts) — Eager Loading
        // ============================================================
        async Task ListCustomers()
        {
            Console.WriteLine("--- All Customers ---");

            var customers = await db.Customers
                .Include(c => c.CustomerAccounts)
                    .ThenInclude(ca => ca.Account)
                        .ThenInclude(a => a.Branch)
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (!customers.Any())
            {
                Console.WriteLine("No customers found.");
                return;
            }

            foreach (var c in customers)
            {
                Console.WriteLine($"#{c.Id} {c.FullName} ({c.CustomerType})");

                if (!c.CustomerAccounts.Any())
                {
                    Console.WriteLine("(no accounts)");
                }
                else
                {
                    foreach (var ca in c.CustomerAccounts)
                    {
                        var a = ca.Account;
                        Console.WriteLine(
                            $"{a.AccountNumber} {a.AccountType} " +
                            $"Balance: {a.CurrentBalance:N2} " +
                            $"{ca.OwnershipType} {ca.AccountStatus} " +
                            $"@ {a.Branch.Name}");
                    }
                }
            }
        }
    }
}
