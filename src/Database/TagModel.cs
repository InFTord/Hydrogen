using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hydrogen.Database
{
    [Table("tags")]
    public class TagModel
    {
        /// <summary>
        /// The id of the tag.
        /// </summary>
        [Column("tag_id")]
        public Guid Id { get; init; }

        /// <summary>
        /// Which guild the tag belongs to.
        /// </summary>
        [Column("guild_id")]
        public ulong GuildId { get; init; }

        /// <summary>
        /// Who created the tag.
        /// </summary>
        [Column("creator_id")]
        public ulong CreatorId { get; init; }

        /// <summary>
        /// The name of the tag.
        /// </summary>
        [Column("tag_name")]
        public string Name { get; internal set; } = null!;

        /// <summary>
        /// The content of the tag.
        /// </summary>
        [Column("tag_content")]
        [Range(1, 2000)]
        public string Content { get; internal set; } = null!;

        [Column("creation_date")]
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }
}