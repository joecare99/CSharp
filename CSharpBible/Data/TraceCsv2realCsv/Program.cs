using System.IO;
using TraceCsv2realCsv.Model;

namespace TraceCsv2realCsv
{
    public class Program
    {
        private static CsvModel csv;

        static void Main(string[] args)
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                csv = new CsvModel();
                csv.ReadTraceCSV(new FileStream(args[0], FileMode.Open));
                if (args.Length > 1 && !File.Exists(args[1]))
                {
                    csv.WriteCSV(new FileStream(args[1], FileMode.CreateNew));
                }
            }
        }
    } 
}