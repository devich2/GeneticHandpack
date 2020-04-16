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
        private List<Backpack> _population = new List<Backpack>();

        private double _mutPoss;
        private List<Item> _baseItems;
        private int _initialCount;
        private int _maxWeight;
        private IGenetic _genetic;

        public GeneticSolver(int initialCount, int maxWeight, List<Item> items, IGenetic genetic)
        {
            _initialCount = initialCount;
            _maxWeight = maxWeight;
            _baseItems = items;
            _genetic = genetic;
            _mutPoss = _rnd.NextDouble();
        }

        public bool isValid(Backpack pack) => pack.TotalWeight <= _maxWeight;
        public bool isMutable(double poss) => poss > _mutPoss;

        public int getMinWorthIndex(List<int> usedIndexes)
        {
            int minIndex = 0;
            double minValue = int.MaxValue;
            for(int i = 0; i < _population.Count(); i++)
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

        private void printPack(string info, Backpack pack)
        {
            Console.WriteLine(info + $"{itemsPresentation()} \n" +
                $"{string.Join("\t  ", pack.Presentation(_baseItems))}" + $"\t TotalWeight - {pack.TotalWeight}, " + $"TotalWorth - {pack.TotalWorth}");
        }

        private void printPackList(string info, List<Backpack> packList)
        {
            Console.WriteLine(info + $"\n {itemsPresentation()} \n" +
                $"{string.Join("\n", packList.Select(x => string.Join("\t   ", x.Presentation(_baseItems)) + $"\t TotalWeight - {x.TotalWeight}, " + $"TotalWorth - {x.TotalWorth}"))}");
        }

        private string itemsFullPresentation => string.Join("\n", _baseItems);
        public void Iterate(int itrCount)
        {
            Console.WriteLine("Base items -\n" + itemsFullPresentation);

            _population = _genetic.CreatePopulation(_initialCount, _maxWeight, _baseItems).ToList();

            for (int i = 0; i < itrCount; i++)
            {
                Console.WriteLine($"-----------------------------------------\nIteration #{i + 1}----------------------------------------------------");
                printPackList($"\nPopulation: ", _population);

                var parents = _genetic.ChooseParents(_population).ToList();

                printPackList($"\nChosen parents for selection: ", parents);

                Console.WriteLine("\n-----------------------SELECTION------------------------------------");
                var descendants = _genetic.Selection(parents, _baseItems).ToList();
                printPackList($"Descendants - \n", descendants);
                Console.WriteLine("------------------------------------------------------\n");
              
                Console.WriteLine($"\nMutation possibility - {_mutPoss}");
                int num = 0;

                foreach(var des in descendants)
                {
                    printPack($"----------------------\nDescendant #{++num}\n", des);
                   
                    double poss = _rnd.NextDouble();

                    Console.WriteLine($"Generated random poss for mutation - {poss} > {_mutPoss}: {poss > _mutPoss}");

                    if (isMutable(poss))
                    {
                        Console.WriteLine("Mutating...........");
                        _genetic.Mutate(des, _baseItems);

                        printPack($"-----------------------\nDescendant after mutation\n", des);
                    }

                    if (!isValid(des))
                    {
                        Console.WriteLine($"------------------\nNeeds reanimation(TotalWeight - {des.TotalWeight})");
                        _genetic.Reanimate(_maxWeight, des, _baseItems);

                        printPack($"-----------------------\nDescendant after reanimation\n", des);
                    }
                    Console.WriteLine();
                }

                int imprvIndex = _rnd.Next(0, descendants.Count());
                Console.WriteLine($"------------------------------------\nImproving descendant# {imprvIndex + 1}");

                _genetic.Improve(_maxWeight, descendants[imprvIndex], _baseItems);
                printPack($"------------------------------------\nDescendant #{imprvIndex + 1} after local improvement\n", descendants[imprvIndex]);

                Console.WriteLine("----------------------------Updating population-----------------------------");
                List<int> replacedIndexes = new List<int>();
                num = 0;

                foreach(var des in descendants)
                {
                    Console.WriteLine($"Descendant #{++num}");
                    if(!_population.Contains(des))
                    {
                        var replaceableIndex = getMinWorthIndex(replacedIndexes);
                        printPack($"Replaceable person #{replaceableIndex + 1} - \n", _population[replaceableIndex]);
                        printPack($"New person - \n", des);

                        _population[replaceableIndex] = des;
                        replacedIndexes.Add(replaceableIndex);
                        printPackList($"-------------------------------------------------------\n\nPopulation after replacement #{num}: ", _population);
                    }
                    else
                    {
                        Console.WriteLine("Descendant already in population....");
                    }
                  
                    Console.WriteLine("-------------------------------------------------------\n");
                }
            }
            printPack("--------------\nBest solution - \n", _population.Aggregate((x, y) => x.TotalWorth >= y.TotalWorth ? x : y));
        }
    }
}
