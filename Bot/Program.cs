using Bot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);

var botToken = builder.Configuration["TelegramBot:Token"];
var webhookUrl = builder.Configuration["TelegramBot:WebhookUrl"];

builder.Services.AddHttpClient("tgwebhook")
    .AddTypedClient(httpClient => new TelegramBotClient(botToken!, httpClient));

builder.Services.AddScoped<CallbackData>();
builder.Services.AddScoped<CommandsHandler>();
builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ButtonHandler>();

var app = builder.Build();

app.MapGet("/bot/setWebhook", async (TelegramBotClient bot) =>
{
    await bot.SetWebhook(webhookUrl!);
    return $"Webhook set to {webhookUrl}";
});

app.MapPost("/bot", async(TelegramBotClient bot, Update update, UpdateHandler updateHandler) =>
{
    await updateHandler.OnUpdateAsync(bot, update);
    return Results.Ok();
});

app.Run();

