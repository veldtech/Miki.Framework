using Miki.Discord.Common;
using Miki.Framework.Commands.Permissions.Attributes;
using Miki.Framework.Commands.Permissions.Models;
using Miki.Framework.Commands.Pipelines;
using Miki.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Permissions
{
    /// <summary>
    /// Pipeline stage for Miki's Command Pipeline system, checks if the permissions are valid for this
    /// executable.
    /// </summary>
    public class PermissionPipelineStage : IPipelineStage
     {
         private readonly PermissionService service;

         internal PermissionPipelineStage(PermissionService service)
         {
             this.service = service;
         }

         /// <inheritdoc/>
         public async ValueTask CheckAsync(
             IDiscordMessage data, [NotNull] IMutableContext e, [NotNull] Func<ValueTask> next)
         {
             if(e?.Executable == null)
             {
                 Log.Debug("No executable found to perform permission check on.");
                 return;
             }

             var message = e.GetMessage();
             if(message.Author is IDiscordGuildUser)
             {
                 var permission = await service.GetPriorityPermissionAsync(e);
                 if(permission == null)
                 {
                     var defaultStatus = FetchPermissionStatusFrom(e.Executable);
                     permission = new Permission
                     {
                         GuildId = (long) e.GetGuild().Id,
                         CommandName = e.Executable.ToString(),
                         EntityId = 0,
                         Status = defaultStatus,
                         Type = 0
                     };
                 }

                 Log.Debug(permission.ToString());

                 if(permission.Status == PermissionStatus.Allow)
                 {
                     await next();
                 }
             }
             else
             {
                 if(FetchPermissionStatusFrom(e.Executable) == PermissionStatus.Deny)
                 {
                     throw new InvalidOperationException(
                         "Denied request due to default setting set to Deny");
                 }

                 await next();
             }
         }

         private PermissionStatus FetchPermissionStatusFrom(IExecutable executable)
         {
             if(!(executable is Node node))
             {
                 return PermissionStatus.Allow;
             }

             var defaultPermissionAtrrib = (DefaultPermissionAttribute) node.Attributes
                 .FirstOrDefault(x => x is DefaultPermissionAttribute);

             if(defaultPermissionAtrrib != null)
             {
                 return defaultPermissionAtrrib.Status;
             }

             return PermissionStatus.Allow;
         }
     }
 }