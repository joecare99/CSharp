using System;
using System.IO;
using System.Reflection;


namespace CallAllExamples
{
    class Program
    {
        static readonly string InvokeTitle =
            "#===================================================================\r\n" +
            "# Invoke: {0}.{1}({2})\r\n" +
            "#===================================================================\r\n";

        static readonly string TestInput =
            "one\r\n" +
            "two\r\n" +
            "\r\n";
        static void Main(string[] args)
        {
            Assembly assem = typeof(TestStatements.Anweisungen.Declarations).Assembly;
            foreach (Type x in assem.GetTypes())
            {
                if (true)//(x.FullName.StartsWith("TestStatements."))
                {
                    foreach (MethodInfo y in x.GetMethods())
                    {
                        if (y.IsPublic && y.IsStatic && (y.ReturnType == typeof(void)))
                        {
                            if (y.GetParameters().Length == 0)
                            {
                                Console.WriteLine(InvokeTitle, x.Name, y.Name, "");
                                object[] param = null;
                                InvokeMethode(y, param);
                            }
                            else if ((y.GetParameters().Length == 1) &&(y.GetParameters()[0].Name.ToLower() == "args"))
                            {

                                Console.WriteLine(InvokeTitle, x.Name, y.Name, "{ }");
                                object[] param = new object[] { new string[] { } };
                                InvokeMethode(y, param);
 
                                Console.WriteLine(InvokeTitle, x.Name, y.Name, "{ \"one\" }");
                                param = new object[] { new string[] { "one" } };
                                InvokeMethode(y, param); 

                                Console.WriteLine(InvokeTitle, x.Name, y.Name, "{ \"one\", \"two\" }");
                                param = new object[] { new string[] { "one", "two" } };
                                InvokeMethode(y, param);

                                Console.WriteLine(InvokeTitle, x.Name, y.Name, "{ \"20\", \"10\" }");
                                param = new object[] { new string[] { "20", "10" } };
                                InvokeMethode(y, param);

                            }

                        }
                    }
                }
            }
            Console.WriteLine("=====================================");
            Console.WriteLine("Press <Enter> to exit");
            Console.ReadLine();
        }

        private static void InvokeMethode(MethodInfo y, object[] param)
        {
            var OldIn = Console.In;
            try
            {
                using (var sr = new StringReader(TestInput))
                {
                    Console.SetIn(sr);
                    y.Invoke(y, param);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("==> EXCEPTION:{1} \"{0}\" <==", e.Message, e.GetType().Name);
            }
            finally
            {
                Console.SetIn(OldIn);
            }
        }
    }
}
