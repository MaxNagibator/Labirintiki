using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace labirint
{
    public class Car
    {
        private int _id;
        public string Name { get; set; }
        public string Color { get; set; }
        public const int ChisloKoles = 4;

        public Car(int id, string name)
        {
            _id = id;
            Name = name;
        }
    }
}
