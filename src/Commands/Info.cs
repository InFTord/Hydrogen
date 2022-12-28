using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

public sealed class TestCommand : BaseCommand
{
    private readonly ILogger<TestCommand>? _logger;

    public TestCommand(ILogger<TestCommand> logger) => _logger = logger;
    [Command("info"), Description("the test command")]
    public async Task ExecuteAsync(CommandContext context, [Description("user")] DiscordMember? user = null)
    {
        user ??= context.Member;


        DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
        {
            Title = $"Information about {user.DisplayName}",
            Thumbnail = new EmbedThumbnail() {Url = user.AvatarUrl}
        };

        embed.AddField("User nickname:", $"{user.DisplayName}#{user.Discriminator} ({user.Id})");
        embed.AddField("Account creation date:", $"{Formatter.Timestamp(user.CreationTimestamp, TimestampFormat.RelativeTime)}");
        embed.AddField("Guild join date:", $"{Formatter.Timestamp(user.JoinedAt, TimestampFormat.RelativeTime)}");

        await context.ReplyAsync(embed);

    }
}
