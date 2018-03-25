using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework
{
	public class MessageBucketArgs
	{
		public MessageProperties properties;
		public IMessageChannel channel;
	}

	public class MessageBucket
    {
		private static ConcurrentQueue<MessageBucketArgs> queuedMessages = new ConcurrentQueue<MessageBucketArgs>();

		private static async Task Tick()
		{
			while(true)
			{
				if (queuedMessages.IsEmpty)
				{
					await Task.Delay(100);
				}

				if (queuedMessages.TryDequeue(out MessageBucketArgs msg))
				{
					try
					{
						await msg.channel.SendMessageAsync(msg.properties.Content.GetValueOrDefault() ?? "", false, msg.properties.Embed.GetValueOrDefault() ?? null);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
		}

		public static void Add(MessageBucketArgs properties)
		{
			queuedMessages.Enqueue(properties);
		}
		public static void AddWorker()
		{
			Task.Run(async () => await Tick());
		}
    }
}
