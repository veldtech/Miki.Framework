namespace Miki.Framework.Commands.Permissions.Models
{
	public class PermissionMapping<T>
	{
		long RoleId { get; set; }
		T OwnerId { get; set; }
	}
}
