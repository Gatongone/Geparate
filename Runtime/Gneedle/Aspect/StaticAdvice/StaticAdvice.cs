using Mono.Cecil;

namespace Geparate.Gneedle
{
    internal class StaticAdvice
    {
        private static bool TryInjectAdvice(AssemblyDefinition assemblyDefinition)
        {
            var isSuccess = false;
            foreach (var typeDefinition in assemblyDefinition.MainModule.Types)
            {
            }
            return isSuccess;
        }
    }
}