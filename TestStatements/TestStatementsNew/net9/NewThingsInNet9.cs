using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestStatementsNew.net9;

public class NewThingsInNet9
{
    public void AggregateByTest()
    {
        (string id, int score)[] data =
        [
            ("0", 42),
            ("1", 5),
            ("2", 4),
            ("1", 10),
            ("0", 25),
        ];

#if NET9_0_OR_GREATER
        var aggregatedData =
            data.AggregateBy(
                keySelector: entry => entry.id,
                seed: 0,
                (totalScore, curr) => totalScore + curr.score
                );
#else
        var aggregatedData = data
            .GroupBy(entry => entry.id)
            .Select(group => (id: group.Key, totalScore: group.Sum(entry => entry.score)))
            .ToList();
#endif
        foreach (var item in aggregatedData)
        {
            Console.WriteLine(item);
        }
        //(0, 67)
        //(1, 15)
        //(2, 4)
    }

    public void IndexTest()
    {
        IEnumerable<string> lines2 = File.ReadAllLines("output.txt");
#if NET9_0_OR_GREATER
        foreach ((int index, string line) in lines2.Index())
        {
            Console.WriteLine($"Line number: {index + 1}, Line: {line}");
        }
#else
        foreach (var (index, line) in (IEnumerable<(int index, string line)>?)lines2?.Select((line, index) => (index, line)))
        {
            Console.WriteLine($"Line number: {index + 1}, Line: {line}");
        }
#endif
    }

    public void CreateAndSaveAssembly(string assemblyPath)
    {
#if NET9_0_OR_GREATER
        PersistedAssemblyBuilder ab = (PersistedAssemblyBuilder)PersistedAssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("MyAssembly"),
            AssemblyBuilderAccess.Run
            );
#else
        AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("MyAssembly"),
            AssemblyBuilderAccess.Run
            );
#endif
        TypeBuilder tb = ab.DefineDynamicModule("MyModule")
            .DefineType("MyType", TypeAttributes.Public | TypeAttributes.Class);

        MethodBuilder mb = tb.DefineMethod(
            "SumMethod",
            MethodAttributes.Public | MethodAttributes.Static,
            typeof(int), [typeof(int), typeof(int)]
            );
        ILGenerator il = mb.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Ret);

        tb.CreateType();
#if NET9_0_OR_GREATER
        ab.Save(assemblyPath); // or could save to a Stream
#else
        
#endif
    }

    public void UseAssembly(string assemblyPath)
    {
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        Type? type = assembly.GetType("MyType");
        MethodInfo? method = type?.GetMethod("SumMethod");
        Console.WriteLine(method?.Invoke(null, [5, 10]));
    }

}