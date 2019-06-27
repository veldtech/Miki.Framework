using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Permissions.Attributes
{
    public class RequiresPermissionAttribute : CommandRequirementAttribute
    {
        private PermissionLevel _level;

        public RequiresPermissionAttribute(PermissionLevel requiredPermission)
        {
            _level = requiredPermission;
        }

        public override async Task<bool> CheckAsync(IContext e)
        {
            if (e.GetGuild() == null)
            {
                return _level <= 0;
            }

            var permissions = e.GetStage<PermissionPipelineStage>();

            long authorId = (long)e.GetMessage().Author.Id;
            
            var i = await permissions.GetForUserAsync(e, authorId);

            if (e.GetGuild().OwnerId == e.GetMessage().Author.Id
                && i < PermissionLevel.OWNER)
            {
                return _level <= PermissionLevel.OWNER;
            }

            return e.GetUserPermissions() >= _level; 
        }

        public override Task OnCheckFail(IContext e)
        {
            return Task.CompletedTask;
        }
    }
}
