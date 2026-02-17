// Shell Module 2: Console Calculator
// Created By: Brandon Grube
// Redesign later by condensing into a 15 <-> 70 line program to iterate your own knowledge of C# and programming.
// Date Edited: 1/31/2926
using System.Globalization;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;

class Program {
    // Constant for tax rate
    private const decimal TaxRate = 0.055m;

    static void Main() {
        Console.WriteLine("Module 2: Console Calculator");

        // Loop Control Boolean
        bool continueRunning = true;

        //Last result tracking
        double lastResult = 0;
        bool hasLastResult = false;

        //Operation counter dictionary
        Dictionary<string, int> operationCount = new()
        {
            ["add"] = 0,
            ["sub"] = 0,
            ["mul"] = 0,
            ["div"] = 0,
            ["avg"] = 0,
            ["tax"] = 0
        };

        // Do while loop to handle user input
        do {
            Console.WriteLine("Operations: +, -, *, /, avg, tax, and exit");
            Console.Write("Enter an operator: ");

            string choice = Console.ReadLine();

            switch (choice) {

                case "+":
                    lastResult = Add();
                    operationCount["add"]++;
                    hasLastResult = true;
                    break;

                case "-":
                    lastResult = Subtract();
                    operationCount["sub"]++;
                    hasLastResult = true;
                    break;

                case "*":
                    lastResult = Multiply();
                    operationCount["mul"]++;
                    hasLastResult = true;
                    break;

                case "/":
                    lastResult = Divide();
                    operationCount["div"]++;
                    hasLastResult = true;
                    break;

                case "avg":
                    lastResult = Average();
                    operationCount["avg"]++;
                    hasLastResult = true;
                    break;

                case "tax":
                    CalculateTax();
                    operationCount["tax"]++;
                    hasLastResult = false;
                    break;

                case "exit":
                    Console.WriteLine("Thank you for using our program. Have a wonderful day or night.");
                    return;

                default:
                    Console.WriteLine("Invalid input; please try again.");
                    break;
            }

            // Terminal operator example to print the last result
            string lastDisplay = hasLastResult ? lastResult.ToString("G", CultureInfo.InvariantCulture) : "N/A";
            Console.WriteLine($"Last numeric result: {lastDisplay}");

            // Control to break out of the program
            Console.Write("Perform another operation? (y/n): ");
            string again = Console.ReadLine().Trim().ToLowerInvariant();
            continueRunning = (again == "y" || again == "yes");

        } while (continueRunning);

        // Display summary with a for loop
        Console.WriteLine("==== Session Summary =====");
        int totalOps = 0;
        foreach (var kvp in operationCount)
            totalOps += kvp.Value;

        Console.WriteLine($"Total operations: {totalOps}");
        Console.WriteLine($"Operations Breakdown: ");

        var keys = new List<string>(operationCount.Keys);
        for (int i = 0; i < keys.Count; i++)
            Console.WriteLine($"{keys[i]}: {operationCount[keys[i]]}");

        Console.WriteLine("Thank you for using the console calculator");
    }

    // Helper Methods

    // Method to validate user entered doubles
    private static double ReadDouble(string prompt) {
        while (true) {
            Console.Write($"{prompt}: ");
            string input = Console.ReadLine();
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                return value;
            Console.WriteLine("Invalid number. Please try again.");
        }
    }

    // Addition
    public static double Add() {
        double a = ReadDouble("Enter the first number.");
        double b = ReadDouble("Enter the second number.");
        double result = a + b;
        Console.WriteLine($"Result: {a} + {b} = {result}");
        return result;
    }

    // Validate user entered decimals
    private static decimal ReadDecimal(string prompt) {
        while (true){
            Console.Write($"{prompt}: ");
            string input = Console.ReadLine();
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                return value;
            Console.WriteLine("Invalid number. Please try again.");
        }
    }


    // Subtraction
    private static double Subtract() {
        double a = ReadDouble("Enter the first number");
        double b = ReadDouble("Enter the second number");
        double result = a - b;
        Console.WriteLine($"Result: {a} - {b} = {result}");
        return result;
    }

    // Multiply
    private static double Multiply() {
        double a = ReadDouble("Enter the first number");
        double b = ReadDouble("Enter the second number");
        double result = a * b;
        Console.WriteLine($"Result: {a} + {b} = {result}");
        return result;
    }

    // Division
    private static double Divide() {
        double a = ReadDouble("Enter the first number");
        double b;  // Created variable, but not assigning right away
        do
        {
            b = ReadDouble("Enter the second number (no zeroes)");
            if (b == 0)
                Console.WriteLine("Cannot divide by zero");
        } while (b == 0);

        double result = a / b;
        Console.WriteLine($"Result: {a} / {b} = {result}");
        return result;
    }

    // Average
    private static double Average() {
        double a = ReadDouble("Enter first number");
        double b = ReadDouble("Enter the second number");
        double result = (a + b) / 2.0;
        Console.WriteLine($"Average of {a} and {b} is {result}");
        return result;
    }

    // Sales Tax Calculation
    private static void CalculateTax() {
        decimal amount = ReadDecimal("Enter item cost");
        decimal tax = amount * TaxRate;
        decimal total = amount + tax;
        Console.WriteLine($"Tax ({TaxRate}:) {tax:C}");
        Console.WriteLine($"Total with tax: {total:C}");
    }

}

