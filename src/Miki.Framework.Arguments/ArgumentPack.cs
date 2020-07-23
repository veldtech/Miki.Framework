using System;
using System.Collections.Generic;
using System.Linq;
using Miki.Functional;

namespace Miki.Framework.Arguments
{
    /// <inheritdoc/>
    public class ArgumentPack : IArgumentPack
	{
		/// <inheritdoc/>
		public bool CanTake => Cursor < arguments.Count && Cursor >= 0;

        /// <inheritdoc/>
        public int Cursor { get; private set; }

        /// <inheritdoc/>
        public int Length => arguments.Count;

        private readonly IReadOnlyList<string> arguments;

        /// <summary>
        /// Creates a new argument pack from an array.
        /// </summary>
        public ArgumentPack(params string[] arguments)
		    : this(arguments.AsEnumerable())
        {
        }
		/// <summary>
		/// Creates a new argument pack from an enumerable.
		/// </summary>
		public ArgumentPack(IEnumerable<string> arguments)
		{
			this.arguments = arguments.ToList();
		}

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public string Take()
		{
			var g = Peek();
			Cursor++;
			return g;
		}

        /// <inheritdoc/>
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

        private Optional<string> Get(int index)
        {
            if(!CanTake)
            {
                throw new IndexOutOfRangeException();
            }
            return arguments[index];
        }
    }
}
