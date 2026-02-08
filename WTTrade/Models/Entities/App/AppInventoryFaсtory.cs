namespace WTTrade.Models;

public abstract class AppInventoryFaсtory
{
    public static Task<AppInventory> Create(List<AppItem> itemList)
    {
        var dictInventory = new Dictionary<AppItem, int>();
        foreach (var item in itemList)
        {
            if (!dictInventory.TryAdd(item, 1))
            {
                dictInventory[item]++;
            }
        }
        return Task.FromResult(new AppInventory(dictInventory));
    }
}