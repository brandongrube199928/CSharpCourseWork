using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace EmployeeDirectoryManager
{
    public sealed class EmployeeManager
    {
        //Create a public binding list of Employee objects and initialize as new. It will need a get; method
        public BindingList<Employee> Employees { get; } = new();

        // Add with validation (unique Id)
        public void AddEmployee(Employee e)
        {
            //Create an if statement to look for duplicate employees. Throw an exception if there is one
			if (Employees.Any(x => string.Equals(x.Id, e.Id, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"An employee with ID '{e.Id}' already exists.");
            //Then add the employee to the list
            Employees.Add(e);

        }

        // Update by Id (replace fields)
        public void UpdateEmployee(Employee updated)
        {
            //Create a var and check the list for the Employee's Id. If the result is null, throw an exception.
            var exists = Employees.FirstOrDefault(x => string.Equals(x.Id, updated.Id, StringComparison.OrdinalIgnoreCase));
            if (exists is null)
                throw new InvalidOperationException($"Employee '{updated.Id}' not found.");

            //If not null, assign the employee variables here-
            exists.FullName = updated.FullName;
            exists.Department = updated.Department;
            exists.Role = updated.Role;
            exists.Salary = updated.Salary;
            exists.HireDate = updated.HireDate;

        }

        // Delete by Id
        public bool RemoveEmployee(string id)
        {
            //Check if the Employee exists and store that result in a var
            var exists = Employees.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
            //If the var is null, return false
            if (exists is null) return false;
            //If the var is not null, Remove the employee and return true
            Employees.Remove(exists);
            return true;

        }

		//!!! Helper Methods for the CSV. Do not modify anything below this line
        // -------- Persistence (CSV) --------
        public void SaveToCsv(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)) ?? ".");
            using var sw = new StreamWriter(path, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            sw.WriteLine("Id,FullName,Department,Role,Salary,HireDate");
            foreach (var e in Employees)
                sw.WriteLine(e.ToCsv());
        }

        public void LoadFromCsv(string path)
        {
            using var sr = new StreamReader(path, Encoding.UTF8);
            Employees.Clear();

            string? header = sr.ReadLine(); // skip header
            if (header is null) throw new InvalidDataException("File is empty.");

            int line = 1, loaded = 0, skipped = 0;
            while (!sr.EndOfStream)
            {
                string? row = sr.ReadLine();
                line++;
                if (string.IsNullOrWhiteSpace(row)) continue;

                try
                {
                    var e = Employee.FromCsv(row);
                    // Enforce unique ID on load as well
                    if (Employees.Any(x => string.Equals(x.Id, e.Id, StringComparison.OrdinalIgnoreCase)))
                        throw new InvalidOperationException($"Duplicate ID '{e.Id}' in file.");
                    Employees.Add(e);
                    loaded++;
                }
                catch (Exception ex)
                {
                    skipped++;
                    // For classroom apps, a console note is fine; in prod you'd log this.
                    Console.WriteLine($"Skipped line {line}: {ex.Message}");
                }
            }
            Console.WriteLine($"Load complete. Loaded: {loaded}, Skipped: {skipped}");
        }
    }
}
