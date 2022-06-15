namespace Geparate.Gneedle
{
    /// A DynamicProxy object is generated in memory during running.
    /// This AOP object contains all the methods of the target object,
    /// and the method of enhancement at a specific cut point and call back to the original object.
    /// </summary>
    public interface IDynamicProxy
    {
        FieldContext<T> GetField<T>(string fieldName);
        PropertyContext<T> GetProperty<T>(string propertyName);
        void SetField(string fieldName, object value);
        void SetProperty(string propertyName, object value);
        bool TryGetField<T>(string fieldName, out FieldContext<T> fieldContext);
        bool TryGetProperty<T>(string fieldName, out PropertyContext<T> propertyContext);
        IDynamicProxyContext GetMethod(string methodName, params object[] parameters);
    }
}