using ProtoBuf;

namespace Miki.Framework.Models
{
	[ProtoContract]
	public class Identifier
	{
		[ProtoMember(1)]
		public long GuildId { get; set; }

		[ProtoMember(2)]
		public string DefaultValue { get; set; }

		[ProtoMember(3)]
		public string Value { get; set; }
	}
}