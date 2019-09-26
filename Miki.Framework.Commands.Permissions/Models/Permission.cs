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

        [Key]
        public long PermissionId { get; set; }

        [Column]
		public long EntityId { get; set; }

		[Column]
		public string CommandName { get; set; }

		[Column]
		public long GuildId { get; set; }

		[Column]
		public EntityType Type { get; set; }

		[Column]
		public PermissionStatus Status { get; set; }
	}
}
