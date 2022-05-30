namespace Geparate.Gneedle
{
    public class BindContext
    {
        internal ScopeType m_ScopeType;
        internal object[] m_Paramters;
        public BindContext AsSingleton()
        {
            m_ScopeType = ScopeType.Singleton;
            return this;
        }
        public BindContext AsPrototype()
        {
            m_ScopeType = ScopeType.Prototype;
            return this;
        }
        public BindContext Params(params object[] paramters)
        {
            m_Paramters = paramters;
            return this;
        }
    }
}
