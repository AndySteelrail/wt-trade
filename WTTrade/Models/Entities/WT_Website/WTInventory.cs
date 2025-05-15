namespace WTTrade.Models;

public class WTInventory
{
    public InventoryResult? result { get; set; }
    
    public List<int> GetAllValues()
    {
        List<int> values = new List<int>();

        if (result != null && result.assets != null)
        {
            foreach (var asset in result.assets)
            {
                if (asset.@class != null)
                {
                    foreach (var itemClass in asset.@class)
                    {
                        values.Add(itemClass.value);
                    }
                }
            }
        }

        return values;
    }
}

public class InventoryResult
{
    public List<InventoryAsset>? assets { get; set; }
    public bool success { get; set; }
}

public class InventoryAsset
{
    public List<InventoryAssetsClass> @class { get; set; }
    public string? id { get; set; }
}

public class InventoryAssetsClass
{
    public string name { get; set; }
    public int value { get; set; }
}