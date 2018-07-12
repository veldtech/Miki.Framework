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
using StackExchange.Redis.Extensions.Core;
using Miki.Discord;
using Miki.Discord.Caching;

namespace Miki.Framework
{
    public class Bot
    {
		public static Bot Instance { get; private set; }

		public DiscordClient Client { get; private set; }
		public CacheClient CacheClient { get; private set; }

		public ClientInformation Information { private set; get; }

		private List<IAttachable> attachables = new List<IAttachable>();

		// TODO: rework params
		public Bot(int amountShards, ClientInformation cInfo, ICacheClient client, string rabbitUrl)
        {
			Information = cInfo;

			Client = new DiscordClient(new DiscordClientConfigurations
			{
				CacheClient = client,
				RabbitMQExchangeName = "consumer",
				RabbitMQQueueName = "gateway",
				RabbitMQUri = rabbitUrl,
				Token = cInfo.Token
			});

			CacheClient = new CacheClient(
				Client._websocketClient,
				client, Client._apiClient
			);

			if (Instance == null)
				Instance = this;
		}

		public void Attach(IAttachable attachable)
		{
			attachables.Add(attachable);
			attachable.AttachTo(this);
		}

		public T GetAttachedObject<T>() where T : class, IAttachable
		{
			for(int i = 0; i < attachables.Count; i++)
			{
				if(attachables[i] is T)
				{
					return attachables[i] as T;
				}
			}
			return default(T);
		}
	}
}