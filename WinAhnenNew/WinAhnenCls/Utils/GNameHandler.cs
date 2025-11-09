using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WinAhnenCls.Utils
{
    public sealed class GNameHandler
    {
        public delegate void SendMsg(string msg, int aType);

        private bool _cfgLearnUnknown = true;
        private string _gNameFile = string.Empty;
        private bool _gNameListChanged;
        private readonly SortedDictionary<string, string> _gNameList = new(StringComparer.Ordinal);
        private SendMsg? _onError;

        public SendMsg? OnError
        {
            get => _onError;
            set
            {
                if (_onError == value) return;
                _onError = value;
            }
        }

        public bool CfgLearnUnknown
        {
            get => _cfgLearnUnknown;
            set
            {
                if (_cfgLearnUnknown == value) return;
                _cfgLearnUnknown = value;
            }
        }

        public bool Changed
        {
            get => _gNameListChanged;
            set
            {
                if (_gNameListChanged == value) return;
                _gNameListChanged = value;
            }
        }

        private const string csUnknown = "…";
        private const string csUnknown2 = "...";

        public void Init()
        {
            _gNameList.Clear();
            _cfgLearnUnknown = true;
            _gNameListChanged = false;
            _gNameFile = string.Empty;
        }

        public void Done()
        {
            try
            {
                if (_gNameListChanged && !string.IsNullOrEmpty(_gNameFile))
                {
                    // Delphi: delete then save
                    if (File.Exists(_gNameFile))
                        File.Delete(_gNameFile);

                    SafeSave(_gNameFile);
                }
            }
            finally
            {
                _gNameList.Clear();
            }
        }

        public void LoadGNameList(string aFilename)
        {
            if (File.Exists(aFilename))
            {
                _gNameList.Clear();
                foreach (var line in File.ReadLines(aFilename, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    int idx = line.IndexOf('=');
                    if (idx < 0)
                    {
                        // Name ohne Wert
                        var nameOnly = line;
                        if (!_gNameList.ContainsKey(nameOnly))
                            _gNameList[nameOnly] = string.Empty;
                    }
                    else
                    {
                        var name = line.Substring(0, idx);
                        var value = line.Substring(idx + 1);
                        _gNameList[name] = value;
                    }
                }
            }
            else
            {
                _gNameList.Clear(); // TODO: Defaults laden
            }

            _gNameFile = aFilename;
            _gNameListChanged = false;
        }

        public void SaveGNameList(string aFilename = "")
        {
            if (string.IsNullOrEmpty(aFilename) && string.IsNullOrEmpty(_gNameFile))
                return;

            if (!string.IsNullOrEmpty(aFilename))
                _gNameFile = aFilename;

            SafeSave(_gNameFile);
            _gNameListChanged = false;
        }

        public void SetGNLFilename(string aFilename = "")
        {
            if (string.IsNullOrEmpty(aFilename) && string.IsNullOrEmpty(_gNameFile))
                return;

            if (!string.IsNullOrEmpty(aFilename))
                _gNameFile = aFilename;
        }

        public void LearnSexOfGivnName(string aName, char aSex)
        {
            if (aName == null) return;

            foreach (var token in SplitBySpace(aName))
            {
                var lName = token;

                // Ignoriere abgekürzte Namen: "A.", "B.", ...
                if (IsAbbrev(lName))
                    continue;

                // Ignoriere „…“ / „...“ am Anfang
                if (TestFor(lName, 1, csUnknown) || TestFor(lName, 1, csUnknown2))
                    continue;

                // Ungültige Namen: kürzer als 3 und nicht "NN", oder Endet mit '.' oder '='
                if ((lName.Length < 3 && !lName.Equals("NN", StringComparison.OrdinalIgnoreCase))
                    || lName.EndsWith(".", StringComparison.Ordinal)
                    || lName.EndsWith("=", StringComparison.Ordinal))
                {
                    if (lName.Length > 0)
                        _onError?.Invoke($"\"{lName}\" is not a valid Name", 0);

                    // weiter prüfen andere Tokens
                }
                else if (lName.Length > 1 && char.IsLower(lName[0]))
                {
                    if (lName.Length > 0)
                        _onError?.Invoke($"\"{lName}\" is not a valid Name (case)", 0);
                }
                else if (lName.Length > 0
                         && !StartsWith(lName, "(")
                         && !StartsWith(lName, "\"")
                         && (GetValueOrEmpty(lName).Length == 0 || GetValueOrEmpty(lName) == "_"))
                {
                    _gNameList[lName] = aSex.ToString();
                    _gNameListChanged = true;
                }
                else if (!StartsWith(lName, "("))
                {
                    // Delphi: if (copy(lName,1,1) <> '(') then break;
                    break;
                }
            }
        }

        public char GuessSexOfGivnName(string aName, bool bLearn = true)
        {
            char result = 'U';
            if (aName == null) return result;

            foreach (var token in SplitBySpace(aName))
            {
                var lName = token;

                if (IsAbbrev(lName))
                    continue;

                if ((lName.Length < 3 && !lName.Equals("NN", StringComparison.OrdinalIgnoreCase))
                    || lName.EndsWith(".", StringComparison.Ordinal)
                    || lName.EndsWith("=", StringComparison.Ordinal))
                {
                    if (lName.Length > 0 && bLearn)
                        _onError?.Invoke($"\"{lName}\" is not a valid Name", 0);
                }
                else if (!StartsWith(lName, "(") && !StartsWith(lName, "\""))
                {
                    var val = GetValueOrEmpty(lName);
                    if (val.Length > 0 && val != "_")
                        return val[0];

                    if (_cfgLearnUnknown && bLearn)
                        LearnSexOfGivnName(lName, '_');
                }
            }

            return result;
        }

        // Helpers

        private static IEnumerable<string> SplitBySpace(string text)
            => text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        private static bool IsAbbrev(string s)
            => s.Length == 2 && s.EndsWith(".", StringComparison.Ordinal) && char.IsUpper(s[0]);

        // Pascal TestFor(aText, 1-based pos, aTest)
        private static bool TestFor(string aText, int pPos1Based, string aTest)
        {
            int idx = Math.Max(0, pPos1Based - 1);
            if (idx < 0 || idx + aTest.Length > aText.Length) return false;
            return string.CompareOrdinal(aText, idx, aTest, 0, aTest.Length) == 0;
        }

        private static bool StartsWith(string s, string prefix)
            => s.Length > 0 && s[0].ToString() == prefix;

        private string GetValueOrEmpty(string key)
            => _gNameList.TryGetValue(key, out var v) ? v ?? string.Empty : string.Empty;

        private void SafeSave(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return;

            var dir = Path.GetDirectoryName(Path.GetFullPath(filename));
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Schreibe in temp Datei und ersetze Ziel
            var temp = Path.GetTempFileName();
            try
            {
                using (var sw = new StreamWriter(temp, false, new UTF8Encoding(false)))
                {
                    foreach (var kv in _gNameList)
                    {
                        sw.Write(kv.Key);
                        sw.Write('=');
                        sw.WriteLine(kv.Value ?? string.Empty);
                    }
                }

                if (File.Exists(filename))
                    File.Delete(filename);

                File.Move(temp, filename);
            }
            catch
            {
                try { if (File.Exists(temp)) File.Delete(temp); } catch { /* ignore */ }
                throw;
            }
        }
    }
}