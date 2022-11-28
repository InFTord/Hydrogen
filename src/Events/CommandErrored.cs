// using DSharpPlus.CommandsNext;

// namespace Hydrogen.Events
// {
//     public class CommandErrored
//     {
//         /// <summary>
//         /// Handler for command errors
//         /// </summary>
//         /// <param name="_">CommandsNext extension</param>
//         /// <param name="eventArgs"></param>
//         /// <returns></returns>
//         /// 
//         public static Task CommandErroredAsync(CommandsNextExtension _, CommandErrorEventArgs eventArgs)
//         {
//             Console.WriteLine(value: $"Command '{eventArgs.Command.Name}' errored: {eventArgs.Exception}");
//             return eventArgs.Context.RespondAsync(content: $"Bot Error: {eventArgs.Exception} {eventArgs.Exception.Message}");
//         }
//     }
// }
