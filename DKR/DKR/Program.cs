using System;
using System.Collections.Generic;

namespace DKR
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HEELO");
            GeneticAlgo gt = new GeneticAlgo();
            List<Item> items = new List<Item>();
            Console.WriteLine("HEELO");
            items.AddRange(new Item[]
            {
                new Item("Bag", 4, 2),
                new Item("Camp", 5, 3),
                new Item("Book", 1, 2),
                new Item("Cup", 1, 1),
                new Item("Medic", 2, 4),
                new Item("Shoes", 3, 2),
                new Item("Navigator", 1, 3),
                new Item("Carpet", 3 , 2),
                new Item("Food", 5 , 4),
                new Item("Game", 2, 1),
                new Item("Clock", 1, 2)
            });
            GeneticSolver gs = new GeneticSolver(10, 15, items, gt);
            gs.Iterate(2);
        }
    }
}
