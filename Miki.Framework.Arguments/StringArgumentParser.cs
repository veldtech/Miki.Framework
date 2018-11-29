using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Arguments
{
	public class StringArgumentParser : IArgumentParser
	{
		public int Priority
			=> 0;

		public bool CanParse(IArgumentPack pack)
			=> true;

		public object Parse(IArgumentPack pack)
		{
			if (pack.Peek().StartsWith("\""))
			{
				List<string> allItems = new List<string>();
				do
				{
					allItems.Add(pack.Take());
				} while (!allItems[allItems.Count - 1].EndsWith("\""));
				return string.Join(" ", allItems).TrimStart('"').TrimEnd('"');
			}
			else
			{
				return pack.Take();
			}
		}
	}

}
