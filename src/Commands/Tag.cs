using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Hydrogen.Database;

namespace Hydrogen.Commands
{
    [Group("tag")]
    [RequireGuild]
    [RequireBotPermissions(Permissions.SendMessages)]
    public class Tag : BaseCommandModule
    {
        public DatabaseContext DatabaseContext { private get; set; } = null!;

        [GroupCommand]

        public Task TagAsync(CommandContext ctx, [RemainingText]
        [Description("the name of tag")] string tagName)
        {
            TagModel? tag = DatabaseContext.Tags.FirstOrDefault(tag => tag.Name == tagName.Trim().ToLowerInvariant() && tag.GuildId == ctx.Guild.Id);
            if (tag == null)
            {
                return ctx.RespondAsync("no tag?");
            }

            DiscordMessageBuilder messageBuilder = new();
            messageBuilder.WithContent(tag.Content);

            messageBuilder.WithAllowedMentions(Mentions.None);
            return ctx.RespondAsync(messageBuilder);
        }


        [Command("create")]
        [Description("Creates a new tag, if it doesn't exist already.")]
        [RequireUserPermissions(Permissions.ManageMessages)]
        public async Task CreateTagAsync(CommandContext context,
            [Description("The name of the tag.")] string name,
            [RemainingText, Description("The content that should be sent when the tag is invoked.")] string content
        )
        {
            TagModel? tag = DatabaseContext.Tags.FirstOrDefault(tag => tag.Name == name.Trim().ToLowerInvariant() && tag.GuildId == context.Guild.Id);
            if (tag != null)
            {
                DiscordMessageBuilder messageBuilder = new();
                messageBuilder.WithContent($"The tag already exists and is owned by <@{tag.CreatorId}> ({tag.CreatorId}).");
                messageBuilder.WithAllowedMentions(Mentions.None);
                await context.RespondAsync(messageBuilder);
                return;
            }

            tag = new TagModel
            {
                GuildId = context.Guild.Id,
                CreatorId = context.User.Id,

                Name = name.Trim().ToLowerInvariant(),

                Content = content.Trim()
            };
            await DatabaseContext.Tags.AddAsync(tag);
            await DatabaseContext.SaveChangesAsync();

            await context.RespondAsync($"Tag created successfully. You can now use {Formatter.InlineCode($"{context.Prefix}tag {name}")} to use it.");
        }
        [Command("info")]
        [Description("Shows information about tag")]
        public async Task InfoTagAsync(CommandContext ctx, [RemainingText, Description("the name of the tag")] string tagName)
        {
            TagModel? tag = DatabaseContext.Tags.FirstOrDefault(tag => tag.Name == tagName.Trim().ToLowerInvariant() && tag.GuildId == ctx.Guild.Id);

            if (tag == null)
            {
                await ctx.RespondAsync("no tag?");
            }

            DiscordMessageBuilder messageBuilder = new();
            messageBuilder.WithContent($"Tag name: {tag.Name}\nTag ID: {tag.Id}");

            await ctx.RespondAsync(messageBuilder);
        }
    }
}