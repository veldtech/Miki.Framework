using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Arguments
{
	public interface IArgumentParser
	{
		int Priority { get; }

		bool CanParse(IArgumentPack pack);

		object Parse(IArgumentPack pack);
	}
}
