namespace WTTrade.Models;

public class Inventory
{
    private readonly Dictionary<Item, int> _dictInventory;

    public Inventory(Dictionary<Item, int> dictInventory)
    {
        _dictInventory = dictInventory;
    }
    
    public override string ToString()
    {
        var setItems = new SortedSet<Item>(_dictInventory.Keys, new SortItemComparer());
        List<string> stringItems = setItems.Select(item => item + " - в кол-ве " + _dictInventory[item]).ToList();

        return string.Join("\n", stringItems);
    }
    
    private class SortItemComparer : IComparer<Item>
    {
        public int Compare(Item? item1, Item? item2)
        {
            int sortingIndex1 = item1!.SortIndex;
            int sortingIndex2 = item2!.SortIndex;

            if (sortingIndex1 == sortingIndex2)
            {
                string name1 = item1.Name;
                string name2 = item2.Name;
                
                return string.Compare(name1, name2, StringComparison.Ordinal);
            }
            
            return sortingIndex1.CompareTo(sortingIndex2);
        }
    }
}