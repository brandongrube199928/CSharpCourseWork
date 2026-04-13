using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

public class TrackerContext : DbContext
{
    //Create a public DbSet for Customers and Orders. Set them by type
	public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();

    //Create a protected override void to set the Data Source to CustomerOrders.db
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=CustomerOrders.db");


    //Use this potected override void for OnModelCreating
    //Pass in a ModelBuilder
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Use the ModelBuilder.Entity to ensure Customer has a unique email
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();


        //Use the ModelBuilder.Entity to set a One to Many relationship between customer and orders
        //Enforce the Foreign Key with HasForeignKey
        //Use OnDelete to cascade delete orders for the deleted customer
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer!)
            .HasForeignKey(o => o.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);


        //Use ModelBuilder.Entity to ensure that Order has a Total Amount
        //Set Precision to (18, 2)
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);
    }
}
