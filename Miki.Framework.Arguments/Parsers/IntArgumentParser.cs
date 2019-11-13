namespace Miki.Framework.Arguments.Parsers
{
    using System;

    public class IntArgumentParser : IArgumentParser
	{
		public Type OutputType
			=> typeof(int);

		public int Priority => 1;

		public bool CanParse(IArgumentPack pack)
			=> int.TryParse(pack.Peek(), out _);

		public object Parse(IArgumentPack pack)
		{
			var value = pack.Take();

			if(int.TryParse(value, out int result))
			{
				return result;
			}
			return null;
		}
	}
}
