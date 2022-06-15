#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Compilation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Geparate.Logger;
using Mono.Cecil;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Geparate.Gneedle
{
    [InitializeOnLoad]
    public static class CompilationPostprocessing
    {
        private static readonly string[] s_IgnoreAssemblyStartWith =
        {
            "UnityEngine.",
            "UnityEditor.",
            "Unity.",
            "Mono.Cecil",
            "log4net",
            "mscorlib.",
            "nunit.framework",
            "unityplastic"
        };

        private static bool s_IsEnableAutoCompilation
        {
            get => EditorPrefs.GetBool("GEPARATE_GNEEDLE_ENABLE_AUTO_COMPILATION", false);
            set => EditorPrefs.SetBool("GEPARATE_GNEEDLE_ENABLE_AUTO_COMPILATION", value);
        }

        static CompilationPostprocessing()
        {
            if (s_IsEnableAutoCompilation)
                InjectAssembly();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void CreateRecompilationObject()
        {
            if (!s_IsEnableAutoCompilation)
                return;
            var gameObject = new GameObject("EndPlayModeEvent");
            gameObject.AddComponent<RecomilationOnQuitPlayMode>();
        }

        [MenuItem("Geparate/Gneedle/Auto Inject")]
        private static void AutoInject()
        {
            s_IsEnableAutoCompilation = !s_IsEnableAutoCompilation;
            var enableStyle = string.Empty;
            enableStyle = s_IsEnableAutoCompilation ? RichFontStyle.ColorfulFont("True", FontColor.GREEN) : RichFontStyle.ColorfulFont("False", FontColor.RED);

            Log.Info("Enable auto inject: " + enableStyle);
        }


        [MenuItem("Geparate/Gneedle/Clear Injections")]
        public static void ReCompilation()
        {
            CompilationPipeline.RequestScriptCompilation();
            Log.Success("Recompilation successfully !");
        }

        [MenuItem("Geparate/Gneedle/Inject Assemblies")]
        private static void InjectAssembly()
        {
            try
            {
                EditorApplication.LockReloadAssemblies();
                var resolver = new DefaultAssemblyResolver();
                var assemblyPaths = new HashSet<string>();
                var typeName2TypeMap = new ConcurrentDictionary<string, Type>();
                var assemblies = new ConcurrentBag<System.Reflection.Assembly>();

                //Add process dlls in the project
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.FullName.StartsWith("JetBrains"))
                        continue;
                    if (!IsIgnoreAssembly(assembly) && assembly.Location.Replace('\\', '/').StartsWith(Application.dataPath.Substring(0, Application.dataPath.Length - 7)))
                    {
                        assemblyPaths.Add(assembly.Location);
                        assemblies.Add(assembly);
                    }

                    resolver.AddSearchDirectory(Path.GetDirectoryName(assembly.Location));
                }

                //Add Unity managed dlls
                resolver.AddSearchDirectory(Path.GetDirectoryName(EditorApplication.applicationPath) + "/Data/Managed");

                var paths = assemblyPaths.Where(p => !p.Contains("Geparate.Gneedle"));
                //Sync
                if (s_IsEnableAutoCompilation)
                {
                    var index = 1;
                    foreach (var path in paths)
                    {
                        EditorUtility.DisplayProgressBar("Injecting assemblies",path,index++/paths.Count());
                        var readerParameters = new ReaderParameters();
                        var writerParameters = new WriterParameters();

                        readerParameters.AssemblyResolver = resolver;
                        readerParameters.ReadWrite = true;
                        readerParameters.ReadingMode = ReadingMode.Immediate;
                        using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(path, readerParameters))
                        {
                            CompileAttribute(assemblyDefinition, assemblies, typeName2TypeMap);
                            assemblyDefinition.Write(writerParameters);
                        }
                    }
                }
                //Async
                else
                {
                    var countdownEvent = new CountdownEvent(paths.Count());
                    Parallel.ForEach(paths, path =>
                    {
                        AssemblyDefinition assemblyDefinition = null;
                        try
                        {
                            var readerParameters = new ReaderParameters();
                            var writerParameters = new WriterParameters();

                            readerParameters.AssemblyResolver = resolver;
                            readerParameters.ReadWrite = true;
                            readerParameters.ReadingMode = ReadingMode.Immediate;
                            using (assemblyDefinition = AssemblyDefinition.ReadAssembly(path, readerParameters))
                            {
                                CompileAttribute(assemblyDefinition, assemblies, typeName2TypeMap);
                                assemblyDefinition.Write(writerParameters);
                                countdownEvent.Signal();
                            }
                        }
                        catch (Exception e)
                        {
                            assemblyDefinition?.Dispose();
                            countdownEvent.Signal();
                            throw;
                        }
                    });
                    countdownEvent.Wait();
                }

                Log.Success("Inject assemblies successfully !");
                EditorApplication.UnlockReloadAssemblies();
            }
            catch (Exception ex)
            {
                Log.Error($"Inject failed ! {ex.InnerException?.Message}\nStackTrace:{ex.InnerException?.StackTrace}");
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
                EditorUtility.ClearProgressBar();
            }
        }

        private static bool IsIgnoreAssembly(System.Reflection.Assembly assembly)
        {
            return s_IgnoreAssemblyStartWith.Any(t => assembly.GetName().Name.StartsWith(t));
        }

        private static void CompileAttribute(AssemblyDefinition assemblyDefinition, ConcurrentBag<System.Reflection.Assembly> assembly, ConcurrentDictionary<string, Type> registeredAttributeMap)
        {
            if (assemblyDefinition == null)
            {
                Debug.Log("NULL!");
                return;
            }

            foreach (var moduleDefinition in assemblyDefinition.Modules)
            {
                foreach (var typeDefinition in moduleDefinition.Types)
                {
                    if (typeDefinition.ContainsAttribute<HasBeenProcessedAttribute>())
                        continue;
                    InjectHasBeenProcessedAttribute(assemblyDefinition, typeDefinition);
                    TryCompileAttributeProvider(assemblyDefinition, typeDefinition, assembly, registeredAttributeMap);
                    foreach (var fieldDefinition in typeDefinition.Fields)
                        TryCompileAttributeProvider(assemblyDefinition, fieldDefinition, assembly, registeredAttributeMap);
                    foreach (var propertyDefinition in typeDefinition.Properties)
                        TryCompileAttributeProvider(assemblyDefinition, propertyDefinition, assembly, registeredAttributeMap);
                    foreach (var methodDefinition in typeDefinition.Methods)
                        TryCompileAttributeProvider(assemblyDefinition, methodDefinition, assembly, registeredAttributeMap);
                }
            }
        }

        private static void InjectHasBeenProcessedAttribute(AssemblyDefinition assembly, TypeDefinition classType)
        {
            var attr = assembly.MainModule.ImportReference(typeof(HasBeenProcessedAttribute).GetConstructor(Type.EmptyTypes));
            classType.CustomAttributes.Add(new CustomAttribute(attr));
        }

        private static void TryCompileAttributeProvider(AssemblyDefinition assemblyDefinition, ICustomAttributeProvider attributeProvider, ConcurrentBag<System.Reflection.Assembly> assemblies, ConcurrentDictionary<string, Type> registeredAttributeMap)
        {
            if (attributeProvider == null || !attributeProvider.HasCustomAttributes)
                return;
            foreach (var attribute in attributeProvider.CustomAttributes)
            {
                foreach (var @interface in attribute.AttributeType.Resolve().Interfaces)
                {
                    if (!@interface.InterfaceType.FullName.Equals(typeof(ICompilePostprocessingAttribute).FullName))
                        continue;
                    var arguments = attribute.ConstructorArguments;
                    var parameters = new object[arguments.Count];
                    var ctorTypes = new Type[arguments.Count];
                    if (arguments.Count != 0)
                    {
                        for (var index = 0; index < attribute.ConstructorArguments.Count; index++)
                        {
                            parameters[index] = arguments[index].Value;
                            ctorTypes[index] = arguments[index].Value.GetType();
                        }
                    }

                    var typeName = attribute.AttributeType.FullName;

                    if (!registeredAttributeMap.TryGetValue(typeName, out var attributeType))
                    {
                        foreach (var assembly in assemblies)
                        {
                            attributeType = assembly.GetType(typeName);
                            if (attributeType != null)
                            {
                                registeredAttributeMap.TryAdd(typeName, attributeType);
                                break;
                            }
                        }
                    }

                    if (attributeType == null)
                        throw new InjectFailedException($"Attribute: \"{typeName}\" exists in an invalid assembly");

                    var instance = (ICompilePostprocessingAttribute) Activator.CreateInstance(attributeType, parameters);
                    instance.BakeAttribute(assemblyDefinition.MainModule, attributeProvider);
                }
            }
        }
    }
}
#endif