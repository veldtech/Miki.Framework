using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miki.Framework.Commands.Permissions.Models
{
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
		Default = 0,
		Allow,
		Deny
	}

	public class Permission
	{
        [Column]
        [Key]
		public long EntityId { get; set; }

		[Column]
        [Key]
        public string CommandName { get; set; }

		[Column]
        [Key]
        public long GuildId { get; set; }

		[Column]
		public EntityType Type { get; set; }

		[Column]
		public PermissionStatus Status { get; set; }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case Permission p:
                    return p.EntityId == EntityId
                           && p.CommandName == CommandName
                           && p.GuildId == GuildId
                           && p.Status == Status;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return $"Permission: {Status} {CommandName} for {Type} {EntityId} ({GuildId})";
        }
    }
}
