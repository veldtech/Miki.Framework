using Discord;
using Discord.WebSocket;

using Miki.Framework.Events;
using Miki.Framework.FileHandling;
using System;
using System.IO;
using System.Threading.Tasks;
using Miki.Common;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Miki.Framework
{
    public class Bot
    {
		public Func<Exception, Task> OnError = null;

		public static Bot Instance { get; private set; }

        public DiscordShardedClient Client { private set; get; }

		public ClientInformation Information
			=> clientInformation;

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

			foreach(var shard in Client.Shards)
			{
				await shard.StartAsync();
				await Task.Delay(6000);
			}

			await Task.Delay(-1);
        }

        private void InitializeBot()
		{
			if(Instance == null)
				Instance = this;

            Client = new DiscordShardedClient(new DiscordSocketConfig()
            {
                TotalShards = clientInformation.ShardCount,
				ConnectionTimeout = 150000,
				AlwaysDownloadUsers = true
			});

			foreach (DiscordSocketClient c in Client.Shards)
            {
                c.Ready += async () =>
                {
                    Log.Message($"shard {c.ShardId} ready!");
                    await c.SetGameAsync($"{c.ShardId + 1}/{Information.ShardCount} | >help");
                };
            }
        }
	}
}