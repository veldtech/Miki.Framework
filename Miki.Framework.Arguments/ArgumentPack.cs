using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miki.Framework.Arguments
{
	public class ArgumentPack : IArgumentPack
	{
		private readonly IReadOnlyList<string> _arguments;

        public bool CanTake => Cursor < _arguments.Count && Cursor >= 0;

        public int Cursor { get; private set; } = 0;

        public int Length => _arguments.Count;

        public ArgumentPack(IEnumerable<string> arguments)
		{
			_arguments = arguments.ToList();
		}

		public string Peek()
		{
			return Get(Cursor);
		}

		public string Take()
		{
			var g = Peek();
			Cursor++;
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

            if (value > Length)
                throw new ArgumentOutOfRangeException();

            Cursor = value;
        }
    }
}
