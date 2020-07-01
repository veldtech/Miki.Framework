namespace Miki.Framework
{
	/// <summary>
	/// <see cref="IExecutable"/> with an unique identifier.
	/// </summary>
	public interface IExecutableCommand : IExecutable
	{
		/// <summary>
		/// Gets the unique identifier attached to this Executable.
		/// </summary>
		string GetIdentifier();
	}

	/// <summary>
	/// Executes something with an <see cref="IContext"/> attached.
	/// </summary>
	public interface IExecutable : IAsyncExecutor<IContext>
	{
	}
}
