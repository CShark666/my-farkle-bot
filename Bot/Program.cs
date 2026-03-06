using Bot;
using Microsoft.EntityFrameworkCore;
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

        services.AddDbContext<BotContext>(options => options.UseSqlite(dbPath));
        services.AddScoped<UserRepository>();
        services.AddScoped<GameRepository>();

        services.AddSingleton<GameButtonsFactory>();
        services.AddSingleton<GameResponseFactory>();
        services.AddSingleton<GameCallbackDataSerializer>();
        services.AddSingleton<ScoreCalculator>();
        
        services.AddScoped<ValidatorService>();
        services.AddScoped<CallbackData>();
        services.AddScoped<CommandsHandler>();
        services.AddScoped<ButtonHandler>();

        
        services.AddTransient<ICommandHandler, StartGameCmdHandler>();
        services.AddTransient<IButtonsHandler, StartGameBtnHandler>();
        services.AddTransient<IButtonsHandler, SelectDiceBtnHandler>();
        services.AddTransient<IButtonsHandler, SaveAndRollBtnHandler>();
        services.AddTransient<IButtonsHandler, SaveAndEndBtnHandler>();
        services.AddTransient<IButtonsHandler, StartNewTurnBtnHandler>();
        services.AddTransient<IButtonsHandler, SurrenderBtnHandler>();


        services.AddHostedService<BotService>();
        services.AddHostedService<GameCleanupService>();
    })
    .Build();

await host.RunAsync();