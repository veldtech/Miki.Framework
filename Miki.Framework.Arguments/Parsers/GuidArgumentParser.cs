namespace Miki.Framework.Arguments.Parsers
{
    using System;

    public class GuidArgumentParser : IArgumentParser
	{
		public Type OutputType => typeof(Guid);

		public int Priority => 0;

		public bool CanParse(IArgumentPack pack)
		{
			return Guid.TryParse(pack.Peek(), out _);
		}

		public object Parse(IArgumentPack pack)
		{
			return Guid.Parse(pack.Take());
		}
	}
}
