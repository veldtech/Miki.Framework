using Microsoft.EntityFrameworkCore;
using Miki.Discord;
using System;

namespace Miki.Common
{
	public class ClientInformation
	{
		public DiscordClientConfigurations ClientConfiguration { get; set; }
	}
}