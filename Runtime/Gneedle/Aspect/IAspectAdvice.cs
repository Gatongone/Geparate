using System;

namespace Geparate.Gneedle.Aspect
{
    /// <summary>
    /// IAspectAdvice describes the advices on the join point method,
    /// and it will return the sub class of the interface for chain programming
    /// </summary>
    public interface IAspectAdvice<out T> where T:IAspectAdvice<T>
    {
        /// <summary>
        /// Execute action before aspect
        /// </summary>
        T WithBefore(Action action);
        /// <summary>
        /// Execute action after aspect
        /// </summary>
        T WithAfter(Action action);
        /// <summary>
        /// Execute action when aspect finish
        /// </summary>
        T WithFinish(Action action);
        /// <summary>
        /// Execute action when aspect throw a exception
        /// </summary>
        T WithFailed(Action<Exception> action);
        
        /// <summary>
        /// Execute action with a custom advice strategy
        /// </summary>
        T WithAdviceStrategy(IAdviceStrategy adviceStrategy);
    }
}