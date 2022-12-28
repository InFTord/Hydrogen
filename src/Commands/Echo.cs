using System.ComponentModel;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace Hydrogen.Commands
{
    public sealed class EchoCommand : BaseCommand
    {
        [Command("echo"), Description("hehe echo goes brrrr.....")]
        public static Task Echo(CommandContext ctx, [Description("the text to echo-ho-ho...")] string text) => ctx.ReplyAsync(text);
    }
}
