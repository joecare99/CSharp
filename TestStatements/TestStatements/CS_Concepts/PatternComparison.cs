using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.CS_Concepts;

public class PatternComparison
{
    private class MyClass
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public MyAddress? Address { get; set; }

    }
    class MyAddress
    {
        public string City { get; set; }
    }
    /// <summary>
    /// Demonstrates pattern matching with switch expressions.
    /// </summary>
    public static void PatternMatchingDemo()
    {
        object obj = 42;
        string result = obj switch
        {
            int i => $"Integer: {i}",
            string s => $"String: {s}",
            _ => "Unknown type"
        };
        Console.WriteLine(result); // Output: Integer: 42
    }


    public static void PatternMatchingDemo2()
    {
        // Example usage of type patterns
        MyClass myClass = new MyClass { Value = 10, Name = "Test" };
        myClass.Address = new MyAddress { City = "TestCity" };

        var obj = myClass;
        if (obj is { Address.City: "TestCity" } myObj)
        {
            Console.WriteLine($"Matched MyClass with Value: {myObj.Value}, Name: {myObj.Name}");
        }
        else
        {
            Console.WriteLine("Did not match MyClass with specified properties.");
        }
    }
    public static void Main()
    {
        // Example usage of pattern matching
        PatternMatchingDemo();
        PatternMatchingDemo2();
    }
}

