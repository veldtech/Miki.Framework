using System.Runtime.Serialization;

namespace Miki.Framework.Commands.States
{
    [DataContract]
    public class CommandState
	{
		[DataMember(Order = 1)]
		public string Name { get; set; }

        [DataMember(Order = 2)]
        public long ChannelId { get; set; }

        [DataMember(Order = 3)]
        public bool State { get; set; }
	}
}