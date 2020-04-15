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
            _population = new List<Backpack>();
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

        private void printPack(Backpack pack)
        {
           Console.WriteLine($"{itemsPresentation()} \n{string.Join("\t  ", pack.Presentation(_baseItems))}");
        }

        private void printPackList(List<Backpack> packList)
        {
            Console.WriteLine($"\n {itemsPresentation()} \n{string.Join("\n", packList.Select(x => string.Join("\t   ", x.Presentation(_baseItems))))}");
        }

        public void Iterate(int itrCount)
        {

            _population = _genetic.CreatePopulation(_initialCount, _maxWeight, _baseItems).ToList();

            Console.WriteLine($"\nInitial population: ");
            printPackList(_population);


            for (int i = 0; i < itrCount; i++)
            {
                Console.WriteLine($"-----------------------------------------\nIteration #{i + 1}");
                var parents = _genetic.ChooseParents(_population).ToList();

                Console.WriteLine($"\nChosen parents: ");
                printPackList(parents);

                var descendants = _genetic.Selection(parents, _baseItems).ToList();
              
                Console.WriteLine($"\nMutation possibility - {_mutPoss}");
                int num = 0;

                foreach(var des in descendants)
                {
                    Console.WriteLine($"----------------------\nDescendant #{++num}\n");
                    printPack(des);
                   
                    double poss = _rnd.NextDouble();
                    Console.WriteLine($"Generated random poss for mutation - {poss} > {_mutPoss}: {poss > _mutPoss}");

                    if (isMutable(poss))
                    {
                        Console.WriteLine("Mutating...........");
                        _genetic.Mutate(des, _baseItems);

                        Console.WriteLine($"-----------------------\nDescendant after mutation\n");
                        printPack(des);

                    }

                    if (!isValid(des))
                    {
                        Console.WriteLine($"------------------\nNeeds reanimation(Weight - {des.TotalWeight})");
                        _genetic.Reanimate(_maxWeight, des, _baseItems);

                        Console.WriteLine($"-----------------------\nDescendant after reanimation\n");
                        printPack(des);

                    }
                    Console.WriteLine();
                }
                int imprvIndex = _rnd.Next(0, descendants.Count());
                Console.WriteLine($"------------------------------------\nImproving descendant# {imprvIndex + 1}");

                _genetic.Improve(_maxWeight, descendants[imprvIndex], _baseItems);
                List<int> replacedIndexes = new List<int>();
                Console.WriteLine("----------------------------Updating population-----------------------------");
                num = 0;

                foreach(var des in descendants)
                {
                    Console.WriteLine($"Descendant #{++num}");
                    if(!_population.Contains(des))
                    {
                        var replaceableIndex = getMinWorthIndex(replacedIndexes);
                        Console.WriteLine($"{itemsPresentation()}\nReplaceable person #{replaceableIndex} - \n");
                        printPack(_population[replaceableIndex]);

                        Console.WriteLine($"New person - \n");
                        printPack(des);

                        _population[replaceableIndex] = des;
                        replacedIndexes.Add(replaceableIndex);

                        Console.WriteLine("-------------------------------------------------------\n");
                        Console.WriteLine($"\nPopulation after replacement #{num}: ");
                        printPackList(_population);
                    }
                    else
                    {
                        Console.WriteLine("Descendant already in population....");
                    }
                  
                    Console.WriteLine("-------------------------------------------------------\n");
                }
            }
        }
    }
}
