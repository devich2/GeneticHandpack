using System;
using System.Collections.Generic;
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
                while( backpack.TotalWorth < maxWeight)
                {
                    int index = _rnd.Next(0, items.Count - 1);
                    if (backpack.Items.Contains(items[index]))
                        continue;
                    if (backpack.TotalWeight + items[i].Weight > maxWeight)
                        break;
                    backpack.Items.Add(items[index]);
                }
                if (population.Contains(backpack)) { }
                else population.Add(backpack);
            }
            return population;
        }

        public Backpack Improve(int maxWeight, Backpack backpack, List<Item> items)
        {
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
            int addIndex = 0;
            while(backpack.TotalWeight > maxWeight)
            {
                var minWorth = backpack.Items.Aggregate((x, y) => x.Price < y.Price ? x : y);
                Console.WriteLine($"Removing {minWorth.Name}");
                backpack.Items.Remove(minWorth);
            }
            return backpack;
        }

        public IEnumerable<Backpack> Selection(IEnumerable<Backpack> backpacks, List<Item> items)
        {
            int rnd = _rnd.Next(0, items.Count());
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
