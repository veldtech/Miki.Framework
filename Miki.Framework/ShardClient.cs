using Discord.WebSocket;
using Miki.Common;
using Miki.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public class ShardClient
    {
		public event Func<IDiscordMessage, Task> MessageRecieved;
		public event Func<int, Task> Ready;

		public List<IDiscordGuild> Guilds => shards
			.SelectMany(x => x.Guilds.Select(y => new RuntimeGuild(y)))
			.Cast<IDiscordGuild>()
			.ToList();

		public List<DiscordSocketClient> Shards
			=> shards;

		protected ClientInformation info;

		private List<DiscordSocketClient> shards = new List<DiscordSocketClient>();

		public ShardClient(ClientInformation clientInfo)
		{
			info = clientInfo;
			
			for(int i = clientInfo.StartShardId; i < clientInfo.StartShardId + clientInfo.ShardCount; i++)
			{
				shards.Add(new DiscordSocketClient(new DiscordSocketConfig()
				{
					ShardId = i,
					ConnectionTimeout = 150000
				}));
			}
		}

		public virtual async Task ConnectAsync()
		{
			foreach(var shard in shards)
			{
				await shard.LoginAsync(Discord.TokenType.Bot, info.Token);
				await shard.StartAsync();
			}
		}

		public virtual async Task DisconnectAsync()
		{
			foreach (var shard in shards)
			{
				await shard.LogoutAsync();
				await shard.StopAsync();
			}
		}
	}
}
