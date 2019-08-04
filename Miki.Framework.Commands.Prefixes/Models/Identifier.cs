using System.Runtime.Serialization;

namespace Miki.Framework.Commands.Prefixes.Models
{
	[DataContract]
	public class Identifier
	{
		[DataMember(Name = "guild_id", Order = 1)]
		public long GuildId { get; set; }

		[DataMember(Name = "default", Order = 2)]
		public string DefaultValue { get; set; }

		[DataMember(Name = "current", Order = 3)]
		public string Value { get; set; }
	}
}