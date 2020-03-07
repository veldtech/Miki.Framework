namespace Miki.Framework.Commands
{
    using Miki.Localization.Exceptions;
    using Miki.Localization.Models;

	/// <summary>
	/// Localizable exception for when a command errors. When instructed to it will be automatically
	/// called to the upstream if a context fails to run, with the exception of other localized
	/// exceptions.
	/// </summary>
    public class CommandException : LocalizedException
	{
		/// <inheritdoc/>
		public override IResource LocaleResource
			=> new LanguageResource("error_default_command");

		/// <summary>
		/// Source of the exception.
		/// </summary>
		public readonly Node Command;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CommandException.CommandException(Node)'
		public CommandException(Node e)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CommandException.CommandException(Node)'
		{
			Command = e;
		}
	}
}