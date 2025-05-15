namespace WTTrade.Services;

public class Service
{
    private readonly ServiceInformer _serviceInformer;
    private readonly ServiceTrader _serviceTrader;

    public Service(HttpService httpService, DataBaseService dataBaseService, ITelegramNotificationService telegramService)
    {
        _serviceInformer = new ServiceInformer(httpService, dataBaseService);
        _serviceTrader = new ServiceTrader(httpService, telegramService, dataBaseService, _serviceInformer);
    }

    public async Task<string> GetBalanceAsync()
    {
        return await _serviceInformer.GetBalanceAsync();
    }
    
    public async Task<string> GetItemsInventoryAsync()
    {
        return await _serviceInformer.GetItemsInventoryAsync();
    }
    
    public Task<string> StartSellingAsync(long chatId, CancellationToken cancellationToken)
    {
        return _serviceTrader.StartSellingAsync(chatId, cancellationToken);
    }
}