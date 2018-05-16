using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework
{
	// TODO: consider privatizing function due to user friendliness
    public interface IAttachable
    {
		void AttachTo(Bot bot);
    }
}
