using Microsoft.Extensions.Logging;
using DSharpPlus.CommandAll;
using DSharpPlus.CommandAll.EventArgs;

namespace Hydrogen.Events
{
    public sealed class CommandError
    {
        private readonly ILogger<CommandError> _logger;

        public CommandError(ILogger<CommandError> logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [DiscordEvent]
        public Task CommandErrored(CommandAllExtension extension, CommandErroredEventArgs eventArgs)
        {
            eventArgs.Context.Channel.SendMessageAsync(eventArgs.Exception.Message);
            _logger.LogInformation("test");
            return Task.CompletedTask;
        }
    }
}
