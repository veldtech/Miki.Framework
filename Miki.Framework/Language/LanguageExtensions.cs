using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Language
{
    public static class LanguageExtensions
    {
		public static LanguageResource CreateResource(this EventContext context, string resource, params object[] parameters)
			=> new LanguageResource(resource, parameters);

		public static LocalizedEmbedBuilder CreateEmbedBuilder(this EventContext context)
			=> new LocalizedEmbedBuilder(context.Channel.Id);
    }
}
