namespace Geparate.Gneedle
{
    public interface IDynamicProxyContext<ResultType> : IAopContext<IDynamicProxyContext<ResultType>>
    {
        /// <summary>
        /// Excute method and return method's return-value
        /// </summary>
        ResultType Result();
    }
    public interface IDynamicProxyContext : IAopContext<IDynamicProxyContext>
    {
        /// <summary>
        /// Excute method
        /// </summary>
        void Done();
    }
}