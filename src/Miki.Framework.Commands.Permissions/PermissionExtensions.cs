using System;
using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Commands.Permissions;

namespace Miki.Framework.Commands
{
    /// <summary>
    /// Helper functions for the PermissionPipelineStage.
    /// </summary>
    public static class PermissionExtensions
	{
        /// <summary>
        /// Initializes the permissions system at this index on your CommandPipeline. Permissions will
        /// give users a way to manage their entire command infrastructure in a ACL kind of manner.
        ///
        /// This stage requires you to already have set an Executable to work properly.
        /// </summary>
		public static CommandPipelineBuilder UsePermissions(this CommandPipelineBuilder builder)
		{
            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.UseStage(
                new PermissionPipelineStage(
                    builder.Services.GetRequiredService<PermissionService>()));
			return builder;
		}
	}
}
