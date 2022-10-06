using System;
using System.Reflection;

namespace TestStatements.Reflection
{
    public class AssemblyExample
    {
        private int factor;
        public AssemblyExample(int f)
        {
            factor = f;
        }

        public int SampleMethod(int x)
        {
            Console.WriteLine("\nExample.SampleMethod({0}) executes.", x);
            return x * factor;
        }

        public static void ExampleMain()
        {
            Assembly assem = typeof(Program).Assembly;

            Console.WriteLine("Assembly Full Name:");
            Console.WriteLine(assem.FullName);

            // The AssemblyName type can be used to parse the full name.
            AssemblyName assemName = assem.GetName();
            Console.WriteLine("\nName: {0}", assemName.Name);
            Console.WriteLine("Version: {0}.{1}",
                assemName.Version.Major, assemName.Version.Minor);

            Console.WriteLine("\nAssembly CodeBase:");
            Console.WriteLine(assem.CodeBase);

            // Create an object from the assembly, passing in the correct number
            // and type of arguments for the constructor.
            Object o = assem.CreateInstance("TestStatements.Reflection.AssemblyExample", false,
                BindingFlags.ExactBinding,
                null, new Object[] { 2 }, null, null);

            foreach (Type x in assem.GetTypes())
            {
                Console.WriteLine(x.FullName);
                if (x.FullName.StartsWith("TestStatements."))
                {
                    foreach (MethodInfo y in x.GetMethods())
                    {
                        if (y.IsPublic && y.IsStatic && (y.ReturnType == typeof(void)))
                        {
                            Console.Write(" '-> .{0}(", y.Name);
                            foreach (ParameterInfo a in y.GetParameters())
                            {
                                if (a.Position > 0) Console.Write(", ");
                                if (a.IsOut) Console.Write("out ");
                                Console.Write("{0} {1}", a.ParameterType.Name, a.Name);
                            }
                            Console.WriteLine(")");
                        }
                    }
                }
            }
            // Make a late-bound call to an instance method of the object.    
            MethodInfo m = assem.GetType("TestStatements.Reflection.AssemblyExample")?.GetMethod("SampleMethod");
            Object ret = m?.Invoke(o, new Object[] { 42 });
            Console.WriteLine("SampleMethod returned {0}.", ret);

            Console.WriteLine("\nAssembly entry point:");
            Console.WriteLine(assem.EntryPoint);
        }
    }
    /* This code example produces output similar to the following:

    Assembly Full Name:
    source, Version=1.0.2000.0, Culture=neutral, PublicKeyToken=null

    Name: source
    Version: 1.0

    Assembly CodeBase:
    file:///C:/sdtree/AssemblyClass/cs/source.exe

    Example.SampleMethod(42) executes.
    SampleMethod returned 84.

    Assembly entry point:
    Void Main()
     */
}