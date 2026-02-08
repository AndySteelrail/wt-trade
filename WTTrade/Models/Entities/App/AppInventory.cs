namespace WTTrade.Models;

public class AppInventory
{
    private readonly Dictionary<AppItem, int> _dictInventory;

    public AppInventory(Dictionary<AppItem, int> dictInventory)
    {
        _dictInventory = dictInventory;
    }
    
    public override string ToString()
    {
        var setItems = new SortedSet<AppItem>(_dictInventory.Keys, new SortItemComparer());
        List<string> stringItems = setItems.Select(item => item + " - в кол-ве " + _dictInventory[item]).ToList();

        return string.Join("\n", stringItems);
    }
    
    private class SortItemComparer : IComparer<AppItem>
    {
        public int Compare(AppItem? item1, AppItem? item2)
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