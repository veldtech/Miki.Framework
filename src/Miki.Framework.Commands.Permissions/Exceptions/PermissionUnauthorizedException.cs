namespace Miki.Framework.Commands.Permissions.Exceptions
{
    using System;
    using Miki.Localization;
    using Miki.Localization.Exceptions;

    public class PermissionUnauthorizedException : LocalizedException
    {
        public override IResource LocaleResource => new LanguageResource("error_permissions_invalid");

        public PermissionUnauthorizedException() { }
        public PermissionUnauthorizedException(string message) : base(message) { }
        public PermissionUnauthorizedException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
