using System;

namespace Miki.Framework
{
    using System.Threading.Tasks;

    public interface IProvider
    {
        bool IsActive { get; }

        Task StartAsync();

        Task StopAsync();
    }
}
