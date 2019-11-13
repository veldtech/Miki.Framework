namespace Miki.Framework
{
    using Discord.Common;
    using Logging;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IMessageReference<T>
        where T : class
	{
        List<Func<T, Task>> Decorators { get; }

        MessageBucketArgs Arguments { get; }

        void PushDecorator(Func<T, Task> fn);
	}

	public class MessageBucketArgs
	{
        public Stream Attachment { get; set; }
		public MessageArgs Properties { get; set; }
		public IDiscordTextChannel Channel { get; set; }
	}

	public class MessageReference : IMessageReference<IDiscordMessage>
	{
		public List<Func<IDiscordMessage, Task>> Decorators  { get; } 
            = new List<Func<IDiscordMessage, Task>>();
        public MessageBucketArgs Arguments { get; }

        public MessageReference(MessageBucketArgs args)
        {
            Arguments = args;
        }

		public void PushDecorator(Func<IDiscordMessage, Task> fn)
		{
            Decorators.Add(fn);
		}
	}


	public class MessageWorker : IMessageWorker<IDiscordMessage>
	{
		private static readonly ConcurrentQueue<IMessageReference<IDiscordMessage>> QueuedMessages 
            = new ConcurrentQueue<IMessageReference<IDiscordMessage>>();

        public int WorkerCount { get; private set; } = 0;

        /// <summary>
        /// Initiates a message worker
        /// </summary>
        /// <param name="workerCount">amount of workers to be initialized, if none are given the default value is PROCESSOR_COUNT * 2</param>
        public MessageWorker(int? workerCount = null)
        {
            var count = workerCount.GetValueOrDefault(Environment.ProcessorCount * 2);
            for(int i = 0; i < count; i++)
            {
                WorkerCount++;
                Task.Run(async () => await Tick());
            }
            WorkerCount = count;
        }

		private static async Task Tick()
		{
			while(true)
			{
				if(QueuedMessages.IsEmpty)
				{
					await Task.Delay(100);
					continue;
				}

				if(QueuedMessages.TryDequeue(out IMessageReference<IDiscordMessage> msg))
				{
					try
					{
						IDiscordMessage m;
						if(msg.Arguments.Attachment == null)
						{
							m = await msg.Arguments.Channel.SendMessageAsync(
                                msg.Arguments.Properties.Content ?? "", 
                                embed: msg.Arguments.Properties.Embed);
						}
						else
						{
							m = await msg.Arguments.Channel.SendFileAsync(
                                msg.Arguments.Attachment, 
                                "file.png", 
                                msg.Arguments.Properties.Content ?? "", 
                                embed: msg.Arguments.Properties.Embed);
							msg.Arguments.Attachment.Dispose();
						}

						if(msg.Decorators.Any())
						{
							ProcessDecorators(msg, m);
						}
					}
					catch(Exception e)
					{
						Log.Error(e);
					}
				}
			}
		}

		private static void ProcessDecorators(IMessageReference<IDiscordMessage> msgRef, IDiscordMessage msg)
		{
			Task.Run(async () =>
			{
				foreach(var x in msgRef.Decorators)
				{
					await x(msg);
				}
			});
		}

        /// <inheritdoc />
        public IMessageReference<IDiscordMessage> CreateRef(MessageBucketArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            return new MessageReference(args);
        }

        /// <inheritdoc />
        public void Execute(IMessageReference<IDiscordMessage> args)
        {
            QueuedMessages.Enqueue(args);
        }
    }

    public interface IMessageWorker<T>
        where T : class
    {
        IMessageReference<T> CreateRef(MessageBucketArgs args);
        
        void Execute(IMessageReference<T> args);
    }
}