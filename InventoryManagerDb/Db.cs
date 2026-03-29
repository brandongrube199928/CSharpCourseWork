using System;
using System.Data;
using System.Data.SQLite;

namespace InventoryManagerDb
{
    public static class Db
    {
        private const string ConnStr = "Data Source=Inventory.db;Version=3;";

        public static void EnsureCreated()
        {
            using var conn = new SQLiteConnection(ConnStr);
            conn.Open();
			//Write a Query as a string that will create the Table if it does not exist.
			//PK should be an autoincrementing INT. Name and category are Text, not null
			//Price and Quantity are Not Null Integers. Price should be real
            string sql =
                @"CREATE TABLE IF NOT EXISTS Products(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Quantity INTEGER NOT NULL CHECK(Quantity >= 0),
                    Price REAL NOT NULL CHECK(Price >= 0)
                );";


            //Use this to execute your string which should be named sql
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public static DataTable GetAll()
        {
            using var conn = new SQLiteConnection(ConnStr);
            conn.Open();
			//Make a var that takes the result of a SQLiteDataAdatper and pass in the query and conn.
            using var da = new SQLiteDataAdapter("SELECT Id, Name, Category, Quantity, Price FROM Products ORDER BY Name;", conn);
            //Make a DataTable, Fill it with the contents of the var and return the table
            var table = new DataTable();
            da.Fill(table);
            return table;
        }

        public static DataTable Search(string? nameLike, string? categoryLike)
        {
            using var conn = new SQLiteConnection(ConnStr);
            conn.Open();

			//Create a string named sql for a select statement then create a var for the SQL command
            string sql = "SELECT Id, Name, Category, Quantity, Price FROM Products WHERE 1=1";
            using var cmd = new SQLiteCommand(conn);

            //Create some validation to add parameters if the name or category is unclear
            if (!string.IsNullOrEmpty(nameLike))
            {
                sql += " AND Name LIKE @n";
                cmd.Parameters.AddWithValue("@n", $"%{nameLike.Trim()}%");
            }
            if (!string.IsNullOrEmpty(categoryLike))
            {
                sql += " AND Category LIKE @c";
                cmd.Parameters.AddWithValue("@c", $"%{categoryLike.Trim()}%");
            }

            //Make the query order by name and execute. Use += on your string to concatenate more query parameters
            sql += " ORDER BY Name;";
            cmd.CommandText = sql;

            //Load the results into a DataTable and return the table
            using var da = new SQLiteDataAdapter(cmd);
            var table = new DataTable();
            da.Fill(table);
            return table;

        }

        public static void Insert(Product p)
        {
            
			using var conn = new SQLiteConnection(ConnStr);
            conn.Open();

            //Create a string named sql with an insert statement. 
            string sql = @"INSERT INTO PRoducts (Name, Category, Quantity, Price)
                            VALUES(@name, @cat, @qty, @price);";

            //Then make a command and add insert parameters
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", p.Name);
            cmd.Parameters.AddWithValue("@cat", p.Category);
            cmd.Parameters.AddWithValue("@qty", p.Quantity);
            cmd.Parameters.AddWithValue("@price", p.Price);

            //Use ExecuteNonQuery() for this one because it does not return anything
            cmd.ExecuteNonQuery();
        }

        public static void Update(Product p)
        {
            //This makes sure that you have a positive productID
			if (p.Id <= 0) throw new ArgumentException("Valid Id required to update.");
            using var conn = new SQLiteConnection(ConnStr);
            conn.Open();
            
			//Create a string named sql with an Update query
			string sql = @"UPDATE Products
                            SET Name=@name, Category=@cat, Quantity=@qty, Price=@price
                            WHERE Id=@id;";
			//Then create a command and add the update parameters
			using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", p.Name);
            cmd.Parameters.AddWithValue("@cat", p.Category);
            cmd.Parameters.AddWithValue("@qty", p.Quantity);
            cmd.Parameters.AddWithValue("@price", p.Price);
            cmd.Parameters.AddWithValue("@id", p.Id);
            //Use ExecuteNonQuery() to run this one
            cmd.ExecuteNonQuery();
        }

        public static void Delete(long id)
        {
            //No change needed, use this as an example for the SQL Syntax
			using var conn = new SQLiteConnection(ConnStr);
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Products WHERE Id=@id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
