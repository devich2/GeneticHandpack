using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DKR
{
    public class GeneticSolver
    {
        private Random _rnd = new Random();
        private List<Backpack> _population;

        private readonly double _mutPoss;
        private readonly List<Item> _baseItems;
        private readonly int _initialCount;
        private readonly int _maxWeight;
        private readonly IGenetic _genetic;

        public GeneticSolver(int initialCount, int maxWeight, List<Item> items, IGenetic genetic)
        {
            _initialCount = initialCount;
            _maxWeight = maxWeight;
            _baseItems = items;
            _genetic = genetic;
            _mutPoss = _rnd.NextDouble();
            _population = new List<Backpack>();
        }

        public bool isValid(Backpack pack) => pack.TotalWorth <= _maxWeight;
        public bool isMutable(double poss) => poss > _mutPoss;

        public int getMinWorthIndex(List<int> usedIndexes)
        {
            int minIndex = 0;
            double minValue = int.MaxValue;
            for(int i = 0; i< _population.Count(); i++)
            {
                double worth = _population[i].TotalWorth;
                if (worth < minValue && (!usedIndexes.Contains(i)))
                {
                    minValue = worth;
                    minIndex = i;
                }
            }
            return minIndex;
        }

        private string itemsPresentation() => string.Join("\t", _baseItems.Select(x=>x.Name));

        public void Iterate(int itrCount)
        {
            _population = _genetic.CreatePopulation(_initialCount, _maxWeight, _baseItems).ToList();
            Console.WriteLine("HERE");
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine($"Iteration #{i + 1}");
                var parents = _genetic.ChooseParents(_population).ToList();
                Console.WriteLine($"\nChosen parents: " +
                    $"\n {itemsPresentation()} \n{string.Join("\n", parents.Select(x=>x.Presentation(_baseItems)))}");
                var descendants = _genetic.Selection(parents, _baseItems).ToList();
               // Console.WriteLine($"\nDescendants before Mutation: \n{string.Join("\n", descendants)}");

                Console.WriteLine($"\nMutation possibility - {_mutPoss}");
                foreach(var des in descendants)
                {
                    Console.WriteLine($"Descendant\n {itemsPresentation()}\n{des.Presentation(_baseItems)}");
                   
                    double poss = _rnd.NextDouble();
                    Console.WriteLine($"Generated poss - {poss} > {_mutPoss}: {poss > _mutPoss}");
                    if (isMutable(poss))
                    {
                        Console.WriteLine("Mutating...........");
                        _genetic.Mutate(des, _baseItems);
                    }

                    if (!isValid(des))
                    {
                        Console.WriteLine("Needs reanimation");
                        _genetic.Reanimate(_maxWeight, des, _baseItems);
                    }
                }

                int imprvIndex = _rnd.Next(0, descendants.Count());
                _genetic.Improve(_maxWeight, descendants[imprvIndex], _baseItems);
                List<int> replacedIndexes = new List<int>();
                foreach(var des in descendants)
                {
                    if(!_population.Contains(des))
                    {
                        var replaceableIndex = getMinWorthIndex(replacedIndexes);
                        Console.WriteLine($"{itemsPresentation()}\nReplaceable person - \n{_population[replaceableIndex].Presentation(_baseItems)}");
                        Console.WriteLine($"New person - \n{des.Presentation(_baseItems)}");
                        _population[replaceableIndex] = des;
                        replacedIndexes.Add(replaceableIndex);
                    }
                    else
                    {
                        Console.WriteLine("Already in population....");
                    }
                }
                Console.ReadKey();
            }
        }
    }
}
