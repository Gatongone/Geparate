using JetBrains.Annotations;

namespace Geparate.Gneedle.Aspect
{
    public interface IDynamicProxyContext : IAspectAdvice<IDynamicProxyContext>
    {
        /// <summary>
        /// Execute method and begin advice transactions
        /// </summary>
        [CanBeNull] object Invoke();
    }
}