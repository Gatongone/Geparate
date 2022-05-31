using System.Reflection;

namespace Geparate.Gneedle.Aspect
{
    public readonly struct FieldContext<T>
    {
        private readonly FieldInfo m_FieldInfo;
        private readonly object m_Target;

        public FieldContext(FieldInfo fieldInfo, object target)
        {
            m_FieldInfo = fieldInfo;
            m_Target = target;
        }

        public T GetValue()
        {
            return (T) m_FieldInfo.GetValue(m_Target);
        }

        public void SetValue(object value)
        {
            m_FieldInfo.SetValue(m_Target, value);
        }
    }

    public readonly struct PropertyContext<T>
    {
        private readonly PropertyInfo m_PropertyInfo;
        private readonly object m_Target;

        public PropertyContext(PropertyInfo propertyInfo, object target)
        {
            m_PropertyInfo = propertyInfo;
            m_Target = target;
        }

        public T GetValue()
        {
            return (T) m_PropertyInfo.GetValue(m_Target);
        }

        public void SetValue(object value)
        {
            m_PropertyInfo.SetValue(m_Target, value);
        }
    }
}