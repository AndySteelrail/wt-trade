using WTTrade.Models;

namespace WTTrade.Services;

public class ServiceTrader
{
    private readonly HttpService _httpService;
    private readonly ServiceInformer _serviceInformer;
    private readonly PriceCalculator _priceCalculator;
    private readonly ITelegramNotificationService _telegramService;
    
    private readonly Dictionary<string, int> _whiteList;
    private readonly HashSet<string> _blackList;
    
    public ServiceTrader(HttpService httpService, ITelegramNotificationService  telegramService, DataBaseService dataBaseService, ServiceInformer serviceInformer)
    {
        _httpService = httpService;
        _telegramService = telegramService;
        _serviceInformer = serviceInformer;
        _priceCalculator = new PriceCalculator();
        
        _whiteList = dataBaseService.GetWhitelist();
        _blackList = dataBaseService.GetBlacklist();
    }
    
    public async Task<string> StartSellingAsync(long chatId, CancellationToken cancellationToken)
    {
        try
        {
            _priceCalculator.ClearSellPriceDictionary();
            _priceCalculator.ClearBuyPriceDictionary();
            
            var openOrders = await _httpService.GetOpenOrdersAsync();
            (List<WTOrder> sell, List<WTOrder> buy) categorizedOrders = openOrders.DevideOrdersByType();

            var shoppingList = await GetShoppingList(chatId);

            await CancelSellOrders(categorizedOrders.sell, shoppingList, chatId, cancellationToken);
            await CreateSellOrders(shoppingList, chatId, cancellationToken);
            await CancelBuyOrders(categorizedOrders.buy, shoppingList, chatId, cancellationToken);
            await CreateBuyOrders(shoppingList, chatId, cancellationToken);
            
            return "Работа с заявками закончена";
            
        }
        catch (OperationCanceledException)
        {
            return "Операция торговли была отменена";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            return $"Ошибка при выставлении заявок: {ex.Message}";
        }
    }

    private async Task<Dictionary<int, int>> GetShoppingList(long chatId)
    {
        var shoppingList = new Dictionary<int, int>();
                
        foreach (var pair in _whiteList)
        {
            int value = await _serviceInformer.GetValueByNameAsync(pair.Key);
            if (value == 0)
            {
                await _telegramService.SendMessageAsync(
                    chatId, 
                    $"<b>{pair.Key}</b> - на сайте не найден");
                continue;
            }
                    
            shoppingList[value] = pair.Value;
        }

        return shoppingList;
    }

    private async Task<string> CancelOrderAsync(WTOrder wtOrder, string type)
    {
        WTOrderBrief wtOrderBrief = await _httpService.GetOrderBriefAsync(wtOrder.market);
        int value = await _serviceInformer.GetValueByHashNameAsync(wtOrder.market);
        AppItem appItem = await _serviceInformer.GetItemAsync(value);
                    
        decimal orderPrice = Convert.ToDecimal(wtOrder.localPrice);
        decimal bestPrice = 0;
        if (type == "на продажу")
        {
            bestPrice = _priceCalculator.CalcSellPrice(wtOrderBrief, value);
        }
        else
        {
            bestPrice = _priceCalculator.CalcBuyPrice(wtOrderBrief, value);
        }

        string message = "Лот не нужно перевыставлять";
        if (orderPrice == bestPrice)
        {
            return message;
        }
        
        WTHttpResponse wtHttpResponse = await _httpService.CancelOrderAsync(wtOrder.pairId, wtOrder.id);
        message = TraderMessages.CancelOrder(
            appItem.GetNameInBold(),
            type,
            wtHttpResponse.Status());

        return message;
    }

    private async Task<string> CreateOrderSellAsync(InventoryAsset asset)
    {
        int value = asset.@class[0].value;

        string type = "на продажу";
        string message = "Лот не нужно продавать";

        AppItem appItem = await _serviceInformer.GetItemAsync(value);
        
        if (_blackList.Contains(appItem.GetName()))
        {
            return message;
        }
        
        WTOrderBrief wtOrderBrief = await _httpService.GetOrderBriefAsync(appItem.GetMarketHashName());
        
        decimal bestPrice = _priceCalculator.CalcSellPrice(wtOrderBrief, value);
        decimal price = _priceCalculator.CalcDiscountPrice(bestPrice, type);
        decimal priceAfterCommission = _priceCalculator.CalcAfterCommissionPrice(price);

        if (price <= 1300m)
        {
            return message;
        }

        WTHttpResponse wtHttpResponse = await _httpService.CreateOrderSellAsync(asset.id, price, priceAfterCommission);
        message = TraderMessages.CreateOrder(
            appItem.GetNameInBold(),
            type,
            price / 10000,
            1,
            wtHttpResponse.Status());
        
        return message;
    }

    private async Task<string> CreateOrderBuyAsync(int value, int amount, AppBalance appBalance)
    {
        decimal currentBalance = appBalance.Value;
        Console.WriteLine($"Текущий остаток по балансу: {currentBalance}");
        string type = "на покупку";
        if (currentBalance <= 10000m)
        {
            return "Баланс составляет меньше или равен 1,00 gjn";
        }
        
        string message = "Лот не нужно покупать";

        AppItem appItem = await _serviceInformer.GetItemAsync(value);
        
        if (_blackList.Contains(appItem.GetName()))
        {
            return message;
        }

        string nameInBold = appItem.GetNameInBold();
        string hashName = appItem.GetMarketHashName();

        WTOrderBrief wtOrderBrief;
        
        try
        {
            wtOrderBrief = await _httpService.GetOrderBriefAsync(hashName);
        }
        catch (Exception e)
        {
            return $"Ошибка получения лота товара: {nameInBold}";
        }
        
        decimal bestPrice = _priceCalculator.CalcBuyPrice(wtOrderBrief, value);
        decimal price = _priceCalculator.CalcDiscountPrice(bestPrice, type);
        int allowableAmount = _priceCalculator.CalcAllowableAmount(amount, price, currentBalance);

        if (price <= 1300m || allowableAmount == 0)
        {
            return message;
        }

        try
        {
            WTHttpResponse wtHttpResponse = await _httpService.CreateOrderBuyAsync(hashName, allowableAmount, price);
            message = TraderMessages.CreateOrder(
                nameInBold,
                type,
                price / 10000,
                allowableAmount,
                wtHttpResponse.Status());

            appBalance.Value -= allowableAmount * price;
        }
        catch (Exception e)
        {
            return $"Ошибка при выставлении заявки: {nameInBold} в количестве {allowableAmount} по цене {price / 10000} gjn";;
        }
        
        return message;
    }

    private void TryChangeValue(Dictionary<int, int> dict, int value, int count)
    {
        if (dict.ContainsKey(value))
        {
            dict[value] += count;
        }
    }

    private async Task CancelSellOrders(List<WTOrder> sellOrders, Dictionary<int,int>? shoppingList, long chatId, CancellationToken cancellationToken)
    {
        foreach (var order in sellOrders)
        {
            cancellationToken.ThrowIfCancellationRequested();
                
            try
            {
                string response = await CancelOrderAsync(order, "на продажу");
                if (response == "Лот не нужно перевыставлять")
                {
                    int value = await _serviceInformer.GetValueByHashNameAsync(order.market);
                    TryChangeValue(shoppingList, value, -1);
                }
                else
                {
                    await _telegramService.SendMessageAsync(chatId, response);
                }
            }
            catch (OperationCanceledException)
            {
                await _telegramService.SendMessageAsync(chatId, "Процесс снятия неконкурентных заявок остановлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                await _telegramService.SendMessageAsync(chatId, "Ошибка при снятии заявки на : {order.market}");
            }
        }
            
        await _telegramService.SendMessageAsync(chatId, "Все неконкурентные заявки на продажу сняты");
    }

    private async Task CreateSellOrders(Dictionary<int,int>? shoppingList, long chatId, CancellationToken cancellationToken)
    {
        WTInventory wtInventory = await _httpService.GetInventoryAsync();
        List<InventoryAsset> assets = wtInventory.result.assets;


        foreach (var asset in assets)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int value = asset.@class[0].value;
            try
            {
                TryChangeValue(shoppingList, value, -1);
                
                string response = await CreateOrderSellAsync(asset);
                if (response != "Лот не нужно продавать")
                {
                    await _telegramService.SendMessageAsync(chatId, response);
                }
            }
            catch (OperationCanceledException)
            {
                await _telegramService.SendMessageAsync(chatId, "Процесс выставления заявок на продажу остановлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                await _telegramService.SendMessageAsync(chatId, $"Ошибка при выставлении заявки на продажу : {_serviceInformer.GetItemAsync(value)}");
            }
        }
            
        await _telegramService.SendMessageAsync(chatId, $"Заявки на продажу выставлены");
    }

    private async Task CancelBuyOrders(List<WTOrder> buyOrders, Dictionary<int,int>? shoppingList, long chatId, CancellationToken cancellationToken)
    {
        foreach (var order in buyOrders)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                int value = await _serviceInformer.GetValueByHashNameAsync(order.market);
                if (shoppingList.ContainsKey(value) && shoppingList[value] > 0)
                {
                    string response = await CancelOrderAsync(order, "на покупку");
                    if (response == "Лот не нужно перевыставлять")
                    {
                        TryChangeValue(shoppingList, value, -Convert.ToInt32(order.amount));
                    }
                    else
                    {
                        await _telegramService.SendMessageAsync(chatId, response);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                await _telegramService.SendMessageAsync(chatId, "Процесс снятия неконкурентных заявок остановлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                await _telegramService.SendMessageAsync(chatId, $"Ошибка при перевыставлении заявки на : {order.market}");
            }
        }
            
        await _telegramService.SendMessageAsync(chatId, $"Все неконкурентные заявки на покупку сняты");
    }

    private async Task CreateBuyOrders(Dictionary<int, int> shoppingList, long chatId, CancellationToken cancellationToken)
    {
        WTBalance currentWtBalance = await _httpService.GetBalanceAsync();
        AppBalance appBalance = new AppBalance(
            currentWtBalance.GetValue());
        
        foreach (var pair in shoppingList)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                string response = await CreateOrderBuyAsync(pair.Key, pair.Value, appBalance);
                if (response == "Лот не нужно покупать")
                {
                    TryChangeValue(shoppingList, pair.Key, -pair.Value);
                }
                else if (response == "Баланс составляет меньше или равен 1,00 gjn")
                {
                    await _telegramService.SendMessageAsync(chatId, "Баланс составляет меньше или равен 1,00 gjn. Процесс выставления заявок на покупку остановлен.");
                    break;
                }
                else
                {
                    await _telegramService.SendMessageAsync(chatId, response);
                }
            }
            catch (OperationCanceledException)
            {
                await _telegramService.SendMessageAsync(chatId, "Процесс выставления заявок на покупку остановлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                await _telegramService.SendMessageAsync(chatId, $"Ошибка при выставлении заявки на покупку : {_serviceInformer.GetItemAsync(pair.Key)}");
            }
        }
            
        await _telegramService.SendMessageAsync(chatId, $"Заявки на покупку выставлены");
    }
}