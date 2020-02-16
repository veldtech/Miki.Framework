namespace Miki.Framework.Arguments.Parsers
{
    using System;

    public class GuidArgumentParser : IArgumentParser
	{
		public Type OutputType => typeof(Guid);

		public int Priority => 0;

		public bool CanParse(IArgumentPack pack, Type targetType)
		{
            if(targetType != typeof(Guid))
            {
				return false;
            }
			return Guid.TryParse(pack.Peek(), out _);
		}

		public object Parse(IArgumentPack pack, Type targetType)
		{
			return Guid.Parse(pack.Take());
		}
	}
}
