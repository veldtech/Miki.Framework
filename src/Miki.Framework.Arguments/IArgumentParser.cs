using System;

namespace Miki.Framework.Arguments
{
    public interface IArgumentParser
	{
		Type OutputType { get; }

		int Priority { get; }

		bool CanParse(IArgumentPack pack, Type targetType);

		object Parse(IArgumentPack pack, Type targetType);
	}
}
