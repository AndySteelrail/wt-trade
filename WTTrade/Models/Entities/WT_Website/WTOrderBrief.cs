using Newtonsoft.Json;

namespace WTTrade.Models;

public class WTOrderBrief
{
    [JsonProperty("response")]
    public OrderResponseBrief response { get; set; }
}

public class OrderResponseBrief
{
    [JsonProperty("BUY")]
    public List<List<long>> buy { get; set; }
    [JsonProperty("SELL")]
    public List<List<long>> sell { get; set; }
    [JsonProperty("depth")]
    public OrderDepth depth { get; set; }
    [JsonProperty("success")]
    public bool success { get; set; }
    [JsonProperty("type")]
    public string type { get; set; }
}

public class OrderDepth
{
    [JsonProperty("BUY")]
    public long buy { get; set; }
    [JsonProperty("SELL")]
    public long sell { get; set; }
}