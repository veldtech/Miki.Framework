namespace Miki.Framework.Events
{
    public class BaseService : Event
    {
        public virtual void Install(Module m, Bot b)
        {
            if (Module == null)
            {
                Module = m;
            }
		}

        public virtual void Uninstall(Module m, Bot b)
        {
        }
    }
}