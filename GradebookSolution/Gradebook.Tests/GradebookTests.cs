using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GradebookApp;

namespace Gradebook.Tests
{
    public class GradebookTests
    {
        // Successfully add a valid grade
        [Fact]
        // Fully qualified class reference used to avoid ambiguity and ensure the compiler
        // correctly resolves the type across projects and namespaces, even if the IDE
        // hasn’t fully recognized project references yet.
        // In this scenario, we form the direct link through Gradebook.App.Gradebook, which is the fully qualified name of the Gradebook class in the GradebookApp project.
        public void AddGrade__AddsValidGrade()
        {
            var book = new GradebookApp.Gradebook();
            book.AddGrade(90);
            Assert.Equal(1, book.GetCount());
        }

        // Reject an invalid grade
        [Fact]
        public void AddGrade__RejectsInvalidGrade()
        {
            var book = new GradebookApp.Gradebook();
            Assert.Throws<ArgumentOutOfRangeException>(() => book.AddGrade(150));
        }

        // Get the correct average grade
        [Fact]
        public void GetAverageGrade_ReturnsCorrectAverage()
        {
            var book = new GradebookApp.Gradebook();
            book.AddGrade(new List<double> { 80, 90, 100 });
            Assert.Equal(90, book.GetAverage(), 2);
        }

        // Validate highest and lowest
        [Fact]
        public void GetHighestandLowest_WorksCorrectly()
        {
            var book = new GradebookApp.Gradebook();
            book.AddGrade(new List<double> { 70, 85, 95 });
            Assert.Equal(95, book.GetHighest());
            Assert.Equal(70, book.GetLowest());
        }
    }
}