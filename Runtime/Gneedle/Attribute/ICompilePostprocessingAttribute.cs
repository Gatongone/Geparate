using Mono.Cecil;

namespace Geparate.Gneedle
{
    public interface ICompilePostprocessingAttribute
    {
        void BakeAttribute(ModuleDefinition moduleDefinition,ICustomAttributeProvider target);

    }
}