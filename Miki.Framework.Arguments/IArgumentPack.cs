using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Arguments
{
	public interface IArgumentPack
	{
		/// <summary>
		/// Check whether you're allowed to take at the moment.
		/// </summary>
		bool CanTake { get; }

        int Cursor { get; }

        int Length { get; }

		/// <summary>
		/// Returns the string without consuming the argument.
		/// </summary>
		/// <returns>current first argument</returns>
		string Peek();

        void SetCursor(int value);

		/// <summary>
		/// Returns and consumes the argument.
		/// </summary>
		/// <returns>current first argument</returns>
		string Take();
	}
}
