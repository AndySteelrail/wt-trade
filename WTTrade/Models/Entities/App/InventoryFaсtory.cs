namespace WTTrade.Models;

public abstract class InventoryFaсtory
{
    public static Task<Inventory> Create(List<Item> itemList)
    {
        var dictInventory = new Dictionary<Item, int>();
        foreach (var item in itemList)
        {
            if (!dictInventory.TryAdd(item, 1))
            {
                dictInventory[item]++;
            }
        }
        return Task.FromResult(new Inventory(dictInventory));
    }
}