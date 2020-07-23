using Miki.Functional;

namespace Miki.Framework.Arguments
{
    /// <summary>
    /// A validated queue pattern for split arguments.
    /// </summary>
    public interface IArgumentPack
	{
        /// <summary>
        /// Indicates whether the <see cref="Cursor"/> position has exceeded the <see cref="Length"/>
        /// or is in a valid index.
        /// </summary>
		bool CanTake { get; }

        /// <summary>
        /// Current cursor index.
        /// </summary>
		int Cursor { get; }

        /// <summary>
        /// Total amount of arguments parsed.
        /// </summary>
		int Length { get; }

        /// <summary>
        /// Peeks the current in the <see cref="Cursor"/> first value without consuming the value.
        /// </summary>
		/// <returns>current first argument</returns>
		Optional<string> Peek();

        /// <summary>
        /// Sets the cursor index.
        /// </summary>
        void SetCursor(int value);

        /// <summary>
        /// Returns the value and increments the <see cref="Cursor"/>.
        /// </summary>
        /// <returns>current first argument</returns>
        string Take();
	}
}
