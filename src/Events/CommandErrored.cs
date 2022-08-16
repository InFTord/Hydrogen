using DSharpPlus.CommandsNext;

namespace Hydrogen.Events
{
    public class CommandErrored
    {
        public static Task CommandErroredAsync(CommandsNextExtension _, CommandErrorEventArgs eventArgs)
        {
            Console.WriteLine($"Command '{eventArgs.Command.Name}' errored: {eventArgs.Exception}");
            return eventArgs.Context.RespondAsync($"Bot Error: {eventArgs.Exception}");
        }
    }
}