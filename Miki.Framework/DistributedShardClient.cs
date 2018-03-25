using Discord;
using Discord.WebSocket;
using Miki.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public class DistributedShardClient : ShardClient
	{
		public DistributedShardClient(ClientInformation info) : base(info)
		{ }

  		public async Task AddAsync(int shardId)
		{
			DiscordSocketClient socketClient = new DiscordSocketClient(new DiscordSocketConfig()
			{
				ShardId = shardId,
				ConnectionTimeout = 150000,
			});

			await socketClient.LoginAsync(TokenType.Bot, info.Token);
			await socketClient.StartAsync();
		}

		public async Task RemoveAsync(int shardId)
		{
			var client = Shards.FirstOrDefault(x => x.ShardId == shardId);

			if (client == null)
			{
				return;
			}

			await client.StopAsync();
			Shards.Remove(client);
		}
	}
}