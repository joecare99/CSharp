using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;
using System.IO;

namespace TestStatements.Runtime.Dynamic;

public static class DynamicAssembly
{
    public static void CreateAndSaveAssembly()
    {
        const string Title = $"Example for {nameof(Dynamic)} ({nameof(DynamicAssembly)})";
        Console.WriteLine(Constants.Constants.Header, Title);
        Console.WriteLine();

#if NET9_0_OR_GREATER
        PersistedAssemblyBuilder ab = new PersistedAssemblyBuilder(
            new AssemblyName("MyAssembly"), typeof(object).Assembly);
#elif NET5_0_OR_GREATER
        AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("MyAssembly"), AssemblyBuilderAccess.Run);
#else
        AssemblyBuilder ab = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName("MyAssembly"), AssemblyBuilderAccess.RunAndSave);
#endif
        ModuleBuilder mob = ab.DefineDynamicModule("MyAssembly.dll");
        TypeBuilder tb = mob.DefineType("MyType", TypeAttributes.Public | TypeAttributes.Class);
        MethodBuilder meb = tb.DefineMethod("SumMethod", 
            MethodAttributes.Public | MethodAttributes.Static,
            typeof(int), 
            new Type[] { typeof(int), typeof(int) });
        ILGenerator il = meb.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Ret);

#if !NET9_0_OR_GREATER
        Type? type = tb?.CreateType();
#else
        tb.CreateType();

        using var stream = new MemoryStream();
        ab.Save(stream);  // or pass filename to save into a file
        stream.Seek(0, SeekOrigin.Begin);
        Assembly? assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(stream);
        Type? type = assembly.GetType("MyType");
#endif
        Console.WriteLine("Evaluation of MyType.SumMethod(?,?)");
        MethodInfo? method = type?.GetMethod("SumMethod");
        Console.WriteLine();
        Console.Write($"{"",5}");
        for (var j = -5; j < 6; j++)
            Console.Write($"{j,4}");
        Console.WriteLine();

        for (var i = -5; i < 6; i++)
        {
            Console.Write($"{i,4}:");
            for (var j = -5; j < 6; j++)
                Console.Write($"{(int?)method?.Invoke(null, [i, j]),4}");
            Console.WriteLine();
        }

#if NET9_0_OR_GREATER
        Console.WriteLine("Save MyAssembly.dll");
        stream.Seek(0, SeekOrigin.Begin);
        using var stream2 = new FileStream("MyAssembly.dll",FileMode.Create);
        stream.CopyTo(stream2);        
#elif !NET5_0_OR_GREATER
        Console.WriteLine("Save MyAssembly.dll");
        ab.Save("MyAssembly.dll");
#endif
    }
}
