using Miki.Discord;

namespace Miki.Framework.Events
{
	public class EventSystemConfig
	{
		public ulong[] Developers;

		/// <summary>
		/// Note: EmbedBuilder.Description will be replaced by the exception text.
		/// </summary>
		public EmbedBuilder ErrorEmbedBuilder;
	}
}