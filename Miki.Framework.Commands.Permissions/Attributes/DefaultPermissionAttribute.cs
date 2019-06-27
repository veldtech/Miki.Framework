using Miki.Framework.Commands.Permissions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Commands.Permissions.Attributes
{
    public class DefaultPermissionAttribute : Attribute
    {
        public PermissionStatus Status { get; }

        public DefaultPermissionAttribute(PermissionStatus status)
        {
            Status = status;
        }
    }
}
