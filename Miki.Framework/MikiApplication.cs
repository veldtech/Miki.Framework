using Microsoft.Extensions.DependencyInjection;
using Miki.Common;
using Miki.Discord;
using System.Collections.Generic;

namespace Miki.Framework
{
	public class MikiApplication
	{
		public static MikiApplication Instance { get; private set; }

		public DiscordClient Discord { get; private set; }

		public ClientInformation Information { get; private set; }

		public IServiceCollection Services { get; private set; }

		private readonly List<IAttachable> _attachables = new List<IAttachable>();

		public MikiApplication(ClientInformation information)
		{
			Discord = new DiscordClient(information.ClientConfiguration);
			Services = new ServiceCollection();

			Information = information;

			if (Instance == null)
			{
				Instance = this;
			}
		}

		public void AddService<T>(T value)
		{
			Services.Add(new ServiceDescriptor(typeof(T), value));
		}

		public void Attach(IAttachable attachable)
		{
			_attachables.Add(attachable);
			attachable.AttachTo(this);
		}

		public T GetAttachedObject<T>() where T : class, IAttachable
		{
			for (int i = 0; i < _attachables.Count; i++)
			{
				if (_attachables[i] is T)
				{
					return _attachables[i] as T;
				}
			}
			return default(T);
		}
	}
}