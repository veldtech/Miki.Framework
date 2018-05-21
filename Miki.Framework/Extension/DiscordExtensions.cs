using Discord;
using Miki.Common.Builders;
using Miki.Framework;
using Miki.Framework.Languages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public static class DiscordExtensions
    {
		public static Color Lerp(this Color colorA, Color colorB, float t)
		{
			t = Mathm.Clamp(t, 0, 1);
			return new Color(
				(1 - t) * colorA.R + t * colorB.R,
				(1 - t) * colorA.G + t * colorB.G,
				(1 - t) * colorA.B + t * colorB.B
			);
		}

		public static EmbedBuilder SetAuthor(this EmbedBuilder b, string text, string iconurl, string url)
		{
			b.Author = new EmbedAuthorBuilder
			{
				Name = text,
				IconUrl = iconurl,
				Url = url
			};
			return b;
		}
		public static EmbedBuilder AddInlineField(this EmbedBuilder b, string header, object value)
			=>	b.AddField(header, value, true);

		public static EmbedBuilder AddInlineField(this EmbedBuilder b, string resourceHeader, string resourceValue, ulong channelId)
			=> b.AddInlineField(Locale.GetString(channelId, resourceHeader), Locale.GetString(channelId, resourceValue));

		public static void QueueToChannel(this Embed embed, IMessageChannel channel)
		{
			MessageBucket.Add(new MessageBucketArgs()
			{
				properties = new MessageProperties()
				{
					Embed = embed
				},
				channel = channel
			});
		}
		public static void QueueToUser(this Embed embed, IUser user)
		{
			MessageBucket.Add(new MessageBucketArgs()
			{
				properties = new MessageProperties()
				{
					Embed = embed
				},
				channel = user.GetOrCreateDMChannelAsync().GetAwaiter().GetResult()
			});
		}
		public static async Task<IUserMessage> SendToChannel(this Embed embed, IMessageChannel channel)
		{
			if (!(await (channel as IGuildChannel).Guild.GetCurrentUserAsync()).GuildPermissions.Has(GuildPermission.EmbedLinks))
			{
				if (!embed.Image.HasValue)
				{
					return await channel.SendMessageAsync(embed.ToMessageBuilder().Build());
				}

				using (WebClient wc = new WebClient())
				{
					byte[] image = wc.DownloadData(embed.Image.Value.Url);
					using (MemoryStream ms = new MemoryStream(image))
					{
						return await channel.SendFileAsync(ms, embed.ToMessageBuilder().Build());
					}
				}
			}
			return await channel.SendMessageAsync("", false, embed);
		}
		public static async Task<IMessage> SendToUser(this Embed embed, IUser user)
		{
			await Task.Yield();
			return await user.SendMessageAsync("", false, embed);
		}

		public static void QueueMessageAsync(this IMessageChannel channel, string message)
			=> MessageBucket.Add(new MessageBucketArgs()
			{
				properties = new MessageProperties()
				{
					Content = message
				},
				channel = channel
			});

		public static MessageBuilder ToMessageBuilder(this Embed embed)
		{
			MessageBuilder b = new MessageBuilder();

			if (embed.Author.HasValue)
			{
				b.AppendText(embed.Author.Value.Name, MessageFormatting.Bold);
			}

			b.AppendText(embed.Title, MessageFormatting.Bold)
			 .AppendText(embed.Description);

			foreach (EmbedField f in embed.Fields)
			{
				b.AppendText(f.Name, MessageFormatting.Underlined)
				 .AppendText(f.Value)
				 .NewLine();
			}

			if (embed.Footer.HasValue)
			{
				b.AppendText(embed.Footer.Value.Text, MessageFormatting.Italic);
			}

			return b;
		}
	}
}
