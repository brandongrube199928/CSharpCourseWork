// SeedData.cs
using Microsoft.EntityFrameworkCore;

namespace MaintenanceTracker.WinForms;

public static class SeedData
{
    public static async Task EnsureCreatedAndSeedAsync(MaintenanceContext db)
    {
        try { await db.Database.MigrateAsync(); }
        catch { await db.Database.EnsureCreatedAsync(); }

        if (await db.Technicians.AnyAsync()) return;

        var techs = new[]
        {
            new Technician { Name = "Sarah Johnson", Department = "Electrical" },
            new Technician { Name = "James Lee",     Department = "Mechanical" },
            new Technician { Name = "Maria Hernandez", Department = "HVAC" },
            new Technician { Name = "Alex Patel",    Department = "Plumbing" },
        };
        db.Technicians.AddRange(techs);
        await db.SaveChangesAsync();

        var rnd = new Random(42);
        var start = DateTime.UtcNow.AddDays(-90);
        var end   = DateTime.UtcNow;

        var list = new List<WorkOrder>();
        for (int i = 0; i < 140; i++)
        {
            var t = techs[rnd.Next(techs.Length)];
            var req = RandomDate(rnd, start, end).Date.AddHours(rnd.Next(7,18));
            bool closed = rnd.NextDouble() < 0.75;
            DateTime? comp = null;
            double hours = Math.Round(1.5 + rnd.NextDouble() * 6.0, 1);

            if (closed)
            {
                comp = req.AddDays(rnd.Next(1,7)).AddHours(rnd.Next(0,8));
                if (comp > end) comp = end;
            }

            list.Add(new WorkOrder
            {
                TechnicianId = t.TechnicianId,
                RequestDate = req,
                CompletionDate = comp,
                Status = closed ? "Closed" : "Open",
                HoursWorked = hours
            });
        }

        db.WorkOrders.AddRange(list);
        await db.SaveChangesAsync();
    }

    private static DateTime RandomDate(Random rnd, DateTime from, DateTime to)
    {
        var range = to - from;
        return from + TimeSpan.FromSeconds(rnd.NextDouble() * range.TotalSeconds);
    }
}
