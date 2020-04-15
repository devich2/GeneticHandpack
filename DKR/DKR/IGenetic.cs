using System;
using System.Collections.Generic;
using System.Text;

namespace DKR
{
    public interface IGenetic
    {

        IEnumerable<Backpack> ChooseParents(IEnumerable<Backpack> backpacks);
        IEnumerable<Backpack> Selection(IEnumerable<Backpack> parents, List<Item> items);

        IEnumerable<Backpack> CreatePopulation(int count, int maxWeight, List<Item> items);

        Backpack Reanimate(int maxWeight, Backpack backpack, List<Item> items);

        Backpack Mutate(Backpack backpack, List<Item> items);

        Backpack Improve(int maxWeight, Backpack backpack, List<Item> items);
    }
}
