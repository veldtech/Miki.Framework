namespace Miki.Framework.Arguments
{
	public interface ITypedArgumentPack
	{
		IArgumentPack Pack { get; }

		/// <summary>
		/// Check whether you're allowed to take at the moment.
		/// </summary>
		bool CanTake { get; }

		void Skip();

		/// <summary>
		/// Returns the string without consuming the argument.
		/// </summary>
		/// <returns>current first argument</returns>
		bool Peek<T>(out T value);

		/// <summary>
		/// Returns and consumes the argument.
		/// </summary>
		/// <returns>current first argument</returns>
		bool Take<T>(out T value);
	}
}
