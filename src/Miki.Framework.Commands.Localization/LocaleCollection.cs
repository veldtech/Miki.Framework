namespace Miki.Framework.Commands.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Miki.Localization.Models;

    public class LocaleCollection
    {
        private readonly HashSet<Locale> locales;

        public LocaleCollection()
        {
            locales = new HashSet<Locale>();
        }

        public void Add(Locale locale)
        {
            if(locale == null)
            {
                throw new ArgumentNullException(nameof(locale));
            }
            locales.Add(locale);
        }

        public Locale Get(string countryCode)
        {
            return locales.FirstOrDefault(x => x.CountryCode == countryCode);
        }

        public bool TryGet(string countryCode, out Locale locale)
        {
            locale = Get(countryCode);
            return locale != null;
        }

        public ISet<Locale> List()
        {
            return locales;
        }
    }
}
