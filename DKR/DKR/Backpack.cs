using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DKR
{
    public static class Extension
    {
        public static IEnumerable<int> Presentation(this Backpack pack, List<Item> items) =>
            items.Select(x => pack.Items.Contains(x) ? 1 : 0);
    }
    public class Backpack : IEquatable<Backpack>
    {
        public static List<Item> PossItems { get; set; } = new List<Item>();
        public Backpack()
        {
            Items = new List<Item>();
        }
        public Backpack(List<Item> items)
        {
            Items = items;
        }

        public List<Item> Items { get; }
        public double TotalWorth => Items.Sum(x => x.Price);

        public double TotalWeight => Items.Sum(x => x.Weight);

        public bool Equals(Backpack other) => Items.TrueForAll(it => other.Items.Contains(it)) && Items.Count == other.Items.Count;
    }
}
