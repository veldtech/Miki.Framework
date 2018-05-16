using Discord;
using Miki.Common;
using Miki.Framework.Events.Commands;
using Miki.Framework.Events.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class SimpleCommandHandler : CommandHandler
	{
		public IReadOnlyList<CommandEvent> Commands => map.Commands;
		public IReadOnlyList<Module> Modules => map.Modules;

		public SimpleCommandHandler(CommandMap map)
		{
			this.map = map;
		}

		public override async Task CheckAsync(MessageContext context)
		{
			await base.CheckAsync(context);

			foreach (PrefixInstance prefix in Prefixes.Values)
			{
				string identifier = prefix.DefaultValue;

				if (context.message.Channel is IGuildChannel channel)
				{
					identifier = await prefix.GetForGuildAsync(channel.GuildId);
				}

				if (!context.message.Content.StartsWith(identifier))
				{
					continue;
				}

				string command = Regex.Replace(context.message.Content, @"\r\n?|\n", "")
					.Substring(identifier.Length)
					.Split(' ')
					.First();

				CommandEvent eventInstance = map.GetCommandEvent(command);

				if (eventInstance == null)
				{
					return;
				}

				if (GetUserAccessibility(context.message) >= eventInstance.Accessibility)
				{
					if (await eventInstance.IsEnabled(context.message.Channel.Id))
					{
						await eventInstance.Check(context, identifier);
					}
				}
			}
		}

		// TODO: rework this
        public EventAccessibility GetUserAccessibility(IMessage e)
        {
            if (e.Channel == null)
				return EventAccessibility.PUBLIC;

			if ((e.Author as IGuildUser).GuildPermissions.Has(GuildPermission.ManageRoles))
				return EventAccessibility.ADMINONLY;

			return EventAccessibility.PUBLIC;
        }
    }
}