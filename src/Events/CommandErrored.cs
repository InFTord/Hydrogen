using OoLunar.DSharpPlus.CommandAll;
using OoLunar.DSharpPlus.CommandAll.EventArgs;

namespace Hydrogen.Events
{
    public class CommandError
    {
        /// <summary>
        /// Handler for command errors
        /// </summary>
        /// <param name="_">CommandsNext extension</param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        /// 
        // public static Task CommandErroredAsync(CommandsNextExtension _, CommandErrorEventArgs eventArgs)
        // {
        //     Console.WriteLine(value: $"Command '{eventArgs.Command.Name}' errored: {eventArgs.Exception}");
        //     return eventArgs.Context.RespondAsync(content: $"Bot Error: {eventArgs.Exception} {eventArgs.Exception.Message}");
        // }
        public static Task CommandErroredAsync(CommandAllExtension _, CommandErroredEventArgs eventArgs)
        {
            eventArgs.Context.Channel.SendMessageAsync("fuck you");
            return Task.CompletedTask;
        }
    }
}
