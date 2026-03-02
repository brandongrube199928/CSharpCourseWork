using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;

namespace _5_IT_Support_Ticket_Manager
{
    public class TicketManager
    {
        // List to hold collection of tickets
        private readonly List<Ticket> _tickets = new();

        // Void to add tickets
        public void AddTicket(Ticket t)
        {
            // Validate Null
            if (t is null) throw new ArgumentNullException(nameof(t));
            // Validate Duplicates
            if (FindTicket(t.Id) is not null)
                throw new InvalidOperationException($"A ticket with ID '{t.Id}' already exists.");
            _tickets.Add(t);
        }

        //Bool to remove tickets
        public bool RemoveTicket(string id)
        {
            var t = FindTicket(id);
            if (t is null) return false;
            _tickets.Remove(t);
            return true;
        }

        // Ticket method to find tickets
        public Ticket FindTicket(string id)
        {
            foreach (var t in _tickets)
                if (string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase))
                    return t;
            return null;
        }

        // Void to display all tickets
        public void DisplayAllTickets()
        {
            if (_tickets.Count == 0)
            {
               Console.WriteLine("No tickets to display.");
                return;
            }

            Console.WriteLine("\n--- Ticket List ---");
            foreach (var t in _tickets)
                Console.WriteLine(t.GetSummary());
        }

        // Int method to get a count of tickets
        public int GetOpenCount()
        {
            int count = 0;
            foreach (var t in _tickets)
                if (!string.Equals(t.Status, "Closed", StringComparison.OrdinalIgnoreCase))
                    count++;
            return count;
        }


        /// --------------------- CSV Persistence ---------------------
        /// Header: ID,Description,Priority,Status,DateCreated

        // Save Tickets Void Method
        public void SaveTickets(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);

            using var sw = new StreamWriter(path, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            sw.WriteLine("Id,Description,Priority,Status,DateCreated");
            foreach (var t in _tickets)
            {
                string line = string.Join(",",
                    CsvEscape(t.Id),
                    CsvEscape(t.Description),
                    CsvEscape(t.Priority),
                    CsvEscape(t.Status),
                    CsvEscape(t.DateCreated.ToString("o", CultureInfo.InvariantCulture)) // ISO 8601
                    );
                sw.WriteLine(line);
            }
        }


        // Load Tickets Void Method
        public void LoadTickets(string path)
        {
            using var sr = new StreamReader(path);

            // Clear current list before loading the file
            _tickets.Clear();

            string? header = sr.ReadLine();
            if (header is null)
                throw new InvalidDataException("File is empty");

            int lineNo = 1, loaded = 0, skipped = 0;

            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) { continue; }

                try
                {
                    var cols = CsvParse(line);
                    if (cols.Count != 5)
                        throw new InvalidDataException($"Expected 5 columns but found {cols.Count}.");

                    string id = cols[0];
                    string description = cols[1];
                    string priority = cols[2];
                    string status = cols[3];
                    string created = cols[4];

                    if (!DateTime.TryParse(created, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var when))
                        throw new InvalidDataException("Invalid DateCreated");

                    var t = new Ticket(id, description, priority, status);
                    // Override auto-created date with persisted value
                    typeof(Ticket).GetProperty(nameof(Ticket.DateCreated))!
                        .SetValue(t, when);

                    AddTicket(t);
                    loaded++;
                }
                catch (Exception ex)
                {
                    skipped++;
                    Console.WriteLine($"Skipped line {lineNo}: {ex.Message}");
                }
            }
            Console.WriteLine($"Load Complete. Loaded: {loaded}, Skipped: {skipped}");
        }

        // -------------------------- Minimal CSV Helpers --------------------------
        // String Method for Escaping
        private static string CsvEscape(string input)
        {
            bool needsQuotes = input.Contains(',') || input.Contains('"') || input.Contains('\n');
            if (!needsQuotes) return input;
            return "\"" + input.Replace("\"", "\"\"") + "\"";
        }


        // List Method to Parse the CSV File
        private static List<string> CsvParse(string line)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        // Escaped Quotes
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i++; // Skip the next quote
                        }
                        else
                        {
                            inQuotes = false; // End of quoted field
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }

                }
                else 
                {
                    if (c == ',')
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if(c == '"')
                    {
                        inQuotes = true; // Start of quoted field
                    }
                    else
                    {
                         sb.Append(c);
                    }
                }
            }

            result.Add(sb.ToString());
            return result;
        }
    }
}
