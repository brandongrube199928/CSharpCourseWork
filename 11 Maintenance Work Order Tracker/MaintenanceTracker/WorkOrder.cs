// WorkOrder.cs NO CHANGE NEEDED!
using System;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceTracker.WinForms;

public class WorkOrder
{
    public int WorkOrderId { get; set; }

    public int TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletionDate { get; set; } // null if open

    [Required, MaxLength(12)]
    public string Status { get; set; } = "Open"; // "Open" or "Closed"

    [Range(0, double.MaxValue)]
    public double HoursWorked { get; set; } = 0.0;
}