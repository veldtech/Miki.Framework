using Discord;
using Discord.WebSocket;
using Miki.Common.Interfaces;
using Miki.Framework;
using Miki.Framework.Internal;
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
			=> new RuntimeGuild((channel as IGuildChannel).Guild);

        public ulong Id
			=> channel.Id;

		public string Name
			=> channel.Name;

		public bool Nsfw 
			=> (channel as ITextChannel).IsNsfw;

        public async Task DeleteMessagesAsync(List<IDiscordMessage> messages)
        {
            await (channel as ITextChannel).DeleteMessagesAsync(
				messages.Select(x => (x as IProxy<IMessage>).ToNativeObject())
			);
        }

        public async Task<List<IDiscordMessage>> GetMessagesAsync(int amount = 100)
        {
            IEnumerable<IMessage> messages = await (channel as ITextChannel).GetMessagesAsync(amount)
				.FlattenAsync();

			return messages.Select(x => new RuntimeMessage(x))
				.Cast<IDiscordMessage>()
				.ToList();
        }

        public async Task<List<IDiscordUser>> GetUsersAsync()
        {
            IEnumerable<IUser> users = await channel.GetUsersAsync()
				.FlattenAsync();

            return users.Select(x => new RuntimeUser(x))
				.Cast<IDiscordUser>()
				.ToList(); 
        }

        public async Task<IDiscordMessage> SendFileAsync(string path, string message = null)
        {
            return new RuntimeMessage(await (channel as IMessageChannel).SendFileAsync(path, message));
        }

        public async Task<IDiscordMessage> SendFileAsync(Stream stream, string fileName, string message = null)
        {
            return new RuntimeMessage(await (channel as IMessageChannel)?.SendFileAsync(stream, fileName, message));
        }

        public async Task<IDiscordMessage> SendMessageAsync(string message, IDiscordEmbed embed = null)
        {
            RuntimeMessage m = null;
            try
            {
                m = new RuntimeMessage(await (channel as IMessageChannel).SendMessageAsync(message ?? "", false, (embed as RuntimeEmbed)?.embed.Build() ?? null));
                Log.Message("Sent message to channel " + channel.Name);
				return m;
			}
			catch (Exception ex)
            {
                Log.ErrorAt("SendMessage", ex.Message);
            }
			return null;
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