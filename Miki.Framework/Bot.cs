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
		public static Bot Instance { get; private set; }

        public DistributedShardClient Client { private set; get; }

		public ClientInformation Information { get; }
		private ClientInformation information;

		public Bot(DistributedShardConfig info, ClientInformation cInfo)
        {
			information = cInfo;

			if (Instance == null)
				Instance = this;

			Client = new DistributedShardClient(info);

			foreach (DiscordSocketClient c in Client.Shards)
			{
				c.Ready += async () =>
				{
					Log.Message($"shard {c.ShardId} ready!");
					await c.SetGameAsync($"{c.ShardId + 1}/{info.TotalShards} | >help");
				};
			}
		}

        public async Task ConnectAsync(string token)
        {
            await Client.LoginAsync(token, TokenType.Bot);

			await Client.StartAsync();

			await Task.Delay(-1);
        }
	}
}