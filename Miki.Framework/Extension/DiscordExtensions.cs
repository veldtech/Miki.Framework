using Miki.Common.Builders;
using Miki.Discord.Common;
using Miki.Framework;
using System;
using System.Threading.Tasks;

namespace Miki.Discord
{
	public static class DiscordExtensions
	{
		public static IMessageReference QueueToChannel(this DiscordEmbed embed, IDiscordChannel channel)
		{
			return MessageBucket.Add(new MessageBucketArgs()
			{
				properties = new MessageArgs()
				{
					embed = embed
				},
				channel = channel as IDiscordTextChannel
			});
		}

		public static IMessageReference ThenWait(this IMessageReference reference, int milliseconds)
		{
			reference.ProcessAfterComplete(async (msg) =>
			{
				await Task.Delay(milliseconds);
			});
			return reference;
		}

		public static IMessageReference ThenDelete(this IMessageReference reference)
		{
			reference.ProcessAfterComplete(async (msg) =>
			{
				await msg.DeleteAsync();
			});
			return reference;
		}

		public static IMessageReference ThenEdit(this IMessageReference reference, string message = "", DiscordEmbed embed = null)
		{
			reference.ProcessAfterComplete(async (x) => await x.EditAsync(new EditMessageArgs
			{
				content = message,
				embed = embed
			}));
			return reference;
		}

		public static IMessageReference Then(this IMessageReference reference, Func<IDiscordMessage, Task> fn)
		{
			reference.ProcessAfterComplete(fn);
			return reference;
		}

		public static async Task<IDiscordMessage> SendToChannel(this DiscordEmbed embed, IDiscordChannel channel)
		{
			//if (!(await (await (channel as IDiscordGuildChannel).GetGuildAsync()).GetSelfAsync())
			//	.GuildPermissions.HasFlag (GuildPermission.EmbedLinks))
			//{
			//	if (!string.IsNullOrEmpty(embed.Image?.Url ?? ""))
			//	{
			//		return await channel.SendMessageAsync(embed.ToMessageBuilder().Build());
			//	}

			//	using (WebClient wc = new WebClient())
			//	{
			//		byte[] image = wc.DownloadData(embed.Image.Url);
			//		using (MemoryStream ms = new MemoryStream(image))
			//		{
			//			return await channel.SendFileAsync(ms, embed.ToMessageBuilder().Build());
			//		}
			//	}
			//}
			return await (channel as IDiscordTextChannel).SendMessageAsync("", false, embed);
		}

		public static async Task<IDiscordMessage> SendToUser(this DiscordEmbed embed, IDiscordUser user)
		{
			return await (await user.GetDMChannelAsync()).SendMessageAsync("", false, embed);
		}

		public static IMessageReference QueueMessageAsync(this IDiscordTextChannel channel, string message)
			=> MessageBucket.Add(new MessageBucketArgs()
			{
				properties = new MessageArgs()
				{
					content = message
				},
				channel = channel
			});

		public static MessageBuilder ToMessageBuilder(this DiscordEmbed embed)
		{
			MessageBuilder b = new MessageBuilder();

			if (embed.Author != null)
			{
				b.AppendText(embed.Author.Name, MessageFormatting.Bold);
			}

			b.AppendText(embed.Title, MessageFormatting.Bold)
			 .AppendText(embed.Description);

			foreach (EmbedField f in embed.Fields)
			{
				b.AppendText(f.Title, MessageFormatting.Underlined)
				 .AppendText(f.Content)
				 .NewLine();
			}

			if (embed.Footer != null)
			{
				b.AppendText(embed.Footer.Text, MessageFormatting.Italic);
			}

			return b;
		}
	}
}