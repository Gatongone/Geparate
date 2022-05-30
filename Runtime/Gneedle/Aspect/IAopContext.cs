using System;

namespace Geparate.Gneedle
{
    /// <summary>
    /// IAopContext  describes the behavior of the life cycle invoked in the aspect method,
    /// and it will return the sub class of the interface for chain programming
    /// </summary>
    public interface IAopContext<out T> where T:IAopContext<T>
    {
        /// <summary>
        /// Excute action before aspect
        /// </summary>
        T WithBefore(Action action);
        /// <summary>
        /// Excute action after aspect
        /// </summary>
        T WithAfter(Action action);
        /// <summary>
        /// Excute action when aspect finish
        /// </summary>
        T WithFinish(Action action);
        /// <summary>
        /// Excute action when aspect throw a exception
        /// </summary>
        T WithFaild(Action<Exception> action);    
    }
}