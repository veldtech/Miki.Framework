namespace Miki.Framework.Arguments
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Miki.Functional;

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

		public Optional<string> Peek()
		{
            try
            {
                return Get(Cursor);
            }
            catch
            {
                return Optional<string>.None;
            }
        }

		public string Take()
		{
			var g = Peek();
			Cursor++;
			return g;
		}

		private Optional<string> Get(int index)
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
