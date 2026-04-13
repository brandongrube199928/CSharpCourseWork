using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== Customer Order Tracker (EF Core) ===");

// Ensure DB is up-to-date with migrations
using (var ctx = new TrackerContext())
{
    try
    {
        ctx.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Database migration failed.");
        Console.WriteLine(ex.Message);
        Console.WriteLine("Make sure you ran:");
        Console.WriteLine("  dotnet ef migrations add InitialCreate");
        Console.WriteLine("  dotnet ef database update");
        return;
    }
}
//UI logic inside a while true loop, no need to modify
while (true)
{
    PrintMenu();
    Console.Write("Choose an option: ");
    var choice = (Console.ReadLine() ?? "").Trim();

    try
    {
        switch (choice)
        {
            case "1":
                await AddCustomerInteractive();
                break;
            case "2":
                await AddOrderInteractive();
                break;
            case "3":
                await ViewOrdersInteractive();
                break;
            case "4":
                await UpdateCustomerEmailInteractive();
                break;
            case "5":
                await DeleteCustomerInteractive();
                break;
            case "6":
                await DeleteOrderInteractive();
                break;
            case "7":
                await ListCustomersInteractive();
                break;
            case "0":
                Console.WriteLine("Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid option. Try again.");
                break;
        }
    }
    catch (Exception ex)
    {
        // Catch-all so the menu doesn't crash
        Console.WriteLine($"Error: {ex.Message}");
    }

    Console.WriteLine();
}

// Method to Print Menu, no need to Modify

static void PrintMenu()
{
    Console.WriteLine("""
    
    Menu
    1) Add Customer
    2) Add Order (by Customer ID)
    3) View Orders (with Customer names)
    4) Update Customer Email
    5) Delete Customer
    6) Delete Order
    7) List Customers (with order counts)
    0) Exit
    """);
}

// ---------------- Features ----------------

static async Task AddCustomerInteractive()
{
    //Code to get User Input:
    Console.Write("Customer name: ");
    var name = (Console.ReadLine() ?? "").Trim();

    Console.Write("Customer email: ");
    var email = (Console.ReadLine() ?? "").Trim();

    // Write an if statement to validate Name and Email input
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Customer name is required.");
        return;
    }
    if (string.IsNullOrWhiteSpace(email))
    {
        Console.WriteLine("Email is required.");
        return;
    }


    //Create a new TrackerContext as a var
    using var ctx = new TrackerContext();
    //Create a var for a customer
    var customer = new Customer { Name = name, Email = email };

    //Then Add the new customer to Customers
    ctx.Customers.Add(customer);

    //Save Changes
    await ctx.SaveChangesAsync();

    //Report to end user that the customer was added
    Console.WriteLine($"Customer added: {customer.CustomerID} - {customer.Name} ({ customer.Email}");

}

static async Task AddOrderInteractive()
{
    int customerId = ReadInt("Customer ID: ");
    double totalAmount = ReadDouble("Order Total Amount: ");

    // Validation
    if (totalAmount < 0)
    {
        Console.WriteLine("TotalAmount must be >= 0.");
        return;
    }

    using var ctx = new TrackerContext();

    // Create some validtion to prevent orphaned orders: verify customer exists
    var customer = await ctx.Customers.FindAsync(customerId);
    if (customer == null)
    {
        Console.WriteLine("Customer not found. Order not created.");
        return;
    }

    //Then create the new order and add it using the Tracker Context (ctx)
    var order = new Order
    {
        CustomerID = customerId,
        TotalAmount = totalAmount,
        OrderDate = DateTime.UtcNow
    };
    ctx.Orders.Add(order);
    //After you add the order, use this to save the information:
    await ctx.SaveChangesAsync();

    //Then write an update to the console for the end user:
    Console.WriteLine($"Order added: {order.OrderID} - ${order.TotalAmount:0.00} on {order.OrderDate:u} for {customer.Name}");
}

static async Task ViewOrdersInteractive()
{
    using var ctx = new TrackerContext();

    //Code to include customer information in our Orders query
    var orders = await ctx.Orders
        .Include(o => o.Customer)
        .OrderBy(o => o.OrderDate)
        .ToListAsync();

	//Begins the readback and handles empty Orders table
    Console.WriteLine("\nOrders:");
    if (orders.Count == 0)
    {
        Console.WriteLine(" (none)");
        return;
    }

    //Create a foreach for the orders we queried
    foreach (var o in orders)
    {
        var custName = o.Customer?.Name ?? "(unknown)";
        Console.WriteLine($" - Order {o.OrderID}: {o.OrderDate:u} ${o.TotalAmount:0.00} Customer: {custName} (ID {o.CustomerID})");
    }
	//For each one, get the customer name or make it Unknown
	//Print the OrderID, OrderDate, TotalAmount, CustomerName, CustomerID

}

static async Task UpdateCustomerEmailInteractive()
{
    //Code to get some User Input and Validate the Email:
	int customerId = ReadInt("Customer ID: ");
    Console.Write("New email: ");
    var newEmail = (Console.ReadLine() ?? "").Trim();

    if (string.IsNullOrWhiteSpace(newEmail))
    {
        Console.WriteLine("Email is required.");
        return;
    }

    using var ctx = new TrackerContext();
    var customer = await ctx.Customers.FindAsync(customerId);

    //Create validaiton for customer not found (is null)
    if (customer == null)
    {
        Console.WriteLine("Customer not found.");
        return;
    }

	//Set the customer Email to hte newEmail
    customer.Email = newEmail;


    //Save the changes
    await ctx.SaveChangesAsync();


    //Update the end user in the console
    Console.WriteLine($"Customer updated: {customer.CustomerID}: {customer.Name} --> {customer.Email}");

}

static async Task DeleteCustomerInteractive()
{
    int customerId = ReadInt("Customer ID to delete: ");

    using var ctx = new TrackerContext();

    var customer = await ctx.Customers
        .Include(c => c.Orders)
        .FirstOrDefaultAsync(c => c.CustomerID == customerId);

    if (customer is null)
    {
        Console.WriteLine("Customer not found.");
        return;
    }

    Console.WriteLine($"Deleting customer: {customer.CustomerID} - {customer.Name} ({customer.Email})");
    if (customer.Orders.Count > 0)
        Console.WriteLine($"NOTE: This customer has {customer.Orders.Count} order(s).");

	//Force the end user to write YES before we delete the customer
	//If anything other than YES or yes comes in, report that it was cancelled
    Console.Write("Type YES to confirm: ");
    var confirm = (Console.ReadLine() ?? "").Trim();

    if (!confirm.Equals("YES", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Delete cancelled.");
        return;
    }


    // This will succeed if cascade delete is configured.
    // If not, you may need to delete orders first.
    ctx.Customers.Remove(customer);
    await ctx.SaveChangesAsync();

    Console.WriteLine("Customer deleted.");
}

static async Task DeleteOrderInteractive()
{
    int orderId = ReadInt("Order ID to delete: ");

    using var ctx = new TrackerContext();
    var order = await ctx.Orders.FindAsync(orderId);

    //Validate if the order is null and exit the oeration
    if (order is null)
    {
        Console.WriteLine("Order not found.");
        return;
    }

	//Remove from the Orders table and then save the changes
    ctx.Orders.Remove(order);
    await ctx.SaveChangesAsync();


    ///Report the result to the end user:
    Console.WriteLine($"Order {orderId} deleted.");
    
}

//Freebie select statement to use as an example. No modification needed:
static async Task ListCustomersInteractive()
{
    using var ctx = new TrackerContext();

    var customers = await ctx.Customers
        .Select(c => new
        {
            c.CustomerID,
            c.Name,
            c.Email,
            OrderCount = c.Orders.Count,
            TotalSpent = c.Orders.Sum(o => (double?)o.TotalAmount) ?? 0.0
        })
        .OrderBy(c => c.Name)
        .ToListAsync();

    Console.WriteLine("\nCustomers:");
    if (customers.Count == 0)
    {
        Console.WriteLine(" (none)");
        return;
    }

    foreach (var c in customers)
    {
        Console.WriteLine($" - {c.CustomerID}: {c.Name,-20} {c.Email,-25} | Orders: {c.OrderCount,2}  Spent: ${c.TotalSpent:0.00}");
    }
}
//!!! Helper Methods to make reading input easer, no need to modify
// ---------------- Input Helpers ----------------

static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var s = (Console.ReadLine() ?? "").Trim();

        if (int.TryParse(s, out int value))
            return value;

        Console.WriteLine("Please enter a valid whole number.");
    }
}

static double ReadDouble(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var s = (Console.ReadLine() ?? "").Trim();

        // Allow both current culture and invariant (helps student machines)
        if (double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out double value) ||
            double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return value;

        Console.WriteLine("Please enter a valid number (example: 249.99).");
    }
}