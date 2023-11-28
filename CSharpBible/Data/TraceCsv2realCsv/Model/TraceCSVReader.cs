using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TraceCsv2realCsv.Model
{
    public static class TraceCSVReader
    {
        public static void ReadTraceCSV(this CsvModel model, Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                // Check Header
                var line = reader.ReadLine();

                if (line != "[key]; [value]")
                    throw new ArgumentException("Stream does not contain a Trace-csv");

                List<(string header, Dictionary<int, double>? data)> columns = new() {("TimeBase",null) };
                while (!reader.EndOfStream)
                {
                    if (ReadColumn(reader, out var header,out var data))
                        columns.Add((header, data));
                }

                model.SetHeader(columns.ConvertAll((s) => (s.header, (Type?)typeof(double))));

                foreach (var s in columns)
                    model.AddColumnData(s.header, s.data);
            }
        }

        private static bool ReadColumn(StreamReader reader, out string header,out Dictionary<int,double> data)
        {
            var xColumnEnd = false;
            header = "";
            bool xDataMode = false;
            data = new();
            while (!reader.EndOfStream && !xColumnEnd )
            {
                int pp = reader.Peek();
                if (xDataMode && (pp != 59))
                    return !string.IsNullOrEmpty(header);
                var line = reader.ReadLine();

                if (string.IsNullOrEmpty(header) && line.Contains(".Variable;"))
                {
                    header = line.Split(';')[1].Trim();
                }

                if (!xDataMode)
                    xDataMode = line.TrimEnd().EndsWith(".Data;");
                else
                {
                    var ls = line.Split(';');
                    if ((ls.Length > 2)
                        && int.TryParse(ls[1].Trim(), out var ix)
                        && double.TryParse(ls[2].Trim(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out var dVal))
                        data[ix] = dVal;    
                }
            }
            return !string.IsNullOrEmpty(header) && xDataMode;
        }
    }
}
