namespace Miki.Framework.Arguments
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public class ArgumentPack : IArgumentPack
	{
		private readonly IReadOnlyList<string> arguments;

		public bool CanTake => Cursor < arguments.Count && Cursor >= 0;

		public int Cursor { get; private set; }

		public int Length => arguments.Count;

        public ArgumentPack(params string[] arguments)
		    : this(arguments.AsEnumerable())
        {
        }
		public ArgumentPack(IEnumerable<string> arguments)
		{
			this.arguments = arguments.ToList();
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
			if(!CanTake)
			{
				throw new IndexOutOfRangeException();
			}
			return arguments[index];
		}

		public void SetCursor(int value)
		{
            if(value < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if(value > Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            Cursor = value;
		}
	}
}
