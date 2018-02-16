using Discord;
using Discord.WebSocket;

using Miki.Framework.Events;
using Miki.Framework.FileHandling;
using Miki.Common.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using Miki.Common;
using System.Collections.Generic;
using System.Linq;
using Miki.Framework.Internal;

namespace Miki.Framework
{
    public class Bot : IBot
    {
		public event Func<IDiscordGuild, Task> GuildJoin;
		public event Func<IDiscordGuild, Task> GuildLeave;
		public event Func<IDiscordGuild, IDiscordGuild, Task> GuildUpdate;

		public event Func<IDiscordMessage, Task> MessageReceived;

		public event Func<int, Task> ShardConnect;
		public event Func<Exception, int, Task> ShardDisconnect;

		public event Func<IDiscordUser, Task> UserJoin;
		public event Func<IDiscordUser, Task> UserLeave;
		public event Func<IDiscordUser, IDiscordUser, Task> UserUpdate;

		public Func<Exception, Task> OnError = null;

		public static IBot Instance { get; private set; }

        public DiscordShardedClient Client { private set; get; }

		public IDiscordSelfUser CurrentUser 
			=> new RuntimeSelfUser(Client.CurrentUser);

		public ClientInformation Information
			=> clientInformation;

		public IReadOnlyList<IDiscordGuild> Guilds => 
			Client.Guilds.Select(x => new RuntimeGuild(x))
			.ToList();

		public int Latency
			=> Client.Latency;

		public IReadOnlyList<IShard> Shards => throw new NotImplementedException();

		private ClientInformation clientInformation;

		public Bot(ClientInformation info)
        {
            clientInformation = info;
            InitializeBot();
        }

        public async Task ConnectAsync()
        {
			if(clientInformation.Token == "")
			{
				Log.Error("Token wasn't defined.");
			}

            await Client.LoginAsync(TokenType.Bot, clientInformation.Token);

            foreach (DiscordSocketClient client in Client.Shards)
            {
                await client.StartAsync();
                await Task.Delay(10000);
            }

            await Task.Delay(-1);
        }

		public IDiscordMessageChannel GetChannel(ulong id)
			=> new RuntimeMessageChannel(Client.GetChannel(id));

		public IDiscordGuild GetGuild(ulong id)
			=> new RuntimeGuild(Client.GetGuild(id));

		public IDiscordUser GetUser(ulong id)
			=> new RuntimeUser(Client.GetUser(id));

		public DiscordSocketClient GetShardFor(IDiscordGuild guild)
		{
			return Client.GetShardFor((guild as RuntimeGuild).guild);
		}

        private void InitializeBot()
		{
			if(Instance == null)
				Instance = this;

			Log.InitializeLogging(clientInformation);

            Client = new DiscordShardedClient(new DiscordSocketConfig()
            {
                TotalShards = clientInformation.ShardCount,
				ConnectionTimeout = 150000,
			});

			LoadEvents();

			foreach (DiscordSocketClient c in Client.Shards)
            {
                c.Ready += async () =>
                {
                    Log.Message($"shard {c.ShardId} ready!");
                    await c.SetGameAsync($"{c.ShardId + 1}/{Information.ShardCount} | >help");
                };

                c.Connected += async () =>
                {
					await ShardConnect(c.ShardId);
                };

                c.Disconnected += async (e) =>
                {
					await ShardDisconnect(e, c.ShardId);
				};
            }

            Client.Log += Client_Log;
        }
		private void LoadEvents()
		{
			Client.MessageReceived += async (m) 
				=> await MessageReceived?.Invoke(new RuntimeMessage(m));

			Client.JoinedGuild += async (g) 
				=> await GuildJoin?.Invoke(new RuntimeGuild(g));
			Client.LeftGuild += async (g) 
				=> await GuildLeave?.Invoke(new RuntimeGuild(g));
			Client.GuildUpdated += async (gOld, gNew) 
				=> await GuildUpdate?.Invoke(new RuntimeGuild(gOld), new RuntimeGuild(gNew));

			Client.UserJoined += async (u) 
				=> await UserJoin?.Invoke(new RuntimeUser(u));
			Client.UserLeft += async (u) 
				=> await UserLeave?.Invoke(new RuntimeUser(u));
			Client.UserUpdated += async (uOld, uNew) 
				=> await UserUpdate(new RuntimeUser(uOld), new RuntimeUser(uNew));
		}

		private async Task Client_Log(LogMessage arg)
        {
			await Task.Yield();
			if (!string.IsNullOrEmpty(arg.Message))
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(arg.Message);
			}
			if (arg.Exception != null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(arg.Exception);
			}
        }

		public IShard GetShard(int id)
		{
			throw new NotImplementedException();
		}

		IShard IBot.GetShardFor(IDiscordGuild guild)
		{
			throw new NotImplementedException();
		}
	}
}