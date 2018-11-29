using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Arguments
{
	public class IntArgumentParser : IArgumentParser
	{
		public int Priority => 1;

		public bool CanParse(IArgumentPack pack)
			=> int.TryParse(pack.Peek(), out _);

		public object Parse(IArgumentPack pack)
		{
			return int.Parse(pack.Take());
		}
	}
}
