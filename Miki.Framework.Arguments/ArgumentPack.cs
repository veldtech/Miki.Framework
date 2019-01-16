using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Arguments
{
	public class ArgumentPack : IArgumentPack
	{
		private readonly IReadOnlyList<string> _arguments;
		int _index = 0;

		public bool CanTake => _index < _arguments.Count && _index >= 0;

        public int Cursor => _index;

        public int Length => _arguments.Count;

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

        public void SetCursor(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();

            if (value >= Length)
                throw new ArgumentOutOfRangeException();

            _index = value;
        }
    }
}
