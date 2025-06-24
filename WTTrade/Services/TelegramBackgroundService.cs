using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WTTrade.Services;

public class TelegramBackgroundService : BackgroundService, ITelegramNotificationService
{
    private readonly ITelegramBotClient _botClient;
    private readonly long _userId;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<long, (Task task, CancellationTokenSource cts)> _activeOperations;

    public TelegramBackgroundService(ITelegramBotClient botClient, long userId, IServiceProvider serviceProvider)
    {
        _botClient = botClient;
        _userId = userId;
        _serviceProvider = serviceProvider;
        _activeOperations = new ConcurrentDictionary<long, (Task, CancellationTokenSource)>();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions
            {
                AllowedUpdates = { }
            },
            cancellationToken: stoppingToken
        );
        
        return Task.CompletedTask;
    }
    
        private async Task HandleUpdateAsync(
        ITelegramBotClient botClient, 
        Update update, 
        CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
            return;

        IServiceScope serviceScope = _serviceProvider.CreateScope();
        var service = serviceScope.ServiceProvider.GetRequiredService<Service>();
        
        Console.WriteLine($"Получено сообщение от {update.Message.From?.FirstName}: {update.Message.Text}");
        
        long chatId = update.Message.Chat.Id;
        long messageFromId = update.Message.From.Id;
        
        if (_userId != messageFromId)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "У вас нет доступа к этому боту.",
                cancellationToken: cancellationToken);
            return;
        }
        
        switch (update.Message.Text.ToLower())
        {
            case "/start":
                await botClient.SendTextMessageAsync(chatId, 
                    "Добро пожаловать! Я ваш Telegram-бот для торговли на https://trade.gaijin.net/", 
                    cancellationToken: cancellationToken);
                break;
                
            case "/help":
                await botClient.SendTextMessageAsync(chatId, 
                    "Доступные команды:\n/start - начать общение\n/help - помощь\n/trade - начать торговлю\n/cancel - отменить текущую операцию", 
                    cancellationToken: cancellationToken);
                break;
                
            case "/balance":
                string balance = await service.GetBalanceAsync();
                await botClient.SendTextMessageAsync(chatId, 
                    $"Баланс на личном счёте:\n{balance:F2} gjn-коинов", 
                    cancellationToken: cancellationToken);
                break;
                
            case "/inventory":
                string inventory = await service.GetItemsInventoryAsync();
                await SendInventory(botClient, chatId, inventory, cancellationToken);
                break;
                
            case "/trade":
                await StartTradeOperation(chatId, service, cancellationToken);
                break;
                
            case "/cancel":
                await CancelActiveOperation(chatId, botClient, cancellationToken);
                break;
                
            default:
                await botClient.SendTextMessageAsync(chatId, 
                    $"Вы написали: {update.Message.Text}", 
                    cancellationToken: cancellationToken);
                break;
        }
    }

    private async Task StartTradeOperation(long chatId, Service service, CancellationToken cancellationToken)
    {
        if (_activeOperations.TryGetValue(chatId, out var existingOp))
        {
            existingOp.cts.Cancel();
            _activeOperations.TryRemove(chatId, out _);
        }

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var task = service.StartSellingAsync(chatId, cts.Token);

        _activeOperations.TryAdd(chatId, (task, cts));
    }

    private async Task CancelActiveOperation(long chatId, ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        if (_activeOperations.TryRemove(chatId, out var operation))
        {
            operation.cts.Cancel();
            await botClient.SendTextMessageAsync(chatId, 
                "Операция торговли отменена.", 
                cancellationToken: cancellationToken);
        }
        else
        {
            await botClient.SendTextMessageAsync(chatId, 
                "Нет активных операций для отмены.", 
                cancellationToken: cancellationToken);
        }
    }
    
    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Произошла ошибка: {exception.Message}");
        
        if (exception is ApiRequestException apiEx)
        {
            Console.WriteLine($"Ошибка API Telegram: {apiEx.ErrorCode} - {apiEx.Message}");
        }

        return Task.CompletedTask;
    }
    
    public async Task SendMessageAsync(long chatId, string? message)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                await _botClient.SendTextMessageAsync(chatId, message, parseMode: ParseMode.Html);
            }
            else
            {
                Console.WriteLine("Сообщение пустое и не может быть отправлено.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке сообщения в Telegram: {ex.Message}");
        }
    }

    private async Task SendInventory(ITelegramBotClient botClient, long chatId, string inventory, CancellationToken cancellationToken)
    {
        List<string> messages = TelegramMessageSplitter.SplitMessage(inventory, 4096);
        foreach (var message in messages)
        {
            await botClient.SendTextMessageAsync(chatId, message, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
        }
    }
}