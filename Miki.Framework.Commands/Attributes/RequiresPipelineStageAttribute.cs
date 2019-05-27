using Miki.Framework.Commands.Pipelines;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Attributes
{
    public class RequiresPipelineStageAttribute : CommandRequirementAttribute
    {
        private readonly Type _t;

        public RequiresPipelineStageAttribute(Type t)
        {
            _t = t;
        }

        public override Task<bool> CheckAsync(IContext e)
        {
            return Task.FromResult(e.GetStage(_t) != null);
        }

        public override Task OnCheckFail(IContext e)
        {
            Log.Error($"Pipeline stage of type '{_t.Name}' is not initialized.");
            return Task.CompletedTask;
        }
    }
}
