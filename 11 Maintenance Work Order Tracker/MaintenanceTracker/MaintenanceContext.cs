// MaintenanceContext.cs
using Microsoft.EntityFrameworkCore;

namespace MaintenanceTracker.WinForms;

public class MaintenanceContext : DbContext
{
    public DbSet<Technician> Technicians => Set<Technician>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    private readonly string _connStr;

    // Default ctor (app use)
    public MaintenanceContext() : this("Data Source=Maintenance.db") { }

    // Overload for tests/in-memory
    public MaintenanceContext(string connectionString)
    {
        _connStr = connectionString;
    }

    // Overload for DI/options (integration tests)
    public MaintenanceContext(DbContextOptions<MaintenanceContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrWhiteSpace(_connStr))
            optionsBuilder.UseSqlite(_connStr);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Technician>()
            .HasMany(t => t.WorkOrders)
            .WithOne(w => w.Technician!)
            .HasForeignKey(w => w.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkOrder>()
            .Property(w => w.Status)
            .HasDefaultValue("Open");
    }
}
