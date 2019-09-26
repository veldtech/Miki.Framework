using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Localization
{
    public interface ILocalizationService
    {
        ValueTask<string> GetKeyAsync(string key, params string[] format);

        ValueTask SetLocaleForChannel(ulong snowflake, string language);
    }
}
