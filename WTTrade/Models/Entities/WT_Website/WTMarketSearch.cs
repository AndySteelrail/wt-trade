using Newtonsoft.Json;

namespace WTTrade.Models;

public class WTMarketSearch
{
    [JsonProperty("response")]
    public MarketSearchResponse response { get; set; }
    
    public string GetValue(string name)
    {
        foreach (var asset in response.assets)
        {
            if (asset.name == name)
            {
                return asset.assetClass[0].value;
            }
        }
        return "";
    }
}

public class MarketSearchResponse
{
    [JsonProperty("success")]
    public bool success { get; set; }
    [JsonProperty("assets")]
    public List<MarketSearchAsset> assets { get; set; }
}

public class MarketSearchAsset
{
    [JsonProperty("hash_name")]
    public string hashName { get; set; }
    [JsonProperty("asset_class")]
    public List<MarketSearchAssetClass> assetClass { get; set; }
    [JsonProperty("name")]
    public string name { get; set; }
    [JsonProperty("tags")]
    public List<string> tags { get; set; }
}

public class MarketSearchAssetClass
{
    [JsonProperty("name")]
    public string name { get; set; }
    [JsonProperty("value")]
    public string value { get; set; }
}