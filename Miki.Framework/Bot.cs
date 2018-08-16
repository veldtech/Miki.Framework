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
using Miki.Discord;
using Miki.Discord.Caching;
using Miki.Cache;
using Miki.Discord.Common;

namespace Miki.Framework
{
    public class Bot
    {
		public static Bot Instance { get; private set; }

		public DiscordClient Client { get; private set; }
		public CacheClient CacheClient { get; private set; }

		public ClientInformation Information { private set; get; }
		public ICachePool CachePool { private set; get; }

		private List<IAttachable> attachables = new List<IAttachable>();

		// TODO: rework params
		public Bot(IGateway gateway, ICachePool client, ClientInformation cInfo)
        {
			Information = cInfo;
			CachePool = client;

			Client = new DiscordClient(new DiscordClientConfigurations
			{
				Pool = client,
				Gateway = gateway,
				Token = cInfo.Token
			});

			CacheClient = new CacheClient(
				gateway,
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

		public async Task StartAsync()
		{
			await Client._gateway.StartAsync();
		}
	}
}