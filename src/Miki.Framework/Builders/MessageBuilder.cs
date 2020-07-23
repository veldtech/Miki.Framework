using System.Text;

namespace Miki.Common.Builders
{
    public class MessageBuilder
	{
		private readonly StringBuilder _builder = new StringBuilder();

		public MessageBuilder AppendText(string text, MessageFormatting formatting = MessageFormatting.Plain, bool newLine = true, bool endWithSpace = false)
		{
			if(string.IsNullOrWhiteSpace(text)) return this;

			text = ApplyFormatting(text, formatting);

			if(endWithSpace) text += " ";

			if(newLine)
			{
				_builder.AppendLine(text);
			}
			else
			{
				_builder.Append(text);
			}

			return this;
		}

		public MessageBuilder NewLine()
		{
			_builder.AppendLine("");
			return this;
		}

		public string Build()
		{
			return _builder.ToString();
		}

		public string BuildWithBlockCode(string language = "")
		{
			return "```" + language + "\n" + _builder + "\n```";
		}

		private string ApplyFormatting(string text, MessageFormatting formatting)
        {
            return formatting switch
            {
                MessageFormatting.Bold => ("**" + text + "**"),
                MessageFormatting.BoldItalic => ("**_" + text + "_**"),
                MessageFormatting.BoldItalicUnderlined => ("__**_" + text + "_**__"),
                MessageFormatting.Italic => ("_" + text + "_"),
                MessageFormatting.ItalicUnderlined => ("___" + text + "___"),
                MessageFormatting.Underlined => ("__" + text + "__"),
                MessageFormatting.Code => ("`" + text + "`"),
                MessageFormatting.BlockCode => ("```" + text + "```"),
                MessageFormatting.BoldUnderlined => ("**_" + text + "_**"),
                MessageFormatting.Plain => text,
                _ => text
            };
        }
	}

	/// <summary>
	/// Message format style.
	/// </summary>
	// TODO: Use a bitflag instead?
	public enum MessageFormatting
	{
		Plain,
		Bold,
		Italic,
		Underlined,
		BoldItalic,
		BoldUnderlined,
		ItalicUnderlined,
		BoldItalicUnderlined,
		Code,
		BlockCode
	}
}