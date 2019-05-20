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

		void ProcessOnException(Func<MessageExceptionArguments, Task> fn);
	}

	public class MessageBucketArgs
	{
		public Stream attachment;
		public MessageArgs properties;
		public IDiscordTextChannel channel;
	}

	public class MessageReference : IMessageReference
	{
		public List<Func<IDiscordMessage, Task>> SuccessActions = new List<Func<IDiscordMessage, Task>>();
		public List<Func<MessageExceptionArguments, Task>> ExceptionActions = new List<Func<MessageExceptionArguments, Task>>();

		public MessageBucketArgs Arguments;

		public void ProcessAfterComplete(Func<IDiscordMessage, Task> fn)
		{
			SuccessActions.Add(fn);
		}

		public void ProcessOnException(Func<MessageExceptionArguments, Task> fn)
		{
			ExceptionActions.Add(fn);
		}
	}

	public class MessageExceptionArguments
	{
		public MessageExceptionArguments(IMessageReference reference, Exception exception)
		{
			Reference = reference;
			Exception = exception;
			LogException = true;
		}

		public IMessageReference Reference { get; }

		public Exception Exception { get; }

		public bool LogException { get; set; }
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

						if (msg.SuccessActions.Count > 0)
						{
							ProcessDecorators(msg, m);
						}
					}
					catch (Exception e)
					{
						ProcessExceptionDecorators(msg, new MessageExceptionArguments(msg, e));
					}
				}
			}
		}

		static void ProcessDecorators(MessageReference msgRef, IDiscordMessage msg)
		{
			if (msgRef.SuccessActions.Count == 0)
			{
				return;
			}

			Task.Run(async () =>
			{
				foreach(var x in msgRef.SuccessActions)
				{
					await x(msg);
				}
			});
		}

		static void ProcessExceptionDecorators(MessageReference msgRef, MessageExceptionArguments args)
		{
			if (msgRef.ExceptionActions.Count == 0)
			{
				Log.Error(args.Exception);
				return;
			}

			Task.Run(async () =>
			{
				foreach (var x in msgRef.ExceptionActions)
				{
					await x(args);
				}

				if (args.LogException)
				{
					Log.Error(args.Exception);
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