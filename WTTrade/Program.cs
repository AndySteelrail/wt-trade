using Telegram.Bot;
using WTTrade.Initialization;
using WTTrade.Services;

namespace WTTrade;

internal abstract class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHttpClient();
        
        var authorization = new Authorization();
        
        builder.Services.AddSingleton(sp => new TelegramBotClient(authorization.TelegramToken));
        builder.Services.AddScoped<HttpService>(
            sp => new HttpService(sp.GetRequiredService<HttpClient>(), authorization.WarThunderToken));
        builder.Services.AddScoped<DataBaseService>(
            sp => new DataBaseService(
                authorization.dbHost, 
                authorization.dbPort, 
                authorization.dbName, 
                authorization.dbUsername, 
                authorization.dbPassword)
            );

        builder.Services.AddSingleton<TelegramBackgroundService>(sp => 
        {
            var botClient = sp.GetRequiredService<TelegramBotClient>();
            var userId = authorization.UserId;
            return new TelegramBackgroundService(botClient, userId, sp);
        });
        
        builder.Services.AddHostedService(sp => 
            sp.GetRequiredService<TelegramBackgroundService>());
        
        builder.Services.AddSingleton<ITelegramNotificationService>(sp => 
            sp.GetRequiredService<TelegramBackgroundService>());


        builder.Services.AddScoped<ServiceInformer>();
        builder.Services.AddScoped<ServiceTrader>();
        
        builder.Services.AddScoped<Service>();
        
        var app = builder.Build();
        
        await app.RunAsync();
    }
}