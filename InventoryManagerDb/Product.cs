using System;

namespace InventoryManagerDb
{
    public sealed class Product
    {
        //!!! No change to this file needed
		public long Id { get; init; }          // DB autoincrement
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public Product(string name, string category, int quantity, double price, long id = 0)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required.");
            if (quantity < 0) throw new ArgumentException("Quantity must be >= 0.");
            if (price < 0) throw new ArgumentException("Price must be >= 0.");

            Id = id;
            Name = name.Trim();
            Category = category.Trim();
            Quantity = quantity;
            Price = price;
        }
    }
}
