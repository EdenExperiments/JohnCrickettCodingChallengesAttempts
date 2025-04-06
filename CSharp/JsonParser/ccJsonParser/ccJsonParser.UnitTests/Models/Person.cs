using System;
using System.Collections.Generic;

namespace ccJsonParser.Tests.Models
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public Address Address { get; set; }
        public List<string> Tags { get; set; }
        public double[] Numbers { get; set; }
        public NestedData Nested { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }

    public class NestedData
    {
        public int[] Array { get; set; }
        public Dictionary<string, string> Object { get; set; }
    }
} 