using Newtonsoft.Json;

namespace WTTrade.Models.WT_Website;

public class WTItem
{
    [JsonProperty("result")]
    public ItemResult result { get; set; }
}

public class ItemResult
{
    [JsonProperty("asset")]
    public ItemAsset asset { get; set; }
}

public class ItemAsset
{
    [JsonProperty("market_hash_name")]
    public string market_hash_name { get; set; }
    [JsonProperty("name")]
    public string name { get; set; }
    [JsonProperty("marketable")]
    public bool marketable { get; set; }
    [JsonProperty("timestamp")]
    public long timestamp { get; set; }
    [JsonProperty("tags")]
    public List<ItemTag> tags { get; set; } = new List<ItemTag>();
}

public class ItemTag
{
    [JsonProperty("category")]
    public string? category { get; set; }
    [JsonProperty("category_name")]
    public string? category_name { get; set; }
    [JsonProperty("category_sort_index")]
    public int category_sort_index { get; set; }
    [JsonProperty("name")]
    public string? name { get; set; }
    [JsonProperty("color")]
    public string? color { get; set; }
    [JsonProperty("sort_index")]
    public int sort_index { get; set; }
}