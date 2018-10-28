using Miki.Localization;
using Miki.Localization.Exceptions;

namespace Miki.Framework.Exceptions
{
	internal class ArgObjectNullException : LocalizedException
	{
		public override IResource LocaleResource
			=> new LanguageResource("error_argument_null", "[docs](https://github.com/Mikibot/Miki/wiki)");
	}
}