using Discord;
using Discord.WebSocket;
using Miki.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework
{
	public class DistributedShardClient
	{
		List<DiscordSocketClient> socketClients = new List<DiscordSocketClient>();

		public event Func<SocketMessage, Task> MessageReceived;
		public event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;
		public event Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;

		public DistributedShardClient(DiscordSocketConfig config)
		{
			if (!config.ShardId.HasValue)
				throw new ArgumentNullException("ShardId can not be lower null");

			for (int i = config.ShardId.Value; i < config.TotalShards + config.ShardId.Value; i++)
			{
				config.ShardId = i;
				socketClients.Add(new DiscordSocketClient(config));
			}

			foreach(var client in socketClients)
			{
				client.MessageReceived += async (msg) =>
				{
					if (MessageReceived != null)
					{
						await MessageReceived(msg);
					}
				};

				client.MessageDeleted += async (msg, channel) =>
				{
					if (MessageDeleted != null)
					{
						await MessageDeleted(msg, channel);
					}
				};

				client.MessageUpdated += async (msg, newMsg, channel) =>
				{
					if (MessageUpdated != null)
					{
						await MessageUpdated(msg, newMsg, channel);
					}
				};
			}
		}

		public async Task LoginAsync(string token, TokenType tokenType = TokenType.Bot)
		{
			foreach (var client in socketClients)
			{
				await client.LoginAsync(tokenType, token);
			}
		}

		public async Task LogoutAsync()
		{
			foreach (var client in socketClients)
			{
				await client.LogoutAsync();
			}
		}

		public async Task StartAsync()
		{
			foreach(var client in socketClients)
			{
				await client.StartAsync();
			}
		}

		public async Task StopAsync()
		{
			foreach (var client in socketClients)
			{
				await client.StopAsync();
			}
		}
	}
}