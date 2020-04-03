namespace Miki.Framework.Commands.Permissions.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;

    public enum EntityType
	{
		User,
		Channel,
		Role,
		Guild,
		Global,
	}

	public enum PermissionStatus
	{
        /// <summary>
        /// Is used as a fall-through option. <see cref="Default"/> will not impact the permission tree.
        /// </summary>
		Default = 0,

        /// <summary>
        /// Means that this permission is allowed to be used.
        /// </summary>
		Allow,

        /// <summary>
        /// Means that this permission is not allowed to be used.
        /// </summary>
		Deny
	}

    [DataContract]
	public class Permission
	{
        /// <summary>
        /// The Entity ID of the permission, use <see cref="Type"/> to find out to which repository it
        /// relates to.
        /// </summary>
        [DataMember(Name = "entity_id", Order = 1)]
        [Column]
        [Key]
		public long EntityId { get; set; }

        [DataMember(Name = "command_name", Order = 2)]
        [Column]
        [Key]
        public string CommandName { get; set; }

        [DataMember(Name = "guild_id", Order = 3)]
        [Column]
        [Key]
        public long GuildId { get; set; }

        [DataMember(Name = "entity_type", Order = 4)]
        [Column]
		public EntityType Type { get; set; }

        /// <summary>
        /// The current permission of the entity.
        /// </summary>
        [DataMember(Name = "status", Order = 5)]
        [Column]
        public PermissionStatus Status { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case Permission p:
                    return Equals(p);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Does an equal check on another permission reference.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(Permission other)
        {
            if(other == null)
            {
                return false;
            }
            return EntityId == other.EntityId 
                   && CommandName == other.CommandName 
                   && GuildId == other.GuildId;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(EntityId, CommandName, GuildId);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Permission: {Status} {CommandName} for {Type} {EntityId} ({GuildId})";
        }
    }
}
