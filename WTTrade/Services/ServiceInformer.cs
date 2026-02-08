using WTTrade.Models;
using WTTrade.Models.WT_Website;

namespace WTTrade.Services;

public class ServiceInformer
{
    private readonly HttpService _httpService;
    private readonly DataBaseService _dataBaseService;
    
    private readonly Dictionary<string, int> _hashNameValues;
    private readonly Dictionary<string, int> _nameValues;
    private readonly Dictionary<int, AppItem> _valueItems;

    public ServiceInformer(HttpService httpClient, DataBaseService dataBaseService)
    {
        _httpService = httpClient;
        _dataBaseService = dataBaseService;
        
        _hashNameValues = dataBaseService.GetHashNameValues();
        _nameValues = dataBaseService.GetNameValues();
        _valueItems = dataBaseService.GetValueItems();
    }
    
    public async Task<string> GetBalanceAsync()
    {
        var balance = await _httpService.GetBalanceAsync();
        return balance.ToString();
    }

    private async Task<WTInventory> GetInventoryAsync()
    {
        return await _httpService.GetInventoryAsync();
    }
    
    public async Task<string> GetItemsInventoryAsync()
    {
        WTInventory wtInventory = await GetInventoryAsync();
        List<int> inventoryValues = wtInventory.GetAllValues();
    
        if (inventoryValues.Count == 0)
        {
            return "Ничего нет";
        }

        var itemList = new List<AppItem>();
        foreach (int value in inventoryValues)
        {
            AppItem appItem = await GetItemAsync(value);
            itemList.Add(appItem);
        }
    
        AppInventory appInventory = await AppInventoryFaсtory.Create(itemList);

        return appInventory.ToString();
    }

    public async Task<AppItem> GetItemAsync(int value)
    {
        _valueItems.TryGetValue(value, out AppItem? item);
        
        if (item == null)
        {
            WTItem wtItem = await _httpService.GetItemAsync(value);
            item = new AppItem(wtItem) { Value = value };

            try
            {
                _dataBaseService.InsertItem(item);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Ошибка вставки: {ex.Message}");
            }
        }
        return item;
    }
    
    public async Task<int> GetValueByNameAsync(string name)
    {
        _nameValues.TryGetValue(name, out int value);
        
        if (value == 0)
        {
            try
            {
                value = await _httpService.GetValueByNameAsync(name);
                _nameValues.Add(name, value);
                
                _dataBaseService.InsertNameValue(name, value);
            }
            catch (Exception)
            {
                Console.WriteLine($"Объекта с именем: {name} на сайте не выявлено");
                return 0;
            }
        }
        return value;
    }
    
    public async Task<int> GetValueByHashNameAsync(string hashName)
    {
        _hashNameValues.TryGetValue(hashName, out int value);
        
        if (value == 0)
        {
            try
            {
                value = await _httpService.GetValueByHashNameAsync(hashName);
                _hashNameValues.Add(hashName, value);

                _dataBaseService.InsertHashNameValue(hashName, value);
            }
            catch (Exception)
            {
                Console.WriteLine($"Объекта с хеш-именем: {hashName} на сайте не выявлено");
                return 0;
            }
        }
        return value;
    }
}