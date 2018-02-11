using Miki.Common;
using Miki.Common.Interfaces;

namespace Miki.Framework.Events
{
    public class BaseService : Event, IService
    {
        public virtual void Install(IModule m)
        {
            if (Module == null)
            {
                Module = m;
            }

            m.Services.Add(this);
        }

        public virtual void Uninstall(IModule m)
        {
            m.Services.Remove(this);
        }
    }
}