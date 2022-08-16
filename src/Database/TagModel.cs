using System.ComponentModel.DataAnnotations;

namespace Hydrogen.Database
{
    public class TagModel
    {
        /// <summary>
        /// The id of the tag.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Which guild the tag belongs to.
        /// </summary>
        public ulong GuildId { get; init; }

        /// <summary>
        /// Who created the tag.
        /// </summary>
        public ulong CreatorId { get; init; }

        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string Name { get; internal set; } = null!;

        /// <summary>
        /// The content of the tag.
        /// </summary>
        [Range(1, 2000)]
        public string Content { get; internal set; } = null!;
    }
}