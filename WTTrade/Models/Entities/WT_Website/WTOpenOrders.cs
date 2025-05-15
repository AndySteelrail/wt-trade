using Newtonsoft.Json;

namespace WTTrade.Models;

public class WTOpenOrders
{
    [JsonProperty("success")]
    public bool success { get; set; }
    [JsonProperty("response")]
    public List<WTOrder> openOrders { get; set; } = new List<WTOrder>();

    public (List<WTOrder> sell, List<WTOrder> buy) DevideOrdersByType()
    {
        List<WTOrder> sellOrders = new List<WTOrder>();
        List<WTOrder> buyOrders = new List<WTOrder>();
        
        foreach (var order in openOrders)
        {
            if (order.type == "SELL")
            {
                sellOrders.Add(order);
            }
            else
            {
                buyOrders.Add(order);
            }
        }
        
        return (sellOrders, buyOrders);
    }
}