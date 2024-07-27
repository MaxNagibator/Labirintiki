using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace labirint
{
    public class Person
    {
        public string Name { get; set; }
        public Car Auto { get; set; }

        public Person(string name)
        {
            Name = name;
        }
    }
}
