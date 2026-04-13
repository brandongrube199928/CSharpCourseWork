using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Customer
{
    //Create a public int for CustomerID and give it Get/Set methods
	public int CustomerID { get; set; }

    //Create a public string for Name. Give it Get/Set and initialize to empty. 
    //Make it required with a max length of 120 chars
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    //Create a public string for Email. Give it Get/Set and initialize it to empty
    //Make it required with a max length of 120 chars
    [Required, MaxLength(120)]
    public string Email { get; set; } = string.Empty;


    //Create a public List of Orders. Give it Get/Set and initialize new
    public List<Order> Orders { get; set; } = new();
}
