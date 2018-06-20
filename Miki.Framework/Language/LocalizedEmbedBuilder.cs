using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Language
{
    public class LocalizedEmbedBuilder
	{
		public EmbedBuilder embedBuilder { get; private set; } = new EmbedBuilder();
		private ulong channelId;

		public LocalizedEmbedBuilder(ulong channelId)
		{
			this.channelId = channelId;
		}

		public LocalizedEmbedBuilder AddField(IResource title, IResource content, bool inline = false)
		{
			embedBuilder.AddField(title.Get(channelId), content.Get(channelId), inline);
			return this;
		}

		public Embed Build()
			=> embedBuilder.Build();

		public LocalizedEmbedBuilder WithAuthor(IResource title, string iconUrl = null, string url = null)
		{
			embedBuilder.Author = new EmbedAuthorBuilder()
			{
				IconUrl = iconUrl,
				Url = url,
				Name = title.Get(channelId)
			};
			return this;
		}

		public LocalizedEmbedBuilder WithColor(Color color)
		{
			embedBuilder.Color = color;
			return this;
		}

		public LocalizedEmbedBuilder WithDescription(string description, params object[] param)
			=> WithDescription(new LanguageResource(description, param));
		public LocalizedEmbedBuilder WithDescription(LanguageResource description)
		{
			embedBuilder.WithDescription(description.Get(channelId));
			return this;
		}

		public LocalizedEmbedBuilder WithFooter(string text, string iconUrl = null, params object[] param)
			=> WithFooter(new LanguageResource(text, param), iconUrl);
		public LocalizedEmbedBuilder WithFooter(IResource text, string iconUrl = null)
		{
			embedBuilder.Footer = new EmbedFooterBuilder()
			{
				IconUrl = iconUrl,
				Text = text.Get(channelId)
			};
			return this;
		}

		public LocalizedEmbedBuilder WithImageUrl(string imageUrl)
		{
			embedBuilder.ImageUrl = imageUrl;
			return this;
		}

		public LocalizedEmbedBuilder WithThumbnailUrl(string thumbnailUrl)
		{
			embedBuilder.ThumbnailUrl = thumbnailUrl;
			return this;
		}

		public LocalizedEmbedBuilder WithTitle(string resource, params object[] param)
			=> WithTitle(new LanguageResource(resource, param));
		public LocalizedEmbedBuilder WithTitle(IResource title)
		{
			embedBuilder.WithTitle(title.Get(channelId));
			return this;
		}
    }
}
