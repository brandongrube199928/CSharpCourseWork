using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradebookApp
{
    public class Gradebook
    {
        // Class variable list to store grades
        private readonly List<double> grades = new(); // Calls the constructor for a blank list

        // Add a single grade
        public void AddGrade(double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentOutOfRangeException(nameof(grade), "Grade must be between 0 and 100.");

            grades.Add(grade);
        }


        // Overload method: add multiple grades at once
        public void AddGrade(IEnumerable<double> grades)
        {
            foreach (var g in grades)
                AddGrade(g);
        }
        // Compatibility method expected by Program.cs / course materials
        public void AddGrades(IEnumerable<double> grades)
        {
            AddGrade(grades);
        }

        // Get Average Method
        public double GetAverage() {
            if (grades.Count == 0) return 0;
            double total = 0;
            foreach (var g in grades)
                total += g;
            return total / grades.Count;
        }

        //Get Highest Grade Method
        public double GetHighest() => grades.Count == 0 ? 0 : grades.Max();

        // Get Lowest Grade Method
        public double GetLowest() => grades.Count == 0 ? 0 : grades.Min();
        
        // Get Count Method
        public int GetCount() => grades.Count;

        // Clear all grades method
        public void Clear() => grades.Clear();
    }
}
