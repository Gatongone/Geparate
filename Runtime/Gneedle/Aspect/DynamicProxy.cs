using System;
using System.Collections.Generic;
using System.Reflection;

namespace Geparate.Gneedle
{
    public struct FieldContext<T>
    {
        private FieldInfo m_FieldInfo;
        private object m_Target;
        public FieldContext(FieldInfo fieldInfo, object target)
        {
            m_FieldInfo = fieldInfo;
            m_Target = target;
        }
        public T GetValue()
        {
            return (T)m_FieldInfo.GetValue(m_Target);
        }
        public void SetValue(object value)
        {
            m_FieldInfo.SetValue(m_Target, value);
        }
    }
    public struct PropertyContext<T>
    {
        private PropertyInfo m_PropertyInfo;
        private object m_Target;
        public PropertyContext(PropertyInfo propertyInfo, object target)
        {
            m_PropertyInfo = propertyInfo;
            m_Target = target;
        }
        public T GetValue()
        {
            return (T)m_PropertyInfo.GetValue(m_Target);
        }
        public void SetValue(object value)
        {
            m_PropertyInfo.SetValue(m_Target, value);
        }
    }
    public interface IDynamicProxy
    {
        FieldContext<T> GetField<T>(string fieldName);
        PropertyContext<T> GetProperty<T>(string propertyName);
        void SetField(string fieldName, object value);
        void SetProperty(string propertyName, object value);
        bool TryGetField<T>(string fieldName, out FieldContext<T> fieldContext);
        bool TryGetProperty<T>(string fieldName, out PropertyContext<T> propertyContext);
        IDynamicProxyContext Excute(string methodName, params object[] parameters);
        IDynamicProxyContext<ResultType> Excute<ResultType>(string methodName, params object[] parameters);
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
            if (m_FieldsMap.TryGetValue(fieldName, out var fieldInfo))
            {
                fieldContext = new FieldContext<T>(fieldInfo, m_Target);
                return true;
            }
            return false;
        }
        public bool TryGetProperty<T>(string fieldName, out PropertyContext<T> propertyContext)
        {
            propertyContext = default;
            if (m_PropertiesMap.TryGetValue(fieldName, out var propertyInfo))
            {
                propertyContext = new PropertyContext<T>(propertyInfo, m_Target);
                return true;
            }
            return false;
        }
        public void SetField(string fieldName, object value)
        {
            m_FieldsMap[fieldName].SetValue(m_Target, value);
        }
        public void SetProperty(string propertyName, object value)
        {
            m_PropertiesMap[propertyName].SetValue(m_Target, value);
        }
        public IDynamicProxyContext Excute(string methodName, params object[] parameters)
        {
            var isContain = m_BehavioursMap.TryGetValue(methodName, out var method);
            return new DynamicProxyContext(methodName, isContain, m_Target, method, parameters);
        }
        public IDynamicProxyContext<ResultType> Excute<ResultType>(string methodName, params object[] parameters)
        {
            var isContain = m_BehavioursMap.TryGetValue(methodName, out var method);
            return new DynamicProxyContext<ResultType>(methodName, isContain, m_Target, method, parameters);
        }
    }
}