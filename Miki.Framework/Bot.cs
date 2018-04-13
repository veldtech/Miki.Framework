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

        public DiscordShardedClient Client { private set; get; }

		public ClientInformation Information { private set; get; }

		public Bot(int amountShards, DiscordSocketConfig info, ClientInformation cInfo)
        {
			Information = cInfo;

			if (Instance == null)
				Instance = this;

			int[] shardIds = new int[amountShards];

			for (int i = 0; i < amountShards; i++)
			{
				shardIds[i] = info.ShardId.GetValueOrDefault() + i;
			}

			info.ShardId = null;

			Client = new DiscordShardedClient(shardIds, info);

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
            await Client.LoginAsync(TokenType.Bot, token);

			await Client.StartAsync();

			await Task.Delay(-1);
        }
	}
}