using Discord;
using Discord.WebSocket;
using Miki.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework
{
	public class DistributedShardConfig : DiscordSocketConfig
	{
		public int AmountShards { get; set; }
	}

	public class DistributedShardClient
	{
		List<DiscordSocketClient> socketClients = new List<DiscordSocketClient>();

		public event Func<SocketMessage, Task> MessageReceived;
		public event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;
		public event Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;
		public event Func<SocketGuildUser, Task> UserLeft;
		public event Func<SocketUser, SocketUser, Task> UserUpdated;
		public event Func<SocketGuildUser, Task> UserJoined;
		public event Func<SocketGuild, Task> LeftGuild;
		public event Func<SocketGuild, Task> JoinedGuild;
		public event Func<SocketGuild, SocketGuild, Task> GuildUpdated;
		public event Func<DiscordSocketClient, Task> ShardConnected;
		public event Func<Exception, DiscordSocketClient, Task> ShardDisconnected;

		public IReadOnlyList<DiscordSocketClient> Shards => socketClients;
		public IReadOnlyList<SocketGuild> Guilds
		{
			get
			{
				List<SocketGuild> guilds = new List<SocketGuild>();
				foreach(var g in socketClients)
				{
					guilds.AddRange(g.Guilds);
				}
				return guilds;
			}
		}

		public ISelfUser CurrentUser => socketClients[0].CurrentUser;

		public DistributedShardClient(DistributedShardConfig config)
		{
			if (!config.ShardId.HasValue)
				throw new ArgumentNullException("ShardId can not be lower null");

			for (int i = config.ShardId.Value; i < config.AmountShards + config.ShardId.Value; i++)
			{
				Console.WriteLine(i);

				var client = new DiscordSocketClient(new DiscordSocketConfig()
				{
					AlwaysDownloadUsers = config.AlwaysDownloadUsers,
					ConnectionTimeout = config.ConnectionTimeout,
					DefaultRetryMode = config.DefaultRetryMode,
					GatewayHost = config.GatewayHost,
					HandlerTimeout = config.HandlerTimeout,
					LargeThreshold = config.LargeThreshold,
					LogLevel = config.LogLevel,
					MessageCacheSize = config.MessageCacheSize,
					RestClientProvider = config.RestClientProvider,
					ShardId = i,
					TotalShards = config.TotalShards,
					UdpSocketProvider = config.UdpSocketProvider,
					WebSocketProvider = config.WebSocketProvider
				});

				socketClients.Add(client);

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

				client.UserLeft += async (user) =>
				{
					if (UserLeft != null)
					{
						await UserLeft(user);
					}
				};
				client.UserJoined += async (user) =>
				{
					if (UserJoined != null)
					{
						await UserJoined(user);
					}
				};
				client.UserUpdated += async (user, otherUser) =>
				{
					if (UserUpdated != null)
					{
						await UserUpdated(user, otherUser);
					}
				};

				client.JoinedGuild += async (user) =>
				{
					if (JoinedGuild != null)
					{
						await JoinedGuild(user);
					}
				};
				client.LeftGuild += async (user) =>
				{
					if (LeftGuild != null)
					{
						await LeftGuild(user);
					}
				};
				client.GuildUpdated += async (guild, guildNew) =>
				{
					if (GuildUpdated != null)
					{
						await GuildUpdated(guild, guildNew);
					}
				};

				client.Connected += async () =>
				{
					await ShardConnected(client);
				};
				client.Disconnected += async (ex) =>
				{
					await ShardDisconnected(ex, client);
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

		public SocketGuild GetGuild(ulong id)
		{
			foreach(var g in socketClients)
			{
				var guild = g.GetGuild(id);
				if (guild != null)
					return guild;
			}
			return null;
		}

		public SocketChannel GetChannel(ulong id)
		{
			foreach (var g in socketClients)
			{
				var channel = g.GetChannel(id);
				if (channel != null)
					return channel;
			}
			return null;
		}

		public SocketUser GetUser(ulong id)
		{
			foreach (var g in socketClients)
			{
				var user = g.GetUser(id);
				if (user != null)
					return user;
			}
			return null;
		}

		public DiscordSocketClient GetShardFor(IGuild guild)
			=> socketClients.FirstOrDefault(x => x.GetGuild(guild.Id) != null);
	}
}