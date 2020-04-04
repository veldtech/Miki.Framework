namespace Miki.Framework
{
    /// <summary>
    /// <see cref="IContext"/> extension methods for utility.
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Generic version of <see cref="IContext.GetService(System.Type)"/>
        /// </summary>
        public static T GetService<T>(this IContext c)
        {
            return (T)c.GetService(typeof(T));
        }

        /// <summary>
        /// Generic version of <see cref="IContext.GetContext(string)"/>
        /// </summary>
        public static T GetContext<T>(this IContext context, string key)
        {
            var value = context.GetContext(key);
            if(value is T castValue)
            {
                return castValue;
            }
            return default;
        }

        /// <summary>
        /// Generic version of <see cref="IMutableContext.SetContext(string, object)"/>
        /// </summary>
        public static void SetContext<T>(this IMutableContext context, string key, T value)
        {
            context.SetContext(key, value);
        }
    }
}
