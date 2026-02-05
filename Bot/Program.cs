using Bot;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

        var botApi = config["TelegramBot:Token"];

        services.AddSingleton(sp => new TelegramBotClient(botApi!));

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddConsole();
        });

        services.AddSingleton<Random>();
        services.AddSingleton<DiceService>();
        services.AddSingleton<IDateTimeProvider>(sp => new DateTimeProvider());

        services.AddScoped<BuilderInlineKeyboardMarkups>();
        services.AddScoped<CallbackData>();
        services.AddScoped<CommandsHandler>();
        services.AddScoped<ButtonHandler>();

        services.AddHostedService<BotService>();
    })
    .Build();

await host.RunAsync();