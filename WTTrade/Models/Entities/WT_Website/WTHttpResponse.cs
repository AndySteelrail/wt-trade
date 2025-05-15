using Newtonsoft.Json;

namespace WTTrade.Models;

public class WTHttpResponse
{
    [JsonProperty("response")]
    public WTHttpResponseSuccess response { get; set; }

    public bool Status()
    {
        return response.success;
    }
    
    public override string ToString()
    {
        return response?.success.ToString() ?? "null";
    }
}

public class WTHttpResponseSuccess
{
    [JsonProperty("success")]
    public bool success { get; set; } = false;
}