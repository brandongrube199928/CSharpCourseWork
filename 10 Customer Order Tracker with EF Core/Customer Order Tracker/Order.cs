using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    //Create a public int for OrderID, give it Get/Set
	public int OrderID { get; set; }
    //Create a public DateTime for OrderDate. It will need Get/Set and initialize with UtcNow
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    //Create a public Double for TotalAmount. Give it Get Set
    //Give it a Range that goes from 0 to the Max Value of a Double. 
    //Look up the function instead of trying to write it out
    [Range(0, double.MaxValue)]
    public double TotalAmount { get; set; }


    //Create a public int of CustomerId with Get/Set
    //Create a public Customer with Get/Set
    //Then set these as the Foreign Key for Customer
    [ForeignKey(nameof(Customer))]
    public int CustomerID { get; set; }
    public Customer? Customer { get; set; }

}