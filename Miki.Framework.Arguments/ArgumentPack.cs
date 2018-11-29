using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Arguments
{
	public class ArgumentPack : IArgumentPack
	{
		IReadOnlyList<string> _arguments;
		int _index = 0;

		public bool CanTake => _index < _arguments.Count && _index >= 0;

		public ArgumentPack(IReadOnlyList<string> arguments)
		{
			_arguments = arguments;
		}

		public string Peek()
		{
			return Get(_index);
		}

		public string Take()
		{
			var g = Peek();
			_index++;
			return g;
		}

		private string Get(int index)
		{
			if (!CanTake)
			{
				throw new IndexOutOfRangeException();
			}
			return _arguments[index];
		}
	}
}
