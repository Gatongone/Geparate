using System.Collections.Generic;
using System.Reflection;

namespace Geparate.Gneedle.Aspect
{
    /// <summary>
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

    public class DynamicProxy : IDynamicProxy
    {
        private readonly object m_Target;
        private readonly Dictionary<string, MethodInfo> m_BehavioursMap = new Dictionary<string, MethodInfo>();
        private readonly Dictionary<string, FieldInfo> m_FieldsMap = new Dictionary<string, FieldInfo>();
        private readonly Dictionary<string, PropertyInfo> m_PropertiesMap = new Dictionary<string, PropertyInfo>();

        public DynamicProxy(object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
        {
            m_Target = obj;
            foreach (var methodInfo in obj.GetType().GetMethods(bindingFlags))
            {
                m_BehavioursMap.Add(methodInfo.Name, methodInfo);
            }

            foreach (var propertyInfo in obj.GetType().GetProperties(bindingFlags))
            {
                m_PropertiesMap.Add(propertyInfo.Name, propertyInfo);
            }

            foreach (var fieldInfo in obj.GetType().GetFields(bindingFlags))
            {
                m_FieldsMap.Add(fieldInfo.Name, fieldInfo);
            }
        }

        public FieldContext<T> GetField<T>(string fieldName)
        {
            return new FieldContext<T>(m_FieldsMap[fieldName], m_Target);
        }

        public PropertyContext<T> GetProperty<T>(string propertyName)
        {
            return new PropertyContext<T>(m_PropertiesMap[propertyName], m_Target);
        }

        public bool TryGetField<T>(string fieldName, out FieldContext<T> fieldContext)
        {
            fieldContext = default;
            if (!m_FieldsMap.TryGetValue(fieldName, out var fieldInfo)) 
                return false;
            fieldContext = new FieldContext<T>(fieldInfo, m_Target);
            return true;

        }

        public bool TryGetProperty<T>(string fieldName, out PropertyContext<T> propertyContext)
        {
            propertyContext = default;
            if (!m_PropertiesMap.TryGetValue(fieldName, out var propertyInfo)) 
                return false;
            propertyContext = new PropertyContext<T>(propertyInfo, m_Target);
            return true;

        }

        public void SetField(string fieldName, object value)
        {
            m_FieldsMap[fieldName].SetValue(m_Target, value);
        }

        public void SetProperty(string propertyName, object value)
        {
            m_PropertiesMap[propertyName].SetValue(m_Target, value);
        }

        public IDynamicProxyContext GetMethod(string methodName, params object[] parameters)
        {
            var isContain = m_BehavioursMap.TryGetValue(methodName, out var method);
            return new DynamicProxyContext(methodName, isContain, m_Target, method, parameters);
        }
    }
}