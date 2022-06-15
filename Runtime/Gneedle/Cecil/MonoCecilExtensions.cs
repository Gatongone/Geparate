using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;
using UnityEditor;
using UnityEngine;

namespace Geparate.Gneedle
{
    public static class AssemblyUtilities
    {
        public static bool ContainsAttribute(this TypeDefinition typeDefinition, Type attributeType)
        {
            return typeDefinition.HasCustomAttributes && typeDefinition.CustomAttributes.Any(attribute => attribute.AttributeType.FullName.Equals(attributeType.FullName));
        }

        public static bool ContainsAttribute<T>(this TypeDefinition typeDefinition)
        {
            return typeDefinition.HasCustomAttributes && typeDefinition.CustomAttributes.Any(attribute => attribute.AttributeType.FullName.Equals(typeof(T).FullName));
        }

        public static void AspectAfter<TargetType>(this MethodDefinition methodDefinition, ModuleDefinition moduleDefinition, string methodName)
        {
            var worker = methodDefinition.Body.GetILProcessor();
            var ins = methodDefinition.Body.Instructions[methodDefinition.Body.Instructions.Count - 1];
            worker.InsertBefore(ins, worker.Create(OpCodes.Call,
                moduleDefinition.ImportReference(typeof(TargetType).GetMethod(methodName, null))));
        }

        public static void AspectAfter<TargetType,ParamType>(this MethodDefinition methodDefinition, ModuleDefinition moduleDefinition, string methodName,ParamType parameter)
        {
            var methodRef = methodDefinition.GetElementMethod();
            
            var worker = methodDefinition.Body.GetILProcessor();
            var ins = methodDefinition.Body.Instructions[methodDefinition.Body.Instructions.Count - 1];
            
            
            worker.InsertBefore(ins, worker.Create(OpCodes.Ldarg_0));
            typeof(TargetType).GetMethod(methodName, new Type[] {typeof(ParamType)});
            worker.InsertBefore(ins, worker.Create(OpCodes.Call,
                moduleDefinition.ImportReference(typeof(TargetType).GetMethod(methodName, new Type[] {typeof(ParamType)}))));
        }
    }
}