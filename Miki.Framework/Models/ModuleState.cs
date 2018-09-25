using ProtoBuf;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miki.Framework.Models
{
	[ProtoContract]
	public class ModuleState
	{
		[ProtoMember(1)]
		public string ModuleName { get; set; }

		[ProtoMember(2)]
		public long ChannelId { get; set; }

		[ProtoMember(3)]
		public bool State { get; set; }
	}
}