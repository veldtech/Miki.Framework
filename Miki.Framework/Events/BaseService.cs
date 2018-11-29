namespace Miki.Framework.Events
{
	public class BaseService : Event
	{
		public virtual void Install(Module m)
		{
			if (Module == null)
			{
				Module = m;
			}
		}

		public virtual void Uninstall(Module m)
		{
		}
	}
}