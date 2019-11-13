namespace Miki.Framework.Commands.Permissions.Exceptions
{
    using Miki.Localization.Exceptions;
    using Miki.Localization.Models;

    public class PermissionUnauthorizedException : LocalizedException
    {
        public override IResource LocaleResource => new LanguageResource("error_permissions_invalid");
    }
}
