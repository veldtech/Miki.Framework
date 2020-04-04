namespace Miki.Framework.Commands
{
    using Miki.Localization;
    using Miki.Localization.Exceptions;

    /// <summary>
    /// Localizable exception for when a command errors. When instructed to it will be automatically
    /// called to the upstream if a context fails to run, with the exception of other localized
    /// exceptions.
    /// </summary>
    public class CommandException : LocalizedException
    {
        /// <inheritdoc/>
        public override IResource LocaleResource => new LanguageResource("error_default_command");

        /// <summary>
        /// Source of the exception.
        /// </summary>
        public readonly Node Command;

        public CommandException(Node e)
        {
            Command = e;
        }
    }
}