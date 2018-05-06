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

		private List<IAttachable> attachables = new List<IAttachable>();

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
					await c.SetGameAsync($"{c.ShardId + 1}/{info.TotalShards} | >help");
				};
			}
		}

        public async Task ConnectAsync(string token)
        {
			foreach(DiscordSocketClient c in Client.Shards)
			{
				await c.LoginAsync(TokenType.Bot, token);
				await c.StartAsync();
				await Task.Delay(5000);
			}
			await Task.Delay(-1);
        }

		public void Attach(IAttachable attachable)
		{
			attachables.Add(attachable);
			attachable.AttachTo(this);
		}

		public T GetAttachedObject<T>() where T : IAttachable
		{
			for(int i = 0; i < attachables.Count; i++)
			{
				if(attachables[i] is T)
				{
					return (T)attachables[i];
				}
			}
			return default(T);
		}
	}
}