using System.Globalization;
using Newtonsoft.Json;

namespace WTTrade.Models;

public class WTBalance
{
    [JsonProperty("balance")]
    public decimal balance { get; set; }

    public decimal GetValue()
    {
        return balance;
    }

    public override string ToString()
    {
        return (balance / 10000).ToString("F2", CultureInfo.CurrentCulture);
    }
}