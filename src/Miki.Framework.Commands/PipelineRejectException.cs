namespace Miki.Framework.Commands
{
    using System;

    /// <summary>
    /// Internal Command pipeline exception when a pipeline stage stops the pipeline.
    /// </summary>
    public class PipelineRejectException : Exception
    {
        public PipelineRejectException(string message)
            : base(message)
        {
        }

        public PipelineRejectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
