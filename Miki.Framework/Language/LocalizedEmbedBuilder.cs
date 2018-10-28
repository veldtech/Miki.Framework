using Miki.Discord;
using Miki.Discord.Common;
using Miki.Discord.Rest;
using Miki.Localization;

namespace Miki.Framework.Language
{
	public class LocalizedEmbedBuilder
	{
		public EmbedBuilder EmbedBuilder { get; private set; } = new EmbedBuilder();

		private readonly LocaleInstance _instance;

		public LocalizedEmbedBuilder(LocaleInstance instance)
		{
			this._instance = instance;
		}

		public LocalizedEmbedBuilder AddField(IResource title, IResource content, bool inline = false)
		{
			EmbedBuilder.AddField(title.Get(_instance), content.Get(_instance), inline);
			return this;
		}

		public DiscordEmbed Build()
			=> EmbedBuilder.ToEmbed();

		public LocalizedEmbedBuilder WithAuthor(IResource title, string iconUrl = null, string url = null)
		{
			EmbedBuilder.SetAuthor(title.Get(_instance), iconUrl, url);
			return this;
		}

		public LocalizedEmbedBuilder WithColor(Color color)
		{
			EmbedBuilder.SetColor(color);
			return this;
		}

		public LocalizedEmbedBuilder WithDescription(string description, params object[] param)
			=> WithDescription(new LanguageResource(description, param));

		public LocalizedEmbedBuilder WithDescription(LanguageResource description)
		{
			EmbedBuilder.SetDescription(description.Get(_instance));
			return this;
		}

		public LocalizedEmbedBuilder WithFooter(string text, string iconUrl = null, params object[] param)
			=> WithFooter(new LanguageResource(text, param), iconUrl);

		public LocalizedEmbedBuilder WithFooter(IResource text, string iconUrl = null)
		{
			EmbedBuilder.SetFooter(text.Get(_instance), iconUrl);
			return this;
		}

		public LocalizedEmbedBuilder WithImageUrl(string imageUrl)
		{
			EmbedBuilder.SetImage(imageUrl);
			return this;
		}

		public LocalizedEmbedBuilder WithThumbnailUrl(string thumbnailUrl)
		{
			EmbedBuilder.SetThumbnail(thumbnailUrl);
			return this;
		}

		public LocalizedEmbedBuilder WithTitle(string resource, params object[] param)
			=> WithTitle(new LanguageResource(resource, param));

		public LocalizedEmbedBuilder WithTitle(IResource title)
		{
			EmbedBuilder.SetTitle(title.Get(_instance));
			return this;
		}
	}
}