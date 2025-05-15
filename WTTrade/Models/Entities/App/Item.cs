using Newtonsoft.Json;
using WTTrade.Models.WT_Website;

namespace WTTrade.Models;

public class Item
{
    [JsonProperty("value")] public required int Value { get; set; } = 0;
    [JsonProperty("market_hash_name")]
    public string MarketHashName { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("marketable")]
    public bool Marketable { get; set; }
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }
    
    [JsonProperty("type")]
    public string? Type { get; set; }
    [JsonProperty("quality")]
    public string? Quality { get; set; }
    [JsonProperty("country")]
    public string? Country { get; set; }
    [JsonProperty("eventName")]
    public string? EventName { get; set; }
    [JsonProperty("sort_index")] public int SortIndex { get; set; } = 1;


    public Item(WTItem wtItem)
    {
        var asset = wtItem.result.asset;
        MarketHashName = asset.market_hash_name;
        Name = asset.name;
        Marketable = asset.marketable;
        Timestamp = asset.timestamp;

        foreach (var tag in asset.tags)
        {
            switch (tag.category)
            {
                case "type":
                    Type = tag.name;
                    SortIndex = tag.sort_index;
                    break;
                case "quality":
                    Quality = !string.IsNullOrEmpty(tag.name) ? tag.name : "Обычный";
                    break;
                case "country":
                    Country = tag.name;
                    break;
                case "eventName":
                    EventName = $"{tag.category_name} : {tag.name}";
                    break;
            }
        }
    }

    [JsonConstructor]
    public Item() {}

    public string GetNameInBold()
    {
        return $"<b>{Name}</b>";
    }
    
    public string GetMarketHashName()
    {
        return MarketHashName;
    }
    
    public string GetName()
    {
        return Name;
    }
    
    public override string ToString()
    {
        if (Name == null)
        {
            return string.Empty;
        }
        
        var formattedString = new List<string> { $"{GetNameInBold()} -" };
        
        if (Type != null) formattedString.Add(Type);
        if (Quality != null) formattedString.Add(Quality);
        if (Country != null)
        {
            formattedString.Insert(0, UnicodeCountryEmojies.GetEmoji(Country));
            formattedString.Add(Country);
        }
        if (EventName != null) formattedString.Add(EventName);
        
        return string.Join(" ", formattedString);
    }
}