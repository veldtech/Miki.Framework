namespace Miki.Framework.Arguments.Parsers
{
    using System;

    public class UlongArgumentParser : IArgumentParser
	{
		public Type OutputType
			=> typeof(ulong);

		public int Priority => 1;

		public bool CanParse(IArgumentPack pack)
			=> ulong.TryParse(pack.Peek(), out _);

		public object Parse(IArgumentPack pack)
		{
			var value = pack.Take();

			if(ulong.TryParse(value, out ulong result))
			{
				return result;
			}
			return null;
		}
	}
}
