using Miki.Framework.Commands.Permissions.Models;
using System;

namespace Miki.Framework.Commands.Permissions.Attributes
{
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
