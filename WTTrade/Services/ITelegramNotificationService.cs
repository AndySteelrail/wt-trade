namespace WTTrade.Services;

public interface ITelegramNotificationService
{
    Task SendMessageAsync(long chatId, string message);
}