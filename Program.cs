using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen
{
    public class Program
    {
        public static IConfiguration Configuration { get; private set; } = null!;
        internal static readonly CancellationTokenSource cancellationTokenSource = new();
        public static CancellationToken cancellationToken = cancellationTokenSource.Token;

        public static async Task Main(string[] args)
        {
            IConfiguration? configuration = LoadConfiguration(args);
            if (configuration == null)
            {
                Console.WriteLine("failed to load configuration");
                Environment.Exit(1);
            }
            Configuration = configuration!;

            IServiceCollection services = new ServiceCollection();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            DiscordShardedClient client = new(new DiscordConfiguration
            {
                Token = configuration.GetValue<string>("token"),
#if DEBUG
                Intents = DiscordIntents.All
#else
                Intents = DiscordIntents.Guilds
#endif
            });

            IReadOnlyDictionary<int, CommandsNextExtension> commandsNextShards = await client.UseCommandsNextAsync(new CommandsNextConfiguration
            {
                StringPrefixes = Configuration.GetValue<string[]>("prefixes"),
                Services = serviceProvider

            });

            await client.StartAsync();
        }

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