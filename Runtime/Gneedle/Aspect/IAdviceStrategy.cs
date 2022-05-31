using System.Reflection;

namespace Geparate.Gneedle.Aspect
{
    /// <summary>
    /// IAdviceStrategy is a custom adviser .It provides a custom aspect advice method to weaves into target.
    /// </summary>
    public interface IAdviceStrategy
    {
        /// <returns> Method's return value</returns>
        object Advice(object target, MethodInfo method, params object[] parameters);
    }
}