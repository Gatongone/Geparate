using System;
using System.Reflection;

namespace Geparate.Gneedle
{
    public struct DynamicProxyContext<ResultType> : IDynamicProxyContext<ResultType>
    {
        private readonly object m_Target;
        private readonly bool m_IsContain;
        private readonly MethodInfo m_Method;
        private readonly string m_MethodName;
        private readonly object[] m_Parameters;
        private Action m_Before;
        private Action m_After;
        private Action<Exception> m_Faild;
        private Action m_Finish;
        public DynamicProxyContext(string methodName,bool isContain,object target,MethodInfo method,params object[] parameters)
        {
            m_MethodName = methodName;
            m_Target = target;
            m_Method = method;
            m_Parameters = parameters;
            m_IsContain = isContain;
            m_Before = m_After = m_Finish = null;
            m_Faild = null;
        }
        public IDynamicProxyContext<ResultType> WithBefore(Action action)
        {
            m_Before = action;
            return this;
        }
        public IDynamicProxyContext<ResultType> WithAfter(Action action)
        {
            m_After = action;
            return this;
        }
        public IDynamicProxyContext<ResultType> WithFaild(Action<Exception> action)
        {
            m_Faild = action;
            return this;
        }
        public IDynamicProxyContext<ResultType> WithFinish(Action action)
        {
            m_Finish = action;
            return this;
        }
        public ResultType Result()
        {
            try
            {
                if (m_IsContain)
                {
                    m_Before?.Invoke();
                    var result = (ResultType)m_Method.Invoke(m_Target, m_Parameters);
                    m_After?.Invoke();
                    return result;
                }
                else
                    throw new Exception($"Method \"{m_MethodName}\" doesn't exsist");
            }
            catch (Exception ex)
            {
                m_Faild?.Invoke(ex);
            }
            finally
            {
                m_Finish?.Invoke();
            }
            return default;
        }
    }
    public struct DynamicProxyContext : IDynamicProxyContext
    {
        private readonly object m_Target;
        private readonly bool m_IsContain;
        private readonly MethodInfo m_Method;
        private readonly string m_MethodName;
        private readonly object[] m_Parameters;
        private Action m_Before;
        private Action m_After;
        private Action<Exception> m_Faild;
        private Action m_Finish;
        public DynamicProxyContext(string methodName,bool isContain,object target,MethodInfo method,params object[] parameters)
        {
            m_MethodName = methodName;
            m_Target = target;
            m_Method = method;
            m_Parameters = parameters;
            m_IsContain = isContain;
            m_Before = m_After = m_Finish = null;
            m_Faild = null;
        }
        public IDynamicProxyContext WithBefore(Action action)
        {
            m_Before = action;
            return this;
        }
        public IDynamicProxyContext WithAfter(Action action)
        {
            m_After = action;
            return this;
        }
        public IDynamicProxyContext WithFaild(Action<Exception> action)
        {
            m_Faild = action;
            return this;
        }
        public IDynamicProxyContext WithFinish(Action action)
        {
            m_Finish = action;
            return this;
        }
        public void Done()
        {
            try
            {
                if (m_IsContain)
                {
                    m_Before?.Invoke();
                    m_Method.Invoke(m_Target, m_Parameters);
                    m_After?.Invoke();
                }
                else
                    throw new Exception($"Method \"{m_MethodName}\" doesn't exsist");
            }
            catch (Exception ex)
            {
                m_Faild?.Invoke(ex);
            }
            finally
            {
                m_Finish?.Invoke();
            }
        }
    }
    
}