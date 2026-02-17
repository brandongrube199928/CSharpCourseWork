

namespace GradebookApp
{
    internal static class Program
    {
        public static void Main()
        {
            // Instantiate an instance of Gradebook
            var book = new Gradebook();

            Console.WriteLine("== Gradebook Utility ==");

            bool running = true;

            // While loop to handle the user input continuously
            while (running)
            {
                Console.WriteLine("\nMenu: ");
                Console.WriteLine("1. Add a grade");
                Console.WriteLine("2. View summary");
                Console.WriteLine("3. Clear all grades");
                Console.WriteLine("4. Exit");
                Console.Write("Choose: ");

                // String to capture user input
                string choice = Console.ReadLine();
            
                switch (choice)
                {
                    // Case 1: Add grades
                    case "1":
                        AddGrades(book);
                        break;
                    // Case 2: Show summary
                    case "2":
                        ShowSummary(book);
                        break;
                    // Case 3: Clear grades
                    case "3":
                        book.Clear();
                        break;
                    // Case 4: Exit the program
                    case "4":
                        Console.WriteLine("Goodbye cruel world!");
                        running = false;
                        break;
                    // Default case: Invalid option
                    default:
                        Console.WriteLine("Invalid option; try again.");
                        break;
                }
            }
        }

        // Method to add grades from user input into the gradebook
        private static void AddGrades(Gradebook book)
        {
            // Get user input (reject empty strings)
            Console.Write("Enter grades separated by spaces: ");
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) return;


            // foreach to create a list from user input
            var grades = new List<double>();
            foreach(var part in input.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (double.TryParse(part, out double g))
                    grades.Add(g);
                else
                    Console.WriteLine($"'{part}' is not a valid number");

            }


            // Validate and add grades
            try
            {
                book.AddGrades(grades);
                Console.WriteLine($"{grades.Count} grade(s) added.");
            }

            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Method to print a Grade Summary
        private static void ShowSummary(Gradebook book)
        {
            Console.WriteLine("\n== Grade Summary ==");
            Console.WriteLine($"Grades Entered: {book.GetCount()}");
            Console.WriteLine($"Highest Grade: {book.GetHighest():F2}");
            Console.WriteLine($"Lowested Grade: {book.GetLowest():F2}");
            Console.WriteLine($"Average Grade: {book.GetAverage():F2}");

            
        }
    }
}
