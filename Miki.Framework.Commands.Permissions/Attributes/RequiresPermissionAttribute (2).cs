using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Permissions.Attributes
{
    public class RequiresPermissionAttribute : CommandRequirementAttribute
    {
        public PermissionLevel Level { get; private set; }

        public RequiresPermissionAttribute(PermissionLevel requiredPermission)
        {
            Level = requiredPermission;
        }

        public override ValueTask<bool> CheckAsync(IContext e)
        {
            return new ValueTask<bool>(e.GetUserPermissions() >= Level); 
        }

        public override Task OnCheckFail(IContext e)
        {
            return Task.CompletedTask;
        }
    }
}
