namespace Miki.Framework
{
	public interface IExecutableCommand : IExecutable
	{
		string GetIdentifier();
	}

	public interface IExecutable : IAsyncExecutor<IContext>
	{
	}
}
