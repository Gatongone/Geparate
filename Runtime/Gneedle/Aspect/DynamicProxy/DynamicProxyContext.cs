using System;
using System.Reflection;

namespace Geparate.Gneedle
{
    public struct DynamicProxyContext : IDynamicProxyContext
    {
        private readonly object m_Target;
        private readonly bool m_IsContain;
        private readonly MethodInfo m_Method;
        private readonly string m_MethodName;
        private readonly object[] m_Parameters;
        private Action m_Before;
        private Action m_After;
        private Action<Exception> m_Failed;
        private Action m_Finish;
        private IAdviceStrategy m_Strategy;

        public DynamicProxyContext(string methodName, bool isContain, object target, MethodInfo method, params object[] parameters)
        {
            m_MethodName = methodName;
            m_Target = target;
            m_Method = method;
            m_Parameters = parameters;
            m_IsContain = isContain;
            m_Before = m_After = m_Finish = null;
            m_Failed = null;
            m_Strategy = null;
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

        public IDynamicProxyContext WithFailed(Action<Exception> action)
        {
            m_Failed = action;
            return this;
        }

        public IDynamicProxyContext WithAdviceStrategy(IAdviceStrategy adviceStrategy)
        {
            m_Strategy = adviceStrategy;
            return this;
        }

        public IDynamicProxyContext WithFinish(Action action)
        {
            m_Finish = action;
            return this;
        }

        public object Invoke()
        {
            try
            {
                if (m_IsContain)
                {
                    m_Before?.Invoke();
                    var result = m_Strategy == null ? m_Method.Invoke(m_Target, m_Parameters) : m_Strategy.Advice(m_Target, m_Method, m_Parameters);
                    m_After?.Invoke();
                    return result;
                }
                else
                    throw new Exception($"Method \"{m_MethodName}\" doesn't exist");
            }
            catch (Exception ex)
            {
                m_Failed?.Invoke(ex);
            }
            finally
            {
                m_Finish?.Invoke();
            }
            return null;
        }
    }
}