using Miki.Framework.Languages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Language
{
	public interface IResource
	{
		string Get(ulong channelId);
	}

	public class StringResource : IResource
	{
		public string Value { get; internal set; }

		public StringResource(string value)
		{
			Value = value;
		}

		public string Get(ulong channelId)
			=> Value;
	}

    public class LanguageResource : IResource
    {
		public string Resource { get; internal set; }
		public object[] Parameters { get; internal set; }

		public LanguageResource(string resource, params object[] param)
		{
			Resource = resource;
			Parameters = param;
		}

		public string Get(ulong channelId)
			=> Locale.GetStringAsync(channelId, Resource, Parameters).Result;
    }
}
