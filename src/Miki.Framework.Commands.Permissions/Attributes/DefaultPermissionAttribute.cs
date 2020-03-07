namespace Miki.Framework.Commands.Permissions.Attributes
{
    using Miki.Framework.Commands.Permissions.Models;
    using System;
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class DefaultPermissionAttribute : Attribute
	{
		public PermissionStatus Status { get; }

		public DefaultPermissionAttribute(PermissionStatus status)
		{
			Status = status;
		}
	}
}
