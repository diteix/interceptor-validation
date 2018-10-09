using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using System;
using System.IO;

namespace Interceptor
{
    internal static class Injector
    {
        public static void InjectAttribute(string assemblyPath, System.Reflection.MethodInfo methodToBeInvoked, string keyfile = null)
        {
            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException($"Assembly not found at: {assemblyPath}");
            }

            if (!methodToBeInvoked.Attributes
                .HasFlag(System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static))
            {
                throw new NotSupportedException("Method not supported. It must be Private and static.");
            }

            bool hasPdb = HasPdbFile(assemblyPath);

            using (var assembly = ReadAssembly(assemblyPath, hasPdb))
            {
                InjectMethod(assembly, methodToBeInvoked);

                WriteAssembly(assembly, assemblyPath, keyfile, hasPdb);
            }
        }

        private static bool HasPdbFile(string assemblyPath)
        {
            string path = Path.ChangeExtension(assemblyPath, ".pdb");

            return File.Exists(path);
        }

        private static AssemblyDefinition ReadAssembly(string assemblyPath, bool hasPdb)
        {
            using (var resolver = new DefaultAssemblyResolver())
            {
                resolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));

                var readParams = new ReaderParameters(ReadingMode.Immediate)
                {
                    AssemblyResolver = resolver,
                    InMemory = true
                };

                if (hasPdb)
                {
                    readParams.ReadSymbols = true;
                    readParams.SymbolReaderProvider = new PdbReaderProvider();
                }

                return AssemblyDefinition.ReadAssembly(assemblyPath, readParams);
            }
        }

        private static void InjectMethod(AssemblyDefinition assembly, System.Reflection.MethodInfo methodToBeInvoked)
        {
            foreach (var type in assembly.MainModule.Types)
            {
                if (type.BaseType == null)
                {
                    continue;
                }

                foreach (var methodDefinition in type.Resolve().Methods)
                {
                    if (methodDefinition.Name != "Teste") { continue; }

                    var processor = methodDefinition.Body.GetILProcessor();
                    var methodReference = assembly.MainModule.ImportReference(methodToBeInvoked);

                    var methodCall = processor.Create(OpCodes.Call, methodReference);
                    processor.InsertBefore(methodDefinition.Body.Instructions[0], methodCall);

                    for (var i = methodDefinition.Parameters.Count - 1; i >= 0; i--)
                    {
                        var dup = processor.Create(OpCodes.Dup);
                        var arrIndex = processor.Create(OpCodes.Ldc_I4, i);
                        var arg = processor.Create(OpCodes.Ldarg, i + 1);
                        var box = processor.Create(OpCodes.Box, methodDefinition.Parameters[i].ParameterType);
                        var stem = processor.Create(OpCodes.Stelem_Ref);

                        processor.InsertBefore(methodDefinition.Body.Instructions[0], stem);
                        processor.InsertBefore(methodDefinition.Body.Instructions[0], box);
                        processor.InsertBefore(methodDefinition.Body.Instructions[0], arg);
                        processor.InsertBefore(methodDefinition.Body.Instructions[0], arrIndex);
                        processor.InsertBefore(methodDefinition.Body.Instructions[0], dup);
                    }

                    if (methodDefinition.Parameters.Count > 0)
                    {
                        var arrLength = processor.Create(OpCodes.Ldc_I4, methodDefinition.Parameters.Count);
                        var arr = processor.Create(OpCodes.Newarr, assembly.MainModule.ImportReference(typeof(object)));

                        processor.InsertBefore(methodDefinition.Body.Instructions[0], arr);
                        processor.InsertBefore(methodDefinition.Body.Instructions[0], arrLength);
                    }
                }
            }
        }

        private static void WriteAssembly(AssemblyDefinition assembly, string assemblyPath, string keyfile, bool hasPdb)
        {
            var writeParams = new WriterParameters();

            if (hasPdb)
            {
                writeParams.WriteSymbols = true;
                writeParams.SymbolWriterProvider = new PdbWriterProvider();
            }

            if (keyfile != null)
            {
                writeParams.StrongNameKeyPair = new System.Reflection.StrongNameKeyPair(File.ReadAllBytes(keyfile));
            }

            assembly.Write(assemblyPath, writeParams);
        }
    }
}
