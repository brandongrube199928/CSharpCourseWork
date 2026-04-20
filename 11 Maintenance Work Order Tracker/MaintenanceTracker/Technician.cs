// Technician.cs NO CHANGE NEEDED!
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceTracker.WinForms;

public class Technician
{
    public int TechnicianId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(60)]
    public string Department { get; set; } = "General";

    public List<WorkOrder> WorkOrders { get; set; } = new();
}
