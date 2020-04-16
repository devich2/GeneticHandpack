using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DKR
{
    public class Item: IEquatable<Item>
    {
        public string Name { get; private set; }
        public int Weight { get; private set; }
        public double Price { get; private set; }

        public Item(string Name, int Weight, double Price)
        {
            this.Name = Name;
            this.Weight = Weight;
            this.Price = Price;
        }

        public bool Equals(Item other) => (other.Name == Name);

        public override string ToString()
        {
            return $"{Name}, worth - {Price}, weight - {Weight}";
        }
    }
}
