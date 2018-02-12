using Discord;
using Discord.WebSocket;

using Miki.Framework.Addons;
using Miki.Framework.Events;
using Miki.Framework.FileHandling;
using Miki.Common.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using Miki.Common;

namespace Miki.Framework
{
    public partial class Bot
    {
        public AddonManager Addons { private set; get; }
        public DiscordShardedClient Client { private set; get; }
        public EventSystem Events { internal set; get; }

        public Func<Exception, Task> OnError = null;

        public string Name => clientInformation.Name;
		public string Version => clientInformation.Version;

        public static Bot instance;

        internal ClientInformation clientInformation;

        public Bot(ClientInformation info)
        {
            clientInformation = info;
            InitializeBot().GetAwaiter().GetResult();
        }

        public void AddDeveloper(ulong id)
        {
            Events.Developers.Add(id);
        }
        public void AddDeveloper(IDiscordUser user)
        {
            Events.Developers.Add(user.Id);
        }
        public void AddDeveloper(IUser user)
        {
            Events.Developers.Add(user.Id);
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

        public int GetShardId()
        {
            return clientInformation.ShardId;
        }

		public DiscordSocketClient GetShardFor(IDiscordGuild guild)
		{
			return Client.GetShardFor((guild as RuntimeGuild).guild);
		}

        public int GetTotalShards()
        {
            return clientInformation.ShardCount;
        }

        private async Task InitializeBot()
        {
            instance = this;

			Log.InitializeLogging(clientInformation);

            Client = new DiscordShardedClient(new DiscordSocketConfig()
            {
                TotalShards = clientInformation.ShardCount,
                LogLevel = LogSeverity.Info,
				ConnectionTimeout = 150000,
				LargeThreshold = 250,		
			});

            LoadEvents();

            EventSystem.RegisterBot(this);

            Addons = new AddonManager();
            await Addons.Load(this);

            if (clientInformation.EventLoaderMethod != null)
            {
                await clientInformation.EventLoaderMethod(this);
            }

            foreach (DiscordSocketClient c in Client.Shards)
            {
                c.Ready += async () =>
                {
                    Log.Message($"shard {c.ShardId} ready!");
                    await c.SetGameAsync($"{c.ShardId + 1}/{GetTotalShards()} | >help");
                };

                c.Connected += async () =>
                {
                    Log.Message($"{c.ShardId}| Connected!");
					await Task.Yield();
                };

                c.Disconnected += async (e) =>
                {
                    Log.ErrorAt(c.ShardId + "| Disconnected", e.Message);
					await Task.Yield();
				};
            }

            Client.Log += Client_Log;
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
    }
}