using Miki.Discord;
using Miki.Discord.Common;
using Miki.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Miki.Framework
{
	public interface IMessageReference
	{
		void ProcessAfterComplete(Func<IDiscordMessage, Task> fn);
	}

	public class MessageBucketArgs
	{
        public Stream attachment;
        public MessageArgs properties;
		public IDiscordTextChannel channel;
	}

	public class MessageReference : IMessageReference
	{
		public List<Func<IDiscordMessage, Task>> AllActions = new List<Func<IDiscordMessage, Task>>();

		public MessageBucketArgs Arguments;

		public void ProcessAfterComplete(Func<IDiscordMessage, Task> fn)
		{
			AllActions.Add(fn);
		}
	}


	public class MessageBucket
	{
		private readonly static ConcurrentQueue<MessageReference> queuedMessages = new ConcurrentQueue<MessageReference>();

		private static async Task Tick()
		{
			while (true)
			{
				if (queuedMessages.IsEmpty)
				{
					await Task.Delay(100);
                    continue;
				}

				if (queuedMessages.TryDequeue(out MessageReference msg))
				{
					try
					{
                        IDiscordMessage m = null;
                        if (msg.Arguments.attachment == null)
                        {
                             m = await msg.Arguments.channel.SendMessageAsync(msg.Arguments.properties.content ?? "", false, msg.Arguments.properties.embed ?? null);
                        }
                        else
                        {
                            m = await msg.Arguments.channel.SendFileAsync(msg.Arguments.attachment, "file.png", msg.Arguments.properties.content ?? "", false, msg.Arguments.properties.embed ?? null);
                            msg.Arguments.attachment.Dispose();
                        }

                        if (msg.AllActions.Count > 0)
						{
							ProcessDecorators(msg, m);
						}
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
		}

		static void ProcessDecorators(MessageReference msgRef, IDiscordMessage msg)
		{
			Task.Run(async () =>
			{
				foreach(var x in msgRef.AllActions)
				{
					await x(msg);
				}
			});
		}

		public static IMessageReference Add(MessageBucketArgs properties)
		{
			MessageReference msg = new MessageReference();
			msg.Arguments = properties ?? throw new ArgumentNullException();
			queuedMessages.Enqueue(msg);
			return msg;
		}

		public static void AddWorker()
		{
			Task.Run(async () => await Tick());
		}
	}
}