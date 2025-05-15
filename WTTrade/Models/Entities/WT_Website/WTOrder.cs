using Newtonsoft.Json;

namespace WTTrade.Models;


public class WTOrder
{
    [JsonProperty("amount")]
    public string amount { get; set; }
    [JsonProperty("appId")]
    public string appId { get; set; }
    [JsonProperty("currency")]
    public string currency { get; set; }
    [JsonProperty("id")]
    public string id { get; set; }
    [JsonProperty("localPrice")]
    public string localPrice { get; set; }
    [JsonProperty("market")]
    public string market { get; set; }
    [JsonProperty("mid")]
    public string mid { get; set; }
    [JsonProperty("pairId")]
    public string pairId { get; set; }
    [JsonProperty("time")]
    public long time { get; set; }
    [JsonProperty("txId")]
    public string txId { get; set; }
    [JsonProperty("type")]
    public string type { get; set; }
}