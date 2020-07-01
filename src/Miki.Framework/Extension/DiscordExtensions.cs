namespace Miki.Framework
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Common.Builders;
    using Discord;
    using Discord.Common;
    using Framework;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods for Miki.Discord
    /// </summary>
    public static class DiscordExtensions
    {
        public static Task QueueAsync(
            this DiscordEmbed embed,
            IContext context,
            IDiscordTextChannel channel,
            string content = "",
            Func<IMessageReference<IDiscordMessage>, IMessageReference<IDiscordMessage>> modifier = null)
        {
            return QueueAsync(
                embed,
                context.GetService<IMessageWorker<IDiscordMessage>>(),
                channel,
                content,
                modifier);
        }

        public static Task QueueAsync(
            this DiscordEmbed embed,
            IMessageWorker<IDiscordMessage> worker,
            IDiscordTextChannel channel,
            string content = "",
            Func<IMessageReference<IDiscordMessage>, IMessageReference<IDiscordMessage>> modifier = null)
        {
            /*var currentUser = await client.GetSelfAsync();
            var currentGuildUser = await guildChannel.GetUserAsync(currentUser.Id);
            var permissions = await guildChannel.GetPermissionsAsync(currentGuildUser);
            if(permissions.HasFlag(GuildPermission.EmbedLinks))
            {
                        if(!string.IsNullOrEmpty(embed.Image?.Url ?? ""))
            {
                using HttpClient wc = new HttpClient(embed.Image.Url);
                await using Stream ms = await wc.GetStreamAsync();
                return channel.QueueMessage(worker, stream: ms, message: embed.ToMessageBuilder().Build());
            }

            if(!string.IsNullOrEmpty(embed.Thumbnail?.Url ?? ""))
            {
                using HttpClient wc = new HttpClient(embed.Thumbnail.Url);
                await using Stream ms = await wc.GetStreamAsync();
                return channel.QueueMessage(worker, stream: ms, message: embed.ToMessageBuilder().Build());
            }

            return channel.QueueMessage(worker, message: embed.ToMessageBuilder().Build());
            }*/
            QueueMessage(channel, worker, embed, content, modifier: modifier);
            return Task.CompletedTask;
        }

        public static IMessageReference<T> ThenWait<T>(this IMessageReference<T> reference, int milliseconds)
            where T : class
        {
            reference.PushDecorator(_ => Task.Delay(milliseconds));
            return reference;
        }

        public static IMessageReference<IDiscordMessage> ThenDelete(this IMessageReference<IDiscordMessage> reference)
        {
            reference.PushDecorator(async (msg) => { await msg.DeleteAsync(); });
            return reference;
        }

        public static IMessageReference<IDiscordMessage> ThenEdit(
            this IMessageReference<IDiscordMessage> reference,
            string message = "",
            DiscordEmbed embed = null)
        {
            reference.PushDecorator(x
                => x.EditAsync(new EditMessageArgs(message, embed)));
            return reference;
        }

        public static IMessageReference<T> Then<T>(this IMessageReference<T> reference, Func<T, Task> fn)
            where T : class
        {
            reference.PushDecorator(fn);
            return reference;
        }

        public static async Task<IDiscordMessage> EditAsync(this DiscordEmbed embed, IDiscordMessage msg)
        {
            var channel = await msg.GetChannelAsync();
            if(channel is IDiscordGuildChannel guildChannel)
            {
                var currentGuildUser = await guildChannel.GetSelfAsync();
                var permissions = await guildChannel.GetPermissionsAsync(currentGuildUser);

                if(!permissions.HasFlag(GuildPermission.EmbedLinks))
                {
                    return await msg.EditAsync(new EditMessageArgs(embed.ToMessageBuilder().Build()));
                }
            }
            return await msg.EditAsync(new EditMessageArgs("", embed));
        }

        public static async Task<IDiscordMessage> SendToChannelAsync(
            this DiscordEmbed embed, IDiscordTextChannel channel)
        {
            if(channel is IDiscordGuildChannel guildChannel)
            {
                var currentGuildUser = await guildChannel.GetSelfAsync();
                var permissions = await guildChannel.GetPermissionsAsync(currentGuildUser);

                if(!permissions.HasFlag(GuildPermission.EmbedLinks))
                {
                    if(!string.IsNullOrEmpty(embed.Image?.Url ?? ""))
                    {
                        using WebClient wc = new WebClient();
                        byte[] image = wc.DownloadData(embed.Image.Url);
                        await using MemoryStream ms = new MemoryStream(image);
                        return await channel.SendFileAsync(ms, "output.png", embed.ToMessageBuilder().Build());
                    }
                    else if(!string.IsNullOrEmpty(embed.Thumbnail?.Url ?? ""))
                    {
                        using WebClient wc = new WebClient();
                        byte[] image = wc.DownloadData(embed.Thumbnail.Url);
                        await using MemoryStream ms = new MemoryStream(image);
                        return await channel.SendFileAsync(ms, "output.png", embed.ToMessageBuilder().Build());
                    }
                    else
                    {
                        return await channel.SendMessageAsync(embed.ToMessageBuilder().Build());
                    }
                }
            }
            return await channel.SendMessageAsync("", embed: embed);
        }

        public static async Task<IDiscordMessage> SendToUserAsync(this DiscordEmbed embed, IDiscordUser user)
        {
            var dmChannel = await user.GetDMChannelAsync();
            return await dmChannel.SendMessageAsync("", false, embed);
        }

        [Obsolete("Consider using the full method.")]
        public static void QueueMessage(
            this IDiscordTextChannel channel,
            IContext context,
            string message)
            => QueueMessage(channel, context, null, message);

        public static void QueueMessage(
            this IDiscordTextChannel channel,
            IContext context,
            DiscordEmbed embed = null,
            string message = "",
            Stream stream = null,
            Func<IMessageReference<IDiscordMessage>, IMessageReference<IDiscordMessage>> modifier = null)
        {
            var worker = context.GetService<IMessageWorker<IDiscordMessage>>();
            QueueMessage(channel, worker, embed, message, stream, modifier);
        }

        public static void QueueMessage(
            this IDiscordTextChannel channel,
            IMessageWorker<IDiscordMessage> worker,
            DiscordEmbed embed = null,
            string message = "",
            Stream stream = null,
            Func<IMessageReference<IDiscordMessage>, IMessageReference<IDiscordMessage>> modifier = null)
        {
            if(worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }
            var @ref = worker.CreateRef(new MessageBucketArgs()
            {
                Attachment = stream,
                Channel = channel,
                Properties = new MessageArgs(message, embed)
            });
            if(modifier != null)
            {
                @ref = modifier(@ref);
            }
            worker.Execute(@ref);
        }

        /// <summary>
        /// Transforms an embed to a formatted Discord message builder.
        /// </summary>
        public static MessageBuilder ToMessageBuilder(this DiscordEmbed embed)
        {
            var messageBuilder = new MessageBuilder();

            if(embed.Author != null)
            {
                messageBuilder.AppendText(embed.Author.Name, MessageFormatting.Bold);
            }

            messageBuilder.AppendText(embed.Title, MessageFormatting.Bold)
             .AppendText(embed.Description);

            if(embed.Fields != null)
            {
                foreach(EmbedField f in embed.Fields)
                {
                    messageBuilder.AppendText(f.Title, MessageFormatting.Underlined)
                     .AppendText(f.Content)
                     .NewLine();
                }
            }

            if(embed.Footer != null)
            {
                messageBuilder.AppendText(embed.Footer.Text, MessageFormatting.Italic);
            }

            return messageBuilder;
        }
    }
}