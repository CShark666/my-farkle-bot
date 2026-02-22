using Bot;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var botApi = context.Configuration["TelegramBot:Token"];
        var dbPath = context.Configuration["TelegramBot:DbString"];

        services.AddSingleton(sp => new TelegramBotClient(botApi!));

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddConsole();
        });

        services.AddSingleton<DiceService>();
        services.AddSingleton<IDateTimeProvider>(sp => new DateTimeProvider());

        services.AddScoped(sp => new BotContext(dbPath!));
        services.AddScoped<UserRepository>();

        services.AddScoped<DiceCallbackDataSerializer>();
        services.AddScoped<DiceKeyboardFactory>();
        services.AddScoped<CallbackData>();
        services.AddScoped<CommandsHandler>();
        services.AddScoped<ButtonHandler>();

        services.AddScoped<GameKeyboardFactory>();
        services.AddScoped<GameCallbackDataSerializer>();
        services.AddScoped<ScoreCalculator>();

        services.AddHostedService<BotService>();
    })
    .Build();

await host.RunAsync();