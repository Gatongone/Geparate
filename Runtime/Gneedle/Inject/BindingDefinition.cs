namespace Geparate.Gneedle
{
    public class BindingDefinition
    {
        internal ScopeType scopeType;
        internal object[] paramters;
        public BindingDefinition AsSingleton()
        {
            scopeType = ScopeType.Singleton;
            return this;
        }
        public BindingDefinition AsPrototype()
        {
            scopeType = ScopeType.Prototype;
            return this;
        }
        public BindingDefinition Params(params object[] parameters)
        {
            paramters = parameters;
            return this;
        }
    }
}
