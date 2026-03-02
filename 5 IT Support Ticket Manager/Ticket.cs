using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5_IT_Support_Ticket_Manager
{
    public class Ticket
    {
        // Allowed values (kept as strings to match the assignment spec)
        public static readonly string[] AllowedPriorities = { "Low", "Medium", "High" };
        public static readonly string[] AllowedStatuses   = { "Open", "In Progress", "Closed" };

		//Private variables initialized and named according to C# guidelines
        private string _id = "";
        private string _description = "";
        private string _priority = "Low";
        private string _status = "Open";

        public string Id
        {
            get => _id;
            set => _id = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("ID cannot be empty.")
                : value.Trim();
        }

        public string Description
        {
            get => _description;
			set => _description = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Description cannot be empty.")
                : value.Trim();
        }

        public string Priority
        {
            get => _priority;
			//This one is a freebie, use it for help below
            set
            {
                var v = (value ?? "").Trim();
                if (Array.IndexOf(AllowedPriorities, v) < 0)
                    throw new ArgumentException($"Priority must be one of: {string.Join(", ", AllowedPriorities)}");
                _priority = v;
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                var v = (value ?? "").Trim();
                if (Array.IndexOf(AllowedStatuses, v) < 0)
                    throw new ArgumentException($"Status must be one of: {string.Join(", ", AllowedStatuses)}");
                _status = v;
            }
        }
		
		//Gifting you this one, it's C#'s way to get the current DateTime from the system
        public DateTime DateCreated { get; private set; } = DateTime.UtcNow;

        // Constructors
        //Default Constructor
		public Ticket() { }

        
        public Ticket(string id, string description, string priority, string status)
        {
            Id = id;
            Description = description;
            Priority = priority; // Will validate through the property setter
            Status = status;     // Will validate through the property setter
            DateCreated =DateTime.UtcNow;
        }

        // Processing Methods
        public void CloseTicket() => Status = "Closed";
        public void ReopenTicket() => Status = "Open";

        public string GetSummary() =>
            $"[{Id}] ({Priority}) \"{Description}\" | Status: {Status} | Created: {DateCreated:yyyy-MM-dd}";

        //make a public void called CloseTicket() that sets Status to "Closed"


        //make a public void called ReopenTicket() that sets Status to "Open"


        //make a public string that returns all public variables in the following format:
        //[T1001] (High) - "Printer not working" | Status: Open | Created: 2025-10-29
    }
}
