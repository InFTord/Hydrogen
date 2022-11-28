using System.ComponentModel;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace Hydrogen.Commands
{
    public sealed class EchoCommand : BaseCommand {
        [Command("echo"), Description("hehe echo goes brrrr.....")]
        public static Task EchoAsync(CommandContext ctx, [Description("the text to echo-ho-ho...")] string text) => ctx.ReplyAsync(messageBuilder: new() { Content = text });
    }
}
