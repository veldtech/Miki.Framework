using Miki.Localization;
using Miki.Localization.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
    class ArgObjectNullException : LocalizedException
    {
		public override IResource LocaleResource 
			=> new LanguageResource("error_argument_null", "[docs](https://github.com/Mikibot/Miki/wiki)");
	}
}
