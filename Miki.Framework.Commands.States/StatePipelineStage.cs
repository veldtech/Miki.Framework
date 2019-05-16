using Microsoft.EntityFrameworkCore;
using Miki.Discord.Common;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Commands.States;
using Miki.Logging;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.States
{
    /// <summary>
    /// A very basic state management pipeline stage for customizable per-channel states.
    /// </summary>
    public class StatePipelineStage : IPipelineStage
    {
        public class StateConfiguration
        {
            public bool defaultState = true;
        }

        private readonly StateConfiguration _config;

        public StatePipelineStage(StateConfiguration config)
        {
            _config = config;
        }

        public async Task CheckAsync(IDiscordMessage data, IMutableContext e, Func<Task> next)
        {
            if (e.Executable == null)
            {
                Log.Warning("Command not found, state check cancelling current pipeline instance.");
                return;
            }

            string commandId = e.Executable.ToString();
            if (string.IsNullOrWhiteSpace(commandId))
            {
                return;
            }

            var dbContext = e.GetService<DbContext>();
            if(dbContext == null)
            {
                return;
            }

            var state = await GetCommandStateAsync(
                dbContext,
                commandId,
                (long)data.ChannelId);
            if(state.State)
            {
                await next();
            }
        }

        public async Task<CommandState> GetCommandStateAsync(
            DbContext context, 
            string commandId, 
            long channelId)
        {
            if(string.IsNullOrWhiteSpace(commandId))
            {
                throw new ArgumentNullException("commandId");
            }

            DbSet<CommandState> set = context.Set<CommandState>();
            if(set == null)
            {
                throw new Exception(
                    $"Set of type '{typeof(CommandState).FullName}' could not be found in DbContext.");
            }

            var state = await set.SingleOrDefaultAsync(
                x => x.Name == commandId
                && x.ChannelId == channelId);
            if (state == null)
            {
                var entity = await context.AddAsync(new CommandState()
                {
                    Name = commandId,
                    ChannelId = channelId,
                    State = _config.defaultState
                });
                state = entity.Entity;
                await context.SaveChangesAsync();
            }
            return state;
        }

        /// <summary>
        /// Sets the current state of a command with an existing <paramref name="context"/>. Keep in mind that this does not save the transaction.
        /// </summary>
        /// <param name="context">A initialized DbContext.</param>
        /// <param name="channelId">The identifier of the target channel.</param>
        /// <param name="commandId">The full identifier of the command.</param>
        /// <param name="newValue">The new state of the command.</param>
        /// <seealso cref="CommandState"/>
        public async Task SetCommandStateAsync(
            DbContext context, 
            long channelId,
            string commandId,
            bool newValue)
        {
            DbSet<CommandState> set = context.Set<CommandState>();
            if (set == null)
            {
                throw new Exception(
                    $"Set of type '{typeof(CommandState).FullName}' could not be found in DbContext.");
            }

            var newState = await set.SingleOrDefaultAsync(
                x => x.Name == commandId
                && x.ChannelId == channelId);
            if (newState == null)
            {
                var entity = await set.AddAsync(new CommandState
                {
                    ChannelId = channelId,
                    Name = commandId,
                    State = newValue
                });
                newState = entity.Entity;
            }
            else
            {
                if (newState.State != newValue)
                {
                    newState.State = newValue;
                }
            }
        }
    }

}

namespace Miki.Framework.Commands
{
    public static class Extensions
    {
        public static CommandPipelineBuilder UseStates(
            this CommandPipelineBuilder builder, 
            StatePipelineStage.StateConfiguration config = null)
        {
            if(config == null)
            {
                config = new StatePipelineStage.StateConfiguration();
            }
            return builder.UseStage(new StatePipelineStage(config));
        }
    }
}
