using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DKR
{
    public class GeneticAlgo : IGenetic
    {
        private readonly Random _rnd = new Random();
        public IEnumerable<Backpack> ChooseParents(IEnumerable<Backpack> backpacks)
        {
           var max = backpacks.Aggregate((x, y) => x.TotalWorth > y.TotalWorth ? x : y);
           int rnd = 0;
            while (backpacks.ElementAt(rnd = _rnd.Next(0, backpacks.Count())) == max) { }
           return new List<Backpack>(new Backpack[] 
           { 
               max,
               backpacks.ElementAt(rnd)
           });
        }

        public IEnumerable<Backpack> CreatePopulation(int count, int maxWeight, List<Item> items)
        {
            var population = new List<Backpack>();
            for(int i = 0; i < count; i++)
            {
                var backpack = new Backpack();
                while(backpack.TotalWeight < maxWeight)
                {
                    int index = _rnd.Next(0, items.Count - 1);
                    if (backpack.Items.Contains(items[index]))
                        continue;
                    if (backpack.TotalWeight + items[index].Weight > maxWeight)
                        break;
                    backpack.Items.Add(items[index]);
                }
                if (population.Contains(backpack)) { count++; }
                else population.Add(backpack);
            }
            return population;
        }

        public Backpack Improve(int maxWeight, Backpack backpack, List<Item> items)
        {
            var allowedItems = items.Except(backpack.Items);
            if (allowedItems.Count() == 0)
            {
                Console.WriteLine("Allready all items in backpack!.....");
            }
            else
            {
                var sorted = allowedItems.OrderByDescending(x => x.Price);
                bool improved = false;
                foreach(var item in sorted)
                {
                    if(item.Weight + backpack.TotalWeight <= maxWeight)
                    {
                        backpack.Items.Add(item);
                        Console.WriteLine($"Added {item.Name} with worth - {item.Price}, weight - {item.Weight}");
                        Console.WriteLine($"TotalWeight - {backpack.TotalWeight}, TotalWorth - {backpack.TotalWorth}");
                        Console.WriteLine("------------");
                        improved = true;
                    }
                }
                if (!improved) Console.WriteLine("----No improvement!----");

            }
            return backpack;
        }

        public Backpack Mutate(Backpack backpack, List<Item> items)
        {
            int index = _rnd.Next(0, backpack.Items.Count() - 1);
            var bitPresentation = backpack.Presentation(items);
            if (bitPresentation.ElementAt(index) == 0)
            {
                Console.WriteLine($"Adding {items[index].Name}");
                backpack.Items.Add(items[index]);
            }
            else
            {
                Console.WriteLine($"Removing {items[index].Name}");
                backpack.Items.Remove(items[index]);
            }

            return backpack;
        }

        public Backpack Reanimate(int maxWeight, Backpack backpack, List<Item> items)
        {
            var sorted = items.OrderBy(x => x.Weight);
            List<Item> usedItems = new List<Item>();
            int addIndex, removeIndex;
            while(backpack.TotalWeight > maxWeight)
            {
                addIndex = 0;
                removeIndex = 0;
                var bpSorted = backpack.Items.OrderBy(x => x.Price);

                while(removeIndex < items.Count && usedItems.Contains(bpSorted.ElementAt(removeIndex)) && backpack.Items.Contains(bpSorted.ElementAt(removeIndex)))
                {
                    removeIndex++;
                }
                if(removeIndex != items.Count)
                {
                    var minWorth = bpSorted.ElementAt(removeIndex);
                    Console.WriteLine($"Removing {minWorth.Name} with worth - {minWorth.Price}, weight - {minWorth.Weight}");
                    backpack.Items.Remove(minWorth);
                    usedItems.Add(minWorth);
                }
                else
                {
                    Console.WriteLine("Cant reanimate...");
                    break;
                }
               
                while(addIndex < items.Count && usedItems.Contains(sorted.ElementAt(addIndex)) && backpack.Items.Contains(sorted.ElementAt(addIndex)))
                {
                    addIndex++;
                }
                if (addIndex != items.Count)
                {
                    var minWeight = sorted.ElementAt(addIndex);
                    Console.WriteLine($"Adding {minWeight.Name} with worth - {minWeight.Price}, " +
                        $"weight - {minWeight.Weight}");
                    backpack.Items.Add(minWeight);
                    usedItems.Add(minWeight);
                }
                else
                {
                    Console.WriteLine("Difficult case, no adding, just continue removing items");
                }
                Console.WriteLine($"TotalWeight - {backpack.TotalWeight}, TotalWorth - {backpack.TotalWorth}");
                Console.WriteLine("------------");
            }
            return backpack;
        }

        public IEnumerable<Backpack> Selection(IEnumerable<Backpack> backpacks, List<Item> items)
        {
            int rnd = _rnd.Next(0, items.Count());
            Console.WriteLine($"---Random point for selection - {rnd + 1}");
            List<Backpack> descendants = new List<Backpack>();
            List<List<int>> bitPresents = new List<List<int>>(backpacks.Select(x => x.Presentation(items).ToList()));
            var lastBp = bitPresents.Last();
            foreach (var bp in bitPresents)
            {
                var newBit = bp.Take(rnd).Concat(lastBp.Skip(rnd)).ToList();
                var newBp = new Backpack();
                for(int i = 0; i < newBit.Count(); i++)
                {
                    if (newBit[i] == 1) newBp.Items.Add(items[i]);
                }
                lastBp = bp;
                descendants.Add(newBp);
            }
            return descendants;
        }
    }
}
