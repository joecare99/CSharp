using DBTest1.Model;
using MySqlConnector;
using System.Data.Common;

namespace DBTest1
{
    public static class program
    {
        static program()
        {
            // The Initialization
#if NET6_0_OR_GREATER
            DbProviderFactories.RegisterFactory("MySqlConnector", MySqlConnectorFactory.Instance);
            DbProviderFactories.RegisterFactory("OleDBConnector", OleDbConnectorFactory.Instance);
#endif
        }

        public static void Main(params string[] args)
        {
            foreach(var s in DbProviderFactories.GetProviderInvariantNames())
                System.Console.WriteLine(s);
            System.Console.WriteLine(new string('=',50));
            BasicExample.DoExample("192.168.0.98", "root", "", "test");
        }
    }
}