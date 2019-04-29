using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Pipelines
{
    public interface IPipelineStage
    {
        ValueTask<bool> CheckAsync(IDiscordMessage data, IMutableContext e);
    }
}
