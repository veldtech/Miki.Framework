using Discord;
using Discord.WebSocket;
using Miki.Common.Interfaces;
using Miki.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Common
{
    public class RuntimeMessageChannel : IDiscordMessageChannel, IProxy<IChannel>
    {
        public IChannel channel;

        public RuntimeMessageChannel(IChannel c)
        {
            channel = c;
        }

        public IDiscordGuild Guild
        {
            get
            {
                return new RuntimeGuild((channel as IGuildChannel).Guild);
            }
        }

        public ulong Id
        {
            get
            {
                return channel.Id;
            }
        }

        public string Name
        {
            get
            {
                return channel.Name;
            }
        }

        public bool Nsfw => channel.IsNsfw;

        public async Task DeleteMessagesAsync(List<IDiscordMessage> messages)
        {
            await (channel as IMessageChannel).DeleteMessagesAsync(
				messages.Select(x => (x as IProxy<IMessage>).ToNativeObject())
			);
        }

        public async Task<List<IDiscordMessage>> GetMessagesAsync(int amount = 100)
        {
            IEnumerable<IMessage> messages = await (channel as IMessageChannel).GetMessagesAsync(amount)
				.Flatten();

			return messages.Select(x => new RuntimeMessage(x))
				.Cast<IDiscordMessage>()
				.ToList();
        }

        public async Task<List<IDiscordUser>> GetUsersAsync()
        {
            IEnumerable<IUser> users = await channel.GetUsersAsync()
				.Flatten();

            return users.Select(x => new RuntimeUser(x))
				.Cast<IDiscordUser>()
				.ToList(); 
        }

        public async Task<IDiscordMessage> SendFileAsync(string path)
        {
            return new RuntimeMessage(await (channel as IMessageChannel).SendFileAsync(path));
        }

        public async Task<IDiscordMessage> SendFileAsync(MemoryStream stream, string extension)
        {
            return new RuntimeMessage(await (channel as IMessageChannel)?.SendFileAsync(stream, extension));
        }

        public async Task<IDiscordMessage> SendMessageAsync(string message)
        {
            RuntimeMessage m = null;
            try
            {
                m = new RuntimeMessage(await (channel as IMessageChannel).SendMessage(message));
                Log.Message("Sent message to channel " + channel.Name);
            }
            catch (Exception ex)
            {
                Log.ErrorAt("SendMessage", ex.Message);
            }
            return m;
        }

        public IChannel ToNativeObject()
        {
            return channel;
        }

        public async Task SendTypingAsync()
        {
            await (channel as IMessageChannel).TriggerTypingAsync();
        }

		public void QueueMessageAsync(string message)
		{
			Task.Run(async () => await SendMessageAsync(message));
		}
	}
}