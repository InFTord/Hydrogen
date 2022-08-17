using System.Runtime.CompilerServices;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Hydrogen.Database;
using Hydrogen.Events;
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
            services.AddDbContextFactory<DatabaseContext>();
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
                StringPrefixes = Configuration.GetSection("prefixes").Get<string[]>(),
                Services = serviceProvider
            });



            foreach (CommandsNextExtension commandsNextExtension in commandsNextShards.Values)
            {
                // We'll register our commands automagically using the power of reflection.
                //  This will automatically register all commands in this project.
                commandsNextExtension.RegisterCommands(typeof(Program).Assembly);
                commandsNextExtension.CommandErrored += CommandErrored.CommandErroredAsync;
            }

            await client.StartAsync();
            await Task.Delay(-1);
        }

        public static IConfiguration? LoadConfiguration(string[] args)
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