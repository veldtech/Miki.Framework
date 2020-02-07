namespace Miki.Framework
{
	/// <inheritdoc/>
	public interface IMutableContext : IContext
	{
		void SetExecutable(IExecutable exec);
		void SetContext<T>(string id, T value);
	}
}
