using System;
using System.IO;
using System.Reflection;
#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TestStatements.Runtime.Loader;

public static class RTLoaderExample
{
    public static async Task Main(string[] args)
    {
        const string Title = $"Example for {nameof(Runtime)} ({nameof(RTLoaderExample)})";
        Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
        Console.WriteLine();

        using (Stream stream = new MemoryStream())
        {

            var compilation = CSharpCompilation.Create("a")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).GetType().Assembly.Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(
                @"public static class C{public static int M(int a, int b)=> a*b ^ b+a ; }"));

            var results = compilation.Emit(stream);
            if (results.Success)
            {
                stream.Position = 0L;
                var b = new byte[stream.Length];
#if NET7_0_OR_GREATER
                stream.ReadExactly(b,0,b.Length);
#else
                stream.Read(b,0,b.Length);
#endif
                string l1="", l2 = "";

                for(int i = 0; i < b.Length; i++)
                {
                    l1 += $"{b[i]:X2} ";
                    l2 += $"{(b[i] > 31 && b[i] < 128 ? (char)b[i]:'.')}";
                    if (i % 8 == 7) l1 += ": ";
                    if (i % 16 == 15) {
                        Console.WriteLine($"{i:X4}: {l1} | {l2}");
                        l1 =  l2 = "";

                    }
                }    
                stream.Position = 0L;
#if NET6_0_OR_GREATER
                var context = AssemblyLoadContext.Default;     
                Assembly a = context.LoadFromStream(stream);//<--Exception here.
#else
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                Assembly a = Assembly.Load(bytes);
#endif                
                Console.WriteLine("Evaluation of C.M(?,?)");
                var m = a.GetType("C")?.GetRuntimeMethod("M", [ typeof(int), typeof(int) ]);
                Console.WriteLine();
                Console.Write($"{"",5}");
                for (var j = -5; j < 6; j++)
                    Console.Write($"{j,4}");
                Console.WriteLine();

                for (var i = -5; i<6; i++ )
                {
                    Console.Write($"{i,4}:");
                    for (var j = -5; j < 6; j++ )
                        Console.Write($"{(int?)m?.Invoke(null, [ i, j ]),4}");
                    Console.WriteLine();
                }
            }
        }
    }
}
