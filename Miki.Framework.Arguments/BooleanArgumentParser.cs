using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Arguments
{
	public class BooleanArgumentParser : IArgumentParser
	{
		public int Priority => 1;

		public bool CanParse(IArgumentPack pack)
			=> bool.TryParse(pack.Peek(), out _);

		public object Parse(IArgumentPack pack)
		{
			return bool.Parse(pack.Take());
		}
	}
}
