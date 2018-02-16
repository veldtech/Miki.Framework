using Discord;
using Discord.WebSocket;
using Miki.Common.Interfaces;
using Miki.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Common
{
    public class RuntimeMessage : IDiscordMessage, IProxy<IMessage>
    {
        private IMessage messageData = null;

        private RuntimeGuild guild = null;
        private RuntimeMessageChannel channel = null;
        private RuntimeUser user = null;
        private RuntimeClient client = null;

        public ulong Id
			=> messageData.Id;

		public IDiscordUser Author
			=> user;

		public IDiscordUser Bot
			=> Guild.GetCurrentUserAsync().GetAwaiter().GetResult();

		public IDiscordMessageChannel Channel
			=> channel;

		public Interfaces.IDiscordClient Discord
			=> client;

		public IDiscordGuild Guild
			=> guild;

		public string Content
			=> messageData.Content;

		public IReadOnlyCollection<IDiscordAttachment> Attachments
			=> messageData.Attachments
				.Select(x => new RuntimeAttachment(x))
				.Cast<IDiscordAttachment>()
				.ToList();

		public IReadOnlyCollection<ulong> MentionedUserIds
			=> messageData.MentionedUserIds;

		public IReadOnlyCollection<ulong> MentionedRoleIds
			=> messageData.MentionedRoleIds;

		public DateTimeOffset Timestamp 
			=> messageData.Timestamp;

        public int ShardId 
			=> client.ShardId;

        public Dictionary<DiscordEmoji, DiscordReactionMetadata> Reactions
        {
            get
            {
                IReadOnlyDictionary<IEmote, ReactionMetadata> x = (messageData as IUserMessage).Reactions;
                Dictionary<DiscordEmoji, DiscordReactionMetadata> emojis = new Dictionary<DiscordEmoji, DiscordReactionMetadata>();
                foreach (Emoji y in x.Keys)
                {
                    DiscordEmoji newEmoji = new DiscordEmoji();
                    newEmoji.Name = y.Name;

                    DiscordReactionMetadata metadata = new DiscordReactionMetadata();
                    metadata.IsMe = x[y].IsMe;
                    metadata.ReactionCount = x[y].ReactionCount;

                    emojis.Add(newEmoji, metadata);
                }
                return emojis;
            }
        }

        public IReadOnlyCollection<ulong> MentionedChannelIds 
			=> messageData.MentionedChannelIds;

		public string ResolvedContent 
			=> (messageData as IUserMessage).Resolve(
				TagHandling.NameNoPrefix,
				TagHandling.NameNoPrefix,
				TagHandling.NameNoPrefix,
				TagHandling.NameNoPrefix,
				TagHandling.NameNoPrefix
			);

		public RuntimeMessage(IMessage msg)
        {
            messageData = msg;

            if (msg.Author != null) user = new RuntimeUser(msg.Author);
            if (msg.Channel != null) channel = new RuntimeMessageChannel(msg.Channel);
            IGuild g = (messageData.Channel as IGuildChannel)?.Guild;

            if (g != null)
            {
                guild = new RuntimeGuild(g);
            }
        }
        public RuntimeMessage(IMessage msg, DiscordSocketClient c)
        {
            messageData = msg;

            if (msg.Author != null) user = new RuntimeUser(msg.Author);
            if (msg.Channel != null) channel = new RuntimeMessageChannel(msg.Channel);
            IGuild g = (messageData.Author as IGuildUser)?.Guild;
            if (g != null)
            {
                guild = new RuntimeGuild(g);
            }
            client = new RuntimeClient(c);
        }

		public RuntimeMessage()
		{
		}

		public async Task AddReaction(string emoji)
        {
            await (messageData as IUserMessage).AddReactionAsync(new Emoji(emoji));
        }

        public async Task DeleteAsync()
        {
            if ((await Guild.GetCurrentUserAsync()).HasPermissions(Channel, DiscordGuildPermission.ManageMessages))
            {
                await messageData.DeleteAsync();
            }
        }

        public void Modify(string message, IDiscordEmbed embed)
        {
			Task.Run(async () => await (messageData as IUserMessage)?.ModifyAsync(x =>
			{
				if(!string.IsNullOrEmpty(message))
					x.Content = message;

				if (embed != null)
					x.Embed = (embed as RuntimeEmbed).ToNativeObject();
			}));
        }

        public async Task PinAsync()
        {
            await (messageData as IUserMessage)?.PinAsync();
        }

        public async Task UnpinAsync()
        {
            await (messageData as IUserMessage)?.UnpinAsync();
        }

        public IMessage ToNativeObject()
        {
            return messageData;
        }
	}
}