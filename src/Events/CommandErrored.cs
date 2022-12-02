using Microsoft.Extensions.Logging;
using OoLunar.DSharpPlus.CommandAll;
using OoLunar.DSharpPlus.CommandAll.EventArgs;

namespace Hydrogen.Events
{
    public class CommandError
    {
        private readonly ILogger<CommandError>? _logger;

        public CommandError(ILogger<CommandError> logger) => _logger = logger;
        public static Task CommandErroredAsync(CommandAllExtension extension, CommandErroredEventArgs eventArgs)
        {
            eventArgs.Context.Channel.SendMessageAsync(eventArgs.Exception.Message);
            logger.LogInformation("test");
            return Task.CompletedTask;
        }
    }
}
