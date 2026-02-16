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

        services.AddSingleton<Random>();
        services.AddSingleton<DiceService>();
        services.AddScoped<DiceCallbackDataSerializer>();
        services.AddSingleton<IDateTimeProvider>(sp => new DateTimeProvider());

        services.AddScoped<BotContext>(sp => new BotContext(dbPath!));
        services.AddScoped<UserRepository>();

        services.AddScoped<DiceKeyboardFactory>();
        services.AddScoped<CallbackData>();
        services.AddScoped<CommandsHandler>();
        services.AddScoped<ButtonHandler>();

        services.AddHostedService<BotService>();
    })
    .Build();

await host.RunAsync();