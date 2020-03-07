namespace Miki.Framework.Commands
{
    using System;

    /// <summary>
    /// Internal Command pipeline exception when a pipeline stage stops the pipeline.
    /// </summary>
    public class PipelineRejectException : Exception
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PipelineRejectException.PipelineRejectException(string)'
        public PipelineRejectException(string message)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PipelineRejectException.PipelineRejectException(string)'
            : base(message)
        {
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PipelineRejectException.PipelineRejectException(string, Exception)'
        public PipelineRejectException(string message, Exception innerException)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PipelineRejectException.PipelineRejectException(string, Exception)'
            : base(message, innerException)
        {
        }
    }
}
