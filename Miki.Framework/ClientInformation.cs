using Microsoft.EntityFrameworkCore;
using Miki.Discord;
using System;

namespace Miki.Common
{
	public class ClientInformation
	{
		public string Version { get; set; } = "1.0.0";

		public DiscordClientConfigurations ClientConfiguration { get; set; }

		public Func<DbContext> DatabaseContextFactory { get; set; } = () => throw new NotSupportedException("No proper database context factory passed.");
	}
}