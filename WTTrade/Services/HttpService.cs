using Newtonsoft.Json;
using WTTrade.Models;
using WTTrade.Models.WT_Website;

namespace WTTrade.Services;

public class HttpService
{
    private readonly HttpClient _client;
    private readonly string _token;

    private readonly string _marketHost = "https://market-proxy.gaijin.net";
    private readonly string _walletHost = "https://wallet.gaijin.net";

    public HttpService(HttpClient client, string token)
    {
        _client = client;
        _token = token;
    }

    public async Task<WTInventory> GetInventoryAsync()
    {
        string url = _marketHost + "/assetAPI";
        var payload = new Dictionary<string, string>
        {
            { "action", "GetContextContents" },
            { "token", _token },
            { "appid", "1067"},
            { "contextid", "1" }
        };
        
        return await MakeRequest<WTInventory>(HttpMethod.Post, url, null, payload);
    }
    
    public async Task<WTItem> GetItemAsync(int value)
    {
        await Task.Delay(DigitGenerator.GenerateGetItemDelay());
        
        string url = _marketHost + "/assetAPI";
        var payload = new Dictionary<string, string>
        {
            { "action", "GetAssetClassInfo" },
            { "token", _token },
            { "appid", "1067" },
            { "language", "ru_RU" },
            { "class_name0", "__itemdefid" },
            { "class_value0", $"{value}" },
            { "class_count", "1" }
        };
        
        return await MakeRequest<WTItem>(HttpMethod.Post, url, null, payload);
    }

    public async Task<WTBalance> GetBalanceAsync()
    {
        string url = _walletHost + "/GetBalance?";
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "BEARER " + _token },
        };
        
        return await MakeRequest<WTBalance>(HttpMethod.Get, url, headers);
    }
    
    public async Task<WTOpenOrders> GetOpenOrdersAsync()
    {
        string url = _marketHost + "/web";
        var payload = new Dictionary<string, string>
        {
            { "action", "cln_get_user_open_orders" },
            { "token", _token }
        };
        
        return await MakeRequest<WTOpenOrders>(HttpMethod.Post, url, null, payload);
    }
    
    public async Task<int> GetValueByHashNameAsync(string hashName)
    {
        string url = _marketHost + "/web";
        var payload = new Dictionary<string, string>
        {
            { "action", "cln_market_get_asset_class" },
            { "token", _token },
            { "appid", "1067" },
            { "name", hashName }
        };
        
        WTAssetClass wtAssetClass = 
            await MakeRequest<WTAssetClass>(HttpMethod.Post, url, null, payload);
            
        return Convert.ToInt32(wtAssetClass.GetValue());
    }
    
    public async Task<int> GetValueByNameAsync(string name)
    {
        string url = _marketHost + "/web";
        var payload = new Dictionary<string, string>
        {
            { "action", "cln_market_search" },
            { "token", _token },
            { "skip", "0" },
            { "count" , "26" },
            { "text" , name },
            { "language" , "ru_RU" },
            { "options" , "" } ,
            { "appid_filter" , "1067" }
        };
        
        WTMarketSearch wtMarketSearch = 
            await MakeRequest<WTMarketSearch>(HttpMethod.Post, url, null, payload);
            
        return Convert.ToInt32(wtMarketSearch.GetValue(name));
    }
    
    public async Task<WTHttpResponse> CancelOrderAsync(string pairId, string orderId)
    {
        await Task.Delay(DigitGenerator.GenerateCancelOrderDelay());
        
        string url = _marketHost + "/market";
        var payload = new Dictionary<string, string>
        {
            { "action", "cancel_order" },
            { "token", _token },
            { "transactid", DigitGenerator.GenerateTransactId(15) },
            { "reqstamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() },
            { "pairId", pairId },
            { "orderId", orderId }
        };

        return await MakeRequest<WTHttpResponse>(HttpMethod.Post, url, null, payload);
    }
    
    public async Task<WTOrderBrief> GetOrderBriefAsync(string marketName)
    {
        string url = _marketHost + "/web";
        var payload = new Dictionary<string, string>
        {
            { "action", "cln_books_brief" },
            { "token", _token },
            { "appid", "1067" },
            { "market_name", marketName }
        };
        
        return await MakeRequest<WTOrderBrief>(HttpMethod.Post, url, null, payload);
    }
    
    public async Task<WTOrderStat> GetOrderStatAsync(string marketName)
    {
        string url = _marketHost + "/web";
        var payload = new Dictionary<string, string>
        {
            { "action", "cln_get_pair_stat" },
            { "token", _token },
            { "appid", "1067" },
            { "market_name", marketName },
            { "currencyid", "gjn"}
        };
        
        return await MakeRequest<WTOrderStat>(HttpMethod.Post, url, null, payload);
    }
    
    public async Task<WTHttpResponse> CreateOrderBuyAsync(string hashName, int amount, decimal price)
    {
        await Task.Delay(DigitGenerator.GenerateBeetweenPlacingOrdersDelay());
        
        string url = "https://market-proxy.gaijin.net/web";

        string agreeStamp = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - DigitGenerator.GeneratePlacingOrderDelay()}";
        var payload = new Dictionary<string, string>
        {
            { "action", "cln_market_buy" },
            { "token", _token },
            { "transactid", DigitGenerator.GenerateTransactId(15) },
            { "reqstamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() },
            { "appid", "1067" },
            { "market_name", hashName },
            { "amount", amount.ToString() },
            { "currencyid", "gjn" },
            { "price", price.ToString() },
            { "agree_stamp", agreeStamp },
            { "privateMode", "true" }
        };
        
        return await MakeRequest<WTHttpResponse>(HttpMethod.Post, url, null, payload);
    }
    
    public async Task<WTHttpResponse> CreateOrderSellAsync(string assetId, decimal price, decimal priceAfterCommission)
    {
        await Task.Delay(DigitGenerator.GenerateBeetweenPlacingOrdersDelay());
        
        string url = "https://market-proxy.gaijin.net/web";

        string agreeStamp = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - DigitGenerator.GeneratePlacingOrderDelay()}";
        var payload = new Dictionary<string, string>
        {
            { "action", "cln_market_sell" },
            { "token", _token },
            { "transactid", DigitGenerator.GenerateTransactId(15) },
            { "reqstamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() },
            { "appid", "1067" },
            { "contextid", "1" },
            { "assetid", assetId },
            { "amount", "1" },
            { "currencyid", "gjn" },
            { "price", price.ToString() },
            { "seller_should_get", priceAfterCommission.ToString() }, // price * 0,85 с округлением в меньшую сторону
            { "agree_stamp", agreeStamp},
            { "privateMode", "true" }
        };
        
        return await MakeRequest<WTHttpResponse>(HttpMethod.Post, url, null, payload);
    }

    private async Task<T> MakeRequest<T>(
        HttpMethod method,
        string url,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? payload = null)
    
        where T : new()
    {
        payload ??= new Dictionary<string, string>();
    
        using var content = new FormUrlEncodedContent(payload);
    
        var request = new HttpRequestMessage(method, url)
        {
            Content = content
        };
        
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        try
        {
            using HttpResponseMessage response = await _client.SendAsync(request);
            
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            T t = JsonConvert.DeserializeObject<T>(responseString);
            
            return t;
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
            return new T();
        }
        catch (JsonException jsonEx)
        {
            Console.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
            return new T();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
            return new T();
        }
    }
}