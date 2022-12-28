using System.Globalization;
using System.Reflection;
using DSharpPlus;
using Hydrogen.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DSharpPlus.CommandAll;
using DSharpPlus.CommandAll.Parsers;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Hydrogen
{
    public class Program
    {
        private static IConfiguration Configuration { get; set; } = null!;
        private static readonly CancellationTokenSource CancellationTokenSource = new();
        public static CancellationToken CancellationToken = CancellationTokenSource.Token;
        /// <summary>
        /// Main application class
        /// </summary>
        /// <param name="args">Arguments for LoadConfiguration method</param>
        ///
        public static async Task Main(string[] args)
        {
            IConfiguration? configuration = LoadConfiguration(args);
            if (configuration == null)
            {
                Console.WriteLine("failed to load configuration");
                Environment.Exit(1);
            }
            Configuration = configuration!;

            IServiceCollection serviceCollection = new ServiceCollection();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddLogging(logger =>
{
    string loggingFormat = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u4}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    // Log both to console and the file
    LoggerConfiguration loggerConfiguration = new LoggerConfiguration().MinimumLevel.Is(LogEventLevel.Information)
    .WriteTo.Console(outputTemplate: loggingFormat, theme: new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
    {
        [ConsoleThemeStyle.Text] = "\x1b[0m",
        [ConsoleThemeStyle.SecondaryText] = "\x1b[90m",
        [ConsoleThemeStyle.TertiaryText] = "\x1b[90m",
        [ConsoleThemeStyle.Invalid] = "\x1b[31m",
        [ConsoleThemeStyle.Null] = "\x1b[95m",
        [ConsoleThemeStyle.Name] = "\x1b[93m",
        [ConsoleThemeStyle.String] = "\x1b[96m",
        [ConsoleThemeStyle.Number] = "\x1b[95m",
        [ConsoleThemeStyle.Boolean] = "\x1b[95m",
        [ConsoleThemeStyle.Scalar] = "\x1b[95m",
        [ConsoleThemeStyle.LevelVerbose] = "\x1b[34m",
        [ConsoleThemeStyle.LevelDebug] = "\x1b[90m",
        [ConsoleThemeStyle.LevelInformation] = "\x1b[36m",
        [ConsoleThemeStyle.LevelWarning] = "\x1b[33m",
        [ConsoleThemeStyle.LevelError] = "\x1b[31m",
        [ConsoleThemeStyle.LevelFatal] = "\x1b[97;91m"
    }))
    .WriteTo.File(
        $"logs/{DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd' 'HH'_'mm'_'ss", CultureInfo.InvariantCulture)}.log",
        outputTemplate: loggingFormat
    );

    // Set Log.Logger for a static reference to the logger
    logger.AddSerilog(loggerConfiguration.CreateLogger());
});

            Assembly currentAssembly = typeof(Program).Assembly;
            serviceCollection.AddSingleton((serviceProvider) =>
            {
                EventManager eventManager = new(serviceProvider);
                eventManager.GatherEventHandlers(currentAssembly);
                return eventManager;
            });

            // Register the Discord sharded client to the service collection
            serviceCollection.AddSingleton((serviceProvider) =>
            {
                EventManager eventManager = serviceProvider.GetRequiredService<EventManager>();
                IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>() ?? throw new ArgumentNullException("serviceProvider.GetRequiredService<IConfiguration>()");
                DiscordConfiguration discordConfig = new()
                {
                    Token = configuration.GetValue<string>("token"),
                    Intents = DiscordIntents.DirectMessages | DiscordIntents.GuildMembers | DiscordIntents.GuildMessages | DiscordIntents.Guilds | DiscordIntents.MessageContents,
                    LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>()
                };

                DiscordShardedClient shardedClient = new(discordConfig);
                eventManager.RegisterEventHandlers(shardedClient);
                return shardedClient;
            });

            ServiceProvider services = serviceCollection.BuildServiceProvider();
            DiscordShardedClient shardedClient = services.GetRequiredService<DiscordShardedClient>();
            EventManager eventManager = services.GetRequiredService<EventManager>();

            IReadOnlyDictionary<int, CommandAllExtension> commandsNextShards = await shardedClient.UseCommandAllAsync(new CommandAllConfiguration(serviceCollection)
            {
                // StringPrefixes = Configuration.GetSection("prefixes").Get<string[]>(),
                // Services = serviceProvider
                DebugGuildId = 940635246527938620,
                PrefixParser = new PrefixParser(Configuration.GetSection("prefixes").Get<string[]>() ?? throw new InvalidOperationException())

            });


            foreach (CommandAllExtension commandsNextExtension in commandsNextShards.Values)
            {
                // We'll register our commands automagically using the power of reflection.
                //  This will automatically register all commands in this project.
                commandsNextExtension.AddCommands(currentAssembly);
                eventManager.RegisterEventHandlers(commandsNextExtension);

            }

            // Start the bot
            await shardedClient.StartAsync();
            await Task.Delay(-1);
        }

        /// <summary>
        /// Loads a configuration file
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>IConfiguration</returns>
        private static IConfiguration? LoadConfiguration(string[] args)
        {
            if (Configuration != null)
            {
                return Configuration;
            }

            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder.Sources.Clear();

            string configurationFilePath = Path.Join(Environment.CurrentDirectory, "res", "config.json");
            if (File.Exists(configurationFilePath))
            {
                configurationBuilder.AddJsonFile(Path.Join(Environment.CurrentDirectory, "res", "config.json"), true, true);
            }

            configurationBuilder.AddEnvironmentVariables("DISCORD_BOT_");
            configurationBuilder.AddCommandLine(args);

            return configurationBuilder.Build();
        }
    }
}
