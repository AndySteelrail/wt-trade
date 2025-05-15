using Newtonsoft.Json;

namespace WTTrade.Models;

public class WTOrderStat
{
    [JsonProperty("responce")]
    public OrderResponseStat responce { get; set; }
}

public class OrderResponseStat
{
    [JsonProperty("1d")]
    public List<(long timestamp, long price, long amount)> _1d { get; set; }
    [JsonProperty("1h")]
    public List<(long timestamp, long price, long amount)> _1h { get; set; }
    [JsonProperty("success")]
    public bool success { get; set; }
}