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


        [Command("create")] // This is the command that will be called when "!tag create" is called.
        [Description("Creates a new tag, if it doesn't exist already.")]
        [RequireUserPermissions(Permissions.ManageMessages)] // This subcommand requires the user to have the Manage Messages permission.
        public async Task CreateTagAsync(CommandContext context,
            [Description("The name of the tag.")] string name,
            [RemainingText, Description("The content that should be sent when the tag is invoked.")] string content
        )
        {
            // Check to see if the tag already exists:
            TagModel? tag = DatabaseContext.Tags.FirstOrDefault(tag => tag.Name == name.Trim().ToLowerInvariant() && tag.GuildId == context.Guild.Id);
            if (tag != null)
            {
                DiscordMessageBuilder messageBuilder = new();
                // You can use <@user id> to mention a user. Prevents having to fetch the user from the Discord API.
                messageBuilder.WithContent($"The tag already exists and is owned by <@{tag.CreatorId}> ({tag.CreatorId}).");
                messageBuilder.WithAllowedMentions(Mentions.None);
                await context.RespondAsync(messageBuilder);
                return;
            }

            // Create the tag:
            tag = new TagModel
            {
                // Note how we don't specify the Id here. This is because the database will kindly generate one for us, preventing conflicts.
                GuildId = context.Guild.Id,
                CreatorId = context.User.Id,

                // Trim the name and lowercase it to prevent case-sensitivity issues and small performance gains upon lookup.
                Name = name.Trim().ToLowerInvariant(),

                // Trim the content in case it was quoted. Every byte counts.
                Content = content.Trim()
            };
            DatabaseContext.Tags.Add(tag);

            // There is a large possiblity that the database will fail to save the tag.
            //  You as a bot developer should handle these race conditions of when two people attempt
            //  to create the same tag at the same time in the same guild.
            //  We don't handle it here because it's out of scope for this example project.
            //  No, I'm not sorry :D
            await DatabaseContext.SaveChangesAsync();

            // The use of the formatter class here is completely redundant, but it's here to bring awareness to it's existance and showcases how to use it.
            await context.RespondAsync($"Tag created successfully. You can now use {Formatter.InlineCode($"{context.Prefix}tag {name}")} to use it.");
        }
        [Command("info")]
        [Description("Shows information about tag")]
        public async Task InfoTagAsync(CommandContext ctx, [RemainingText, Description("the name of the tag")] string tagName) {
            TagModel? tag = DatabaseContext.Tags.FirstOrDefault(tag => tag.Name == tagName.Trim().ToLowerInvariant() && tag.GuildId == ctx.Guild.Id);

            if (tag == null) {
                await ctx.RespondAsync("no tag?");
            }

            DiscordMessageBuilder messageBuilder = new();
            messageBuilder.WithContent($"Tag name: {tag.Name}\nTag ID: {tag.Id}");

            await ctx.RespondAsync(messageBuilder);
        }
    }
}