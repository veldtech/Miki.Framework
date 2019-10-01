using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Commands.Localization.Models.Exceptions
{
    public class LocaleNotFoundException : Exception
    {
        string Iso3 { get; }

        public LocaleNotFoundException(string iso3)
            : base($"Locale with reference '{iso3}' was not found.")
        {
            Iso3 = iso3;
        }
    }
}
