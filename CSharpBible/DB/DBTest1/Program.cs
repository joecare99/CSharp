using DBTest1.Model;

namespace DBTest1
{
    public static class program
    {
        static program()
        {
            // The Initialization
        }

        public static void Main(params string[] args)
        {
            BasicExample.DoExampleAsync("192.168.0.98", "root", "", "TestDB");
        }
    }
}