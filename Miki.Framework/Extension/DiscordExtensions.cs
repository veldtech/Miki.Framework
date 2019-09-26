using Microsoft.Extensions.DependencyInjection;
using Miki.Common.Builders;
using Miki.Discord.Common;
using Miki.Framework;
namespace Miki.Discord
{
    using Miki.Framework.Exceptions;
    using Miki.Net.Http;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public static class DiscordExtensions
	{
		public static async Task<IMessageReference> QueueAsync(this DiscordEmbed embed, IDiscordTextChannel channel, string content = "")
		{
			if(channel is IDiscordGuildChannel guildChannel)
			{
				var currentUser = await MikiApp.Instance
					.Services.GetService<IDiscordClient>()
					.GetSelfAsync();
				var currentGuildUser = await guildChannel.GetUserAsync(currentUser.Id);
				var permissions = await guildChannel.GetPermissionsAsync(currentGuildUser);
                if(permissions.HasFlag(GuildPermission.EmbedLinks))
                {
                    return QueueMessage(channel, embed, content);
                }

                if(!string.IsNullOrEmpty(embed.Image?.Url ?? ""))
                {
                    using(HttpClient wc = new HttpClient(embed.Image.Url))
                    {
                        using(Stream ms = await wc.GetStreamAsync())
                        {
                            return channel.QueueMessage(stream: ms, message: embed.ToMessageBuilder().Build());
                        }
                    }
                }

                if(!string.IsNullOrEmpty(embed.Thumbnail?.Url ?? ""))
                {
                    using(HttpClient wc = new HttpClient(embed.Thumbnail.Url))
                    {
                        using(Stream ms = await wc.GetStreamAsync())
                        {
                            return channel.QueueMessage(stream: ms, message: embed.ToMessageBuilder().Build());
                        }
                    }
                }
                return channel.QueueMessage(embed.ToMessageBuilder().Build());
            }
			return QueueMessage(channel, embed, content);
		}

        public static Task<IMessageReference> ThenWaitAsync(
                this Task<IMessageReference> reference,
                int milliseconds) 
            => reference.ContinueWith(x => x.Result.ThenWait(milliseconds));
		public static IMessageReference ThenWait(this IMessageReference reference, int milliseconds)
		{
			reference.ProcessAfterComplete(async (msg) =>
			{
				await Task.Delay(milliseconds);
			});
			return reference;
		}

		public static Task<IMessageReference> ThenDeleteAsync(this Task<IMessageReference> reference)
			=> reference.ContinueWith(x => x.Result.ThenDelete());
		public static IMessageReference ThenDelete(this IMessageReference reference)
		{
			reference.ProcessAfterComplete(async (msg) =>
			{
				await msg.DeleteAsync();
			});
			return reference;
		}

		public static Task<IMessageReference> ThenEditAsync(this Task<IMessageReference> reference, string message = "", DiscordEmbed embed = null)
			=> reference.ContinueWith(x => x.Result.ThenEdit(message, embed));
		public static IMessageReference ThenEdit(this IMessageReference reference, string message = "", DiscordEmbed embed = null)
		{
			reference.ProcessAfterComplete(async (x)
				=> await x.EditAsync(new EditMessageArgs(message, embed)));
			return reference;
		}

		public static Task<IMessageReference> ThenAsync(this Task<IMessageReference> reference, Func<IDiscordMessage, Task> fn)
			=> reference.ContinueWith(x => x.Result.Then(fn));
		public static IMessageReference Then(this IMessageReference reference, Func<IDiscordMessage, Task> fn)
		{
			reference.ProcessAfterComplete(fn);
			return reference;
		}

		public static async Task<IDiscordMessage> EditAsync(this DiscordEmbed embed, IDiscordMessage msg)
		{
			var channel = await msg.GetChannelAsync();
			if(channel is IDiscordGuildChannel guildChannel)
			{
				var currentUser = await MikiApp.Instance
                    .Services.GetService<IDiscordClient>()
					.GetSelfAsync();
				var currentGuildUser = await guildChannel.GetUserAsync(currentUser.Id);
				var permissions = await guildChannel.GetPermissionsAsync(currentGuildUser);

				if(!permissions.HasFlag(GuildPermission.EmbedLinks))
				{
					return await msg.EditAsync(new EditMessageArgs(embed.ToMessageBuilder().Build()));
				}
			}
			return await msg.EditAsync(new EditMessageArgs("", embed));
		}

		public static async Task<IDiscordMessage> SendToChannel(this DiscordEmbed embed, IDiscordTextChannel channel)
		{
			if(channel is IDiscordGuildChannel guildChannel)
			{
				var currentUser = await MikiApp.Instance
					.Services.GetService<DiscordClient>()
					.GetSelfAsync();
				var currentGuildUser = await guildChannel.GetUserAsync(currentUser.Id);
				var permissions = await guildChannel.GetPermissionsAsync(currentGuildUser);

				if(!permissions.HasFlag(GuildPermission.EmbedLinks))
				{
					if(!string.IsNullOrEmpty(embed.Image?.Url ?? ""))
					{
						using(WebClient wc = new WebClient())
						{
							byte[] image = wc.DownloadData(embed.Image.Url);
							using(MemoryStream ms = new MemoryStream(image))
							{
								return await channel.SendFileAsync(ms, "output.png", embed.ToMessageBuilder().Build());
							}
						}
					}
					else if(!string.IsNullOrEmpty(embed.Thumbnail?.Url ?? ""))
					{
						using(WebClient wc = new WebClient())
						{
							byte[] image = wc.DownloadData(embed.Thumbnail.Url);
							using(MemoryStream ms = new MemoryStream(image))
							{
								return await channel.SendFileAsync(ms, "output.png", embed.ToMessageBuilder().Build());
							}
						}
					}
					else
					{
						return await channel.SendMessageAsync(embed.ToMessageBuilder().Build());
					}
				}
			}
			return await channel.SendMessageAsync("", embed: embed);
		}

		public static Task<IDiscordMessage> SendToUser(this DiscordEmbed embed, IDiscordUser user)
		{
			return user.GetDMChannelAsync()
				.ContinueWith(x => x.Result.SendMessageAsync("", false, embed))
                .Unwrap();
		}

		public static IMessageReference QueueMessage(this IDiscordTextChannel channel, string message)
			=> QueueMessage(channel, null, message: message);

		public static IMessageReference QueueMessage(this IDiscordTextChannel channel, DiscordEmbed embed = null, string message = "", Stream stream = null)
			=> MessageBucket.Add(new MessageBucketArgs()
			{
				attachment = stream,
				channel = channel,
				properties = new MessageArgs(message, embed)
			});

		public static MessageBuilder ToMessageBuilder(this DiscordEmbed embed)
		{
			MessageBuilder b = new MessageBuilder();

			if(embed.Author != null)
			{
				b.AppendText(embed.Author.Name, MessageFormatting.Bold);
			}

			b.AppendText(embed.Title, MessageFormatting.Bold)
			 .AppendText(embed.Description);

			if(embed.Fields != null)
			{
				foreach(EmbedField f in embed.Fields)
				{
					b.AppendText(f.Title, MessageFormatting.Underlined)
					 .AppendText(f.Content)
					 .NewLine();
				}
			}

			if(embed.Footer != null)
			{
				b.AppendText(embed.Footer.Text, MessageFormatting.Italic);
			}

			return b;
		}

		public static async Task<IDiscordGuildUser> GetUserAsync(string text, IDiscordGuild guild)
		{
			if(string.IsNullOrWhiteSpace(text))
			{
                throw new ArgObjectNullException();
            }

			IDiscordGuildUser guildUser = null;
			if(Regex.IsMatch(text, "<@(!?)\\d+>"))
			{
				guildUser = await guild.GetMemberAsync(ulong.Parse(text
					.TrimStart('<')
					.TrimStart('@')
					.TrimStart('!')
					.TrimEnd('>')));
			}
			else if(ulong.TryParse(text, out ulong id))
			{
				guildUser = await guild.GetMemberAsync(id);
			}
			else
			{
				var allUsers = await guild.GetMembersAsync();

				guildUser = allUsers.Where(x => x != null)
					.Where(x =>
					{
						if(x.Nickname != null)
						{
							if(x.Nickname.ToLowerInvariant() == text.ToLowerInvariant())
							{
								return true;
							}
						}
						else if(x.Username != null)
						{
							if(x.Username.ToLowerInvariant() == text.ToLowerInvariant())
							{
								return true;
							}
							else if(text == (x.Username + "#" + x.Discriminator).ToLowerInvariant())
							{
								return true;
							}
						}
						return false;
					})
					.FirstOrDefault();
			}

			if(guildUser == null)
			{
				throw new ArgObjectNullException();
			}

			if(guildUser.Id == 0)
			{
				throw new ArgObjectNullException();
			}

			return guildUser;
		}
	}
}