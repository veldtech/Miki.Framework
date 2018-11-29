using Miki.Common;
using Miki.Discord;
using System.Collections.Generic;

namespace Miki.Framework
{
	public class DiscordBot
	{
		public static DiscordBot Instance { get; private set; }

		public DiscordClient Discord { get; private set; }

		public ClientInformation Information { get; private set; }

		private List<IAttachable> _attachables = new List<IAttachable>();

		public DiscordBot(ClientInformation information)
		{
			Discord = new DiscordClient(information.ClientConfiguration);
			Information = information;

			if (Instance == null)
			{
				Instance = this;
			}
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