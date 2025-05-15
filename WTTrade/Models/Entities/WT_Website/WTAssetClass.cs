using Newtonsoft.Json;

namespace WTTrade.Models;

public class WTAssetClass
{
    [JsonProperty("response")]
    public AssetClassResponse response { get; set; }

    public string GetValue()
    {
        return response.assets[0].value;
    }
}

public class AssetClassResponse
{
    [JsonProperty("asset_class")]
    public List<AssetClassAssets> assets { get; set; }
    [JsonProperty("success\n")]
    public bool success { get; set; }
}

public class AssetClassAssets
{
    [JsonProperty("name")]
    public string name { get; set; }
    [JsonProperty("value")]
    public string value { get; set; }
}