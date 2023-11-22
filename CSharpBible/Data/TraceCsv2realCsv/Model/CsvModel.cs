using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using BaseLib.Helper;

namespace TraceCsv2realCsv.Model
{
    public class CsvModel
    {
        #region internal Class
        public class _DataRows
        {
            CsvModel _parent;
            public int Count => _parent._data.Count;

            public int Fields => _parent._header.Count;

            public Dictionary<string, object> this[int ix]
            {
                get
                {
                    var val = new Dictionary<string, object>();
                    var row = _parent._data.ElementAt(ix);
                    var i = 0;
                    val[_parent._header[i++].name] = row.Key;
                    foreach (var fieldval in _parent._data.ElementAt(ix).Value)
                        val[_parent._header[i++].name] = fieldval;
                    return val;
                }
            }

            public _DataRows(CsvModel parent)
                => _parent = parent;
        }
        #endregion
        #region Properties
        // Header
        private List<(string name, Type? type)> _header = new();
        // Data
        private Dictionary<object, List<object?>> _data = new();
        private List<string> _separators = new() { ";", "\t", "," };
        const char cQuotation = '\"';
        public _DataRows Rows { get; }
        #endregion

        #region Methods
        public CsvModel()
        {
            Rows = new _DataRows(this);
        }
        public bool ReadCsv(Stream st)
            => ReadCsv(st, _separators);

        public bool ReadCsv(Stream st, List<string> separators)
        {
            using var tr = new StreamReader(st, true);
            // Header
            var line = tr.ReadLine();
            var seperator = _separators.FirstOrDefault(s => line.Contains(s)) ?? ",";
            _header.Clear();
            var header = line.Split(new string[] { seperator }, StringSplitOptions.None).ToList();

            // Lesen Sie die ersten fünf Zeilen der CSV-Datei und trennen Sie die Werte.
            var lines = new List<List<string>>();
            for (int i = 0; i < 5 && !tr.EndOfStream; i++)
            {
                lines.Add(SplitCSVLine(tr.ReadLine(), seperator, cQuotation));
            }

            // Bestimmen Sie den Datentyp jeder Spalte.
            for (int i = 0; i < header.Count; i++)
            {
                _header.Add((header[i], GetColumnType(lines.Select(l => l[i]).ToList())));
            }

            foreach (var lnVals in lines)
                AppendData(CastList(lnVals));

            while (!tr.EndOfStream)
            {
                line = tr.ReadLine();
                var values = SplitCSVLine(line, seperator, cQuotation);

                AppendData(CastList(values));
            }
            return true;
        }

        public void AppendData(List<object?> values)
        // _data
        {
            // Hier können Sie die Werte in Ihre Klasse einfügen.
            var Index = values[0];
            values.RemoveAt(0);
            _data.Add(Index, values);
        }

        public void AppendData(object[][] values)
        // _data
        {
            // Hier können Sie die Werte in Ihre Klasse einfügen.
            foreach (object?[] o in values)
                AppendData(o.ToList());
        }
        public List<object?> CastList(List<string>? values)
        // Benutzt _header
        {
            List<object?> _List = new();
            for (int i = 0;values != null && i < Math.Min(values.Count, _header.Count); i++)
                _List.Add(_header[i].type?.Get(values[i]));
            return _List;
        }

        public static List<string> SplitCSVLine(string line, string seperator, char quotation = cQuotation)
        {
            var values = new List<string>();
            int i = 0;
            int pn;
            if (string.IsNullOrEmpty(seperator))
                return new() { line };
            string value = "";

            while (i < line.Length)
            {
                if (line.Substring(i).StartsWith(seperator))
                {
                    values.Add(value);
                    i += seperator.Length;
                    value = "";
                    if (i == line.Length)
                        values.Add(value);
                }
                else if ((line[i] == quotation)
                    && (((pn = line.IndexOf($"{quotation}" + seperator, i)) > -1)
                    || (line[pn = line.Length - 1] == quotation)))
                {
                    value = line.Substring(i, pn - i + 1);
                    i = pn + 1; // length of quotation
                    if (i == line.Length)
                        values.Add(value);
                }
                else if ((line[i] != quotation)
                    && (pn = line.IndexOf(seperator, i)) > -1)
                {
                    value = line.Substring(i, pn - i);
                    i = pn;
                }
                else
                {
                    values.Add(line.Substring(i));
                    i = line.Length;
                }
            }
            return values;
        }

        public static Type GetColumnType(List<string> values, char quotation = cQuotation)
        {
            // Versuchen Sie zuerst, Mit den Qutations die Strings herauszufiltern.
#if NET5_0_OR_GREATER
            if (values.All(v => v.StartsWith(quotation) && v.EndsWith(quotation)))
#else
            if (values.All(v => v.StartsWith(quotation.ToString()) && v.EndsWith(quotation.ToString())))
#endif
                return typeof(string);
            else
                // Dann versuchen Sie, den Datentyp als Ganzzahl zu bestimmen.
                if (values.All(v => int.TryParse(v, out _)))
            {
                return typeof(int);
            }

            // Versuchen Sie dann, den Datentyp als Gleitkommazahl zu bestimmen.
            else if (values.All(v => double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out _)))
            {
                return typeof(double);
            }

            // Wenn alle Werte Zeichenfolgen sind oder nicht in einen Ganzzahl- oder Gleitkommazahl-Datentyp konvertiert werden können,
            // geben Sie den Datentyp als Zeichenfolge zurück.
            else
                return typeof(string);
        }

        public void SetHeader(List<string> lsHNames)
        {
            _data.Clear();
            _header.Clear();
            foreach (string s in lsHNames)
                _header.Add((s, typeof(object)));
        }
        public void SetHeader(List<(string , Type?)> lsHeader)
        {
            _data.Clear();
            _header.Clear();
            _header = lsHeader;
        }

        public void WriteCSV(Stream stream, char cSeparator = '\t')
        {
            using var tw = new StreamWriter(stream, Encoding.UTF8);
            // Write Header
            tw.WriteLine(string.Join($"{cSeparator}", _header.ConvertAll((l) => $"\"{l.name}\"")));
            // Write Data
            foreach (var l in _data)
                tw.WriteLine($"{(l.Key is string sl ? $"\"{sl.Quote()}\"" : l.Key)}{cSeparator}" + string.Join($"{cSeparator}", l.Value.ConvertAll((l) => l is string sl ? $"\"{sl.Quote()}\"" : l is double d ? $"{d.ToString(CultureInfo.InvariantCulture)}" : $"{l}")));

        }

        public void AddColumnData(string header, Dictionary<int, double>? data)
        {
            var icolidx = _header.ConvertAll(s => s.name).IndexOf(header);
            if (icolidx > 0 && data !=null)
                foreach (var r in data)
                    if (_data.TryGetValue(r.Key, out var ls))
                        ls[icolidx - 1] = r.Value;
                    else
                    {
                        ls = new();
                        foreach (var h in _header)
                            if (h.name != _header[0].name)
                                ls.Add(0d);
                        ls[icolidx - 1] = r.Value;
                        _data.Add(r.Key, ls);
                    }
        }
        #endregion
    }
}
