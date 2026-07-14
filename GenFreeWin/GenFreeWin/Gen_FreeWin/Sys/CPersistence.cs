using BaseLib.Helper;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GenFree.Sys
{
    public class CPersistence : IGenPersistence
    {
        /// <summary>
        /// Gets the temporary path.
        /// </summary>
        /// <value>The temporary path.</value>
        public string TempPath => _Modul1.Instance.TempPath;
        /// <summary>
        /// Gets the path of the mandate.
        /// </summary>
        /// <value>The path of the mandate.</value>
        public string MandDir => _Modul1.Instance.Verz;
        /// <summary>
        /// Gets the path of th instance.
        /// </summary>
        /// <value>The path of th instance.</value>
        public string GenFreeDir => _Modul1.Instance.GenFreeDir;
        /// <summary>
        /// Gets the initialization path.
        /// </summary>
        /// <value>The initialization path with default-values.</value>
        public string InitDir => _Modul1.Instance.InitDir;
        /// <summary>
        /// Gets the output path.
        /// </summary>
        /// <value>The output path.</value>
        public string OutputDir => _Modul1.Instance.Verz1 + "INIT\\Ausgaben\\";


        //modernisierte Version von WriteInt: Verzicht auf FileSystem, stattdessen File.WriteAllText
        void WriteInt(string sArea, string sSection, int iValue)
        {
            var filePath = Path.Combine(sArea, sSection);
            File.WriteAllText(filePath, iValue.ToString());
        }

        //modernisierte Version von WriteInt: Verzicht auf FileSystem, stattdessen File.WriteAllText
        void WriteEnum(string sArea, string sSection, Enum eValue)
        {
            var filePath = Path.Combine(sArea, sSection);
            File.WriteAllText(filePath, eValue.AsInt().ToString());
        }

        private int GetInt(string sArea, string sSection, long lPos)
        {
            var filePath = Path.Combine(sArea, sSection);
            if (!File.Exists(filePath))
                return 0;
            int value = 0;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var br = new BinaryReader(fs))
            {
                long pos = (lPos - 1) * sizeof(int);
                if (fs.Length < pos + sizeof(int))
                    return 0;
                fs.Seek(pos, SeekOrigin.Begin);
                value = br.ReadInt32();
            }
            return value;
        }

        private int ReadInt(string sArea, string sSection)
        {
            var filePath = Path.Combine(sArea, sSection);
            if (!File.Exists(filePath))
                return 0;
            var content = File.ReadAllText(filePath).Trim();
            if (int.TryParse(content, out int value))
                return value;
            return 0;
        }

        private IList<int> ReadInts(string sArea, string sSection)
        {
            var filePath = Path.Combine(sArea, sSection);
            var values = new List<int>();
            if (!File.Exists(filePath))
                return values;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (int.TryParse(line.Trim(), out int val))
                    values.Add(val);
            }
            return values;
        }
        private int[] ReadInts(string sArea, string sSection, int iCnt)
        {
            var filePath = Path.Combine(sArea, sSection);
            var values = new List<int>();
            if (!File.Exists(filePath))
                return values.ToArray();

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (iCnt-- > 0 && int.TryParse(line.Trim(), out int val))
                    values.Add(val);
            }
            return values.ToArray();
        }
        private int ReadStrings(string sArea, string sSection, IList<string> asValue, bool xReplace = false)
        {
            var filePath = Path.Combine(sArea, sSection);
            if (!File.Exists(filePath))
                return -1;

            var lines = File.ReadAllLines(filePath);
            if (asValue is string[] asS)
                for (var i = 0; i < asS.Length && i < lines.Length; i++)
                    asS[i] = lines[i];
            else if (xReplace)
                for (var i = 0; i < lines.Length; i++)
                    if (i + 1 < asValue.Count)
                        asValue[i + 1] = lines[i];
                    else
                        asValue.Add(lines[i]);
            else
                foreach (var line in lines)
                    asValue.Add(line);
            return lines.Count();
        }
        private int ReadStrings(string sArea, string sSection, string[] asValue)
        {
            var filePath = Path.Combine(sArea, sSection);
            if (!File.Exists(filePath))
                return -1;

            var lines = File.ReadAllLines(filePath);
            for (var i = 0; i < asValue.Length && i < lines.Length; i++)
                asValue[i] = lines[i];
            return lines.Count();
        }
        private string ReadString(string asArea, string sSection)
        {
            var filePath = Path.Combine(asArea, sSection);
            if (!File.Exists(filePath))
                return "";
            return File.ReadAllText(filePath);
        }

        private void WriteInts(string sArea, string sSection, int[] aiValues)
        {
            var filePath = Path.Combine(sArea, sSection);
            File.WriteAllText(filePath, string.Join(Environment.NewLine, aiValues.Select(i => i.AsString())));
        }
        private void WriteString(string sArea, string sSection, string verz)
        {
            FileSystem.FileClose(99);
            var filePath = Path.Combine(sArea, sSection);
            File.WriteAllText(filePath, verz);
        }

        #region Init-Methods
        public void WriteIntInit(string sSection, int iValue) => WriteInt(InitDir, sSection, iValue);

        public void WriteEnumInit(string sSection, Enum eValue) => WriteEnum(InitDir, sSection, eValue);
        public int GetIntInit(string sSection, long lPos)
        {
            int iValue = default;
            FileSystem.FileOpen(99, InitDir + sSection, OpenMode.Random, OpenAccess.Default, OpenShare.Default);
            FileSystem.FileGet(99, ref iValue, lPos);
            FileSystem.FileClose(99);
            return iValue;
        }

        public int ReadIntInit(string sSection) => ReadInt(InitDir, sSection);

        public IList<int> ReadIntsInit(string sSection) => ReadInts(InitDir, sSection);

        public int[] ReadIntsInit(string sSection, int iCnt)
        {

            if (iCnt == -1)
                return ReadIntsInit(sSection).ToArray();
            var Value = new int[iCnt];
            Value.Initialize();
            FileSystem.FileClose(99); //??
            FileSystem.FileOpen(99, InitDir + sSection, OpenMode.Input);
            int i = 0;
            while (!FileSystem.EOF(99) && i < iCnt)
                FileSystem.Input(99, ref Value[i++]);
            FileSystem.FileClose(99);
            return Value;
        }

        public Color[] ReadFarbenInit(string name, int iCnt)
        {
            string FileName = Path.Combine(InitDir, name);
            var result = new Color[iCnt + 1];
            if (_Modul1.Instance.cMandDrive.DriveType != DriveType.CDRom)
            {
                FileSystem.FileClose(99);
                FileSystem.FileOpen(99, FileName, OpenMode.Append);
                FileSystem.FileClose(99);
                FileSystem.FileOpen(99, FileName, OpenMode.Random);

                for (int i = 1; i <= iCnt; i++)
                {
                    result[i] = GetSetColor(99, i);
                }

            }
            FileSystem.FileClose(99);
            return result;

            static Color GetSetColor(int iFile, long lPos)
            {
                Color cFarb = Color.Black;
                int farb = 0;
                FileSystem.FileGet(iFile, ref farb, lPos);
                Color farb1 = ColorTranslator.FromOle(farb);
                return farb1;
            }
        }
        public int ReadStringsInit(string sSection, IList<string> asValue, bool xReplace = false)
            => ReadStrings(InitDir, sSection, asValue, xReplace);

        public string ReadStringInit(string sSection) => ReadString(InitDir, sSection);

        public void ReadStringsInit(string sSetion, string[] aus) => ReadStrings(InitDir, sSetion, aus);

        public void WriteStringInit(string sSection, string verz) => WriteString(InitDir, sSection, verz);

        public void WriteStringsInit(string sSection, string[] asData)
        {
            FileSystem.FileOpen(99, InitDir + "\\" + sSection, OpenMode.Output);
            foreach (var verz in asData)
                FileSystem.PrintLine(99, verz);
            FileSystem.FileClose();
        }

        public void ReadBoolsInit(string sSection, IList<bool> axOption)
        {
            int num4;
            FileSystem.FileOpen(99, InitDir + sSection, OpenMode.Input);
            num4 = 0;
            var Value = "";
            while (!FileSystem.EOF(99))
            {
                num4++;
                FileSystem.Input(99, ref Value);
                axOption[num4] = Value.AsBool();
            }
            FileSystem.FileClose(99);
        }
        public void WriteBoolsInit(string sSection, IList<bool> axOption)
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, InitDir + sSection, OpenMode.Output);
            var num4 = 1;
            while (num4 <= 50)
            {
                FileSystem.PrintLine(99, axOption[num4]);
                num4++;
            }

            FileSystem.FileClose(99);
        }
        public void ReadStringsInit(string sSection, int anz, IList<string> txT)
        {
            FileSystem.FileOpen(99, InitDir + sSection, OpenMode.Input);
            var M1_Iter = 0;
            while (!FileSystem.EOF(99) && ++M1_Iter < anz)
            {
                txT[M1_Iter] = Strings.Trim(FileSystem.LineInput(99));
            }
            FileSystem.FileClose(99);
        }
        public void WriteStringsInit(string sSection, IList<string> asData)
        {
            FileSystem.FileOpen(99, InitDir + sSection, OpenMode.Output);
            foreach (var verz in asData)
                FileSystem.PrintLine(99, verz);
            FileSystem.FileClose(99);
        }
        public bool ExistFileInit(string v)
        {
            var fileName = Path.Combine(InitDir, v);
            if (File.Exists(fileName))
                return true;
            return false;
        }
        public void PutColorsInit(string sSection, IList<Color> value)
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, InitDir + sSection, OpenMode.Random);
            for (int i = 0; i < value.Count; i++)
            {
                FileSystem.FilePut(99, value[i], i + 1);
            }
            FileSystem.FileClose(99);
        }
        public void PutColorInit(string sSection, Color value, int iCnt)
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, InitDir + sSection, OpenMode.Random);
            FileSystem.FilePut(99, value, iCnt);
            FileSystem.FileClose(99);
        }
        #endregion

        #region Prog-Methods
        public void WriteIntsProg(string sSection, int[] aiValues) => WriteInts(GenFreeDir, sSection, aiValues);


        public void WriteStringProg(string sSection, string text) => WriteString(GenFreeDir, sSection, text);
        public void ReadStringsProg(string sSection, string[] asData) => ReadStrings(GenFreeDir, sSection, asData);

        public void ReadStringsProg(string sSection, IList<string> asData, int iCount) => ReadStrings(GenFreeDir, sSection, asData);

        public IEnumerable<string> ReadStringsProg(string sSection)
        {
            var full = File.ReadAllText(Path.Combine(GenFreeDir, sSection)).Split([Environment.NewLine], StringSplitOptions.None);
            foreach (string s in full)
            {
                yield return s;
            }
        }

        public void WriteStringsProg(string sSection, string[] asData)
        {
            FileSystem.FileOpen(99, Path.Combine(GenFreeDir, sSection), OpenMode.Output);
            foreach (var verz in asData)
                FileSystem.PrintLine(99, verz);
            FileSystem.FileClose(99);
        }
        public string ReadStringProg(string sSection)
        {
            var Value = "";
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, GenFreeDir + "\\" + sSection, OpenMode.Input);
            FileSystem.Input(99, ref Value);
            FileSystem.FileClose(99);
            return Value;
        }
        #endregion

        #region Mandant-Methods
        public void WriteEnumMand(string sSection, Enum eValue) => WriteEnum(MandDir, sSection, eValue);

        public void WriteIntMand(string sFilename, int iVal) => WriteInt(MandDir, sFilename, iVal);

        public int ReadIntMand(string sFilename) => ReadInt(MandDir, sFilename);

        public void PutIntsMand(string sSection, int[] aiValues)
        {
            FileSystem.FileOpen(99, MandDir + "\\" + sSection, OpenMode.Append);
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, MandDir + "\\" + sSection, OpenMode.Random);
            foreach (var i in aiValues)
                FileSystem.FilePut(99, i);
            FileSystem.FileClose(99);
        }
        public void PutIntMand(string sSection, ValueType letzte, long lPos)
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, MandDir + sSection, OpenMode.Random, OpenAccess.Default, OpenShare.Default);
            FileSystem.FilePut(99, letzte, lPos);
            FileSystem.FileClose(99);
        }
        public void PutEnumsMand<T>(string sSection, T[] enums) where T : Enum
        {
            FileSystem.FileOpen(99, MandDir + "\\" + sSection, OpenMode.Random);
            for (long i = 1L; i <= enums.Length; i++)
            {
                FileSystem.FilePut(99, Convert.ToInt32(enums[i - 1]), i);
            }
            FileSystem.FileClose(99);
        }

        public void GetEnumsMand<T>(string sSection, T[] enums) where T : Enum
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, MandDir + "\\" + sSection, OpenMode.Random);
            for (long i = 1L; i <= enums.Length; i++)
            {
                int iTmp = 0;
                FileSystem.FileGet(99, ref iTmp, i);
                enums[i - 1] = (T)Enum.ToObject(typeof(T), iTmp);
            }
            FileSystem.FileClose(99);
        }
        public void WriteStringMand(string sSection, string sValue)
        {
            FileSystem.FileOpen(99, MandDir + "\\" + sSection, OpenMode.Output);
            FileSystem.PrintLine(99, sValue);
            FileSystem.FileClose();
        }
        public int GetIntMand(string sSection, long lPos = 1L)
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, MandDir + sSection, OpenMode.Random, OpenAccess.Default, OpenShare.Default, sizeof(int));
            int Value = 0;
            FileSystem.FileGet(99, ref Value, lPos);
            FileSystem.FileClose();
            return Value;
        }


        #endregion

        #region Temp-Methods
        public void WriteStringTemp(string sSection, string text)
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, TempPath + "\\" + sSection, OpenMode.Output);
            FileSystem.PrintLine(99, text);
            FileSystem.FileClose(99);
        }

        public void AppendStringsTemp(string sSection, IList<string> lines)
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, TempPath + "\\" + sSection, OpenMode.Append);
            foreach (var line in lines)
                FileSystem.PrintLine(99, line);
            FileSystem.FileClose(99);
        }

        private void ReadBoolsTemp(string sSection, IList<bool> axOption)
        {
            FileSystem.FileClose(6);
            FileSystem.FileOpen(6, TempPath + "\\" + sSection, OpenMode.Input);
            var num4 = 0;
            var Value = "";
            while (!FileSystem.EOF(6))
            {
                FileSystem.Input(6, ref Value);
                axOption[(short)num4 + 10] = Value.AsBool();
                num4++;
            }
            FileSystem.FileClose(99);
        }

        public void CreateTempFile(string v)
        {
            FileSystem.FileOpen(99, Path.Combine(TempPath, v), OpenMode.Output);
            FileSystem.FileClose(99);
        }
        #endregion
        public void WriteBoolsTemp(string sSection, IList<bool> Option)
        {
            FileSystem.FileClose(6);
            FileSystem.Kill(TempPath + "\\" + sSection);
            FileSystem.FileOpen(6, TempPath + "\\" + sSection, OpenMode.Output);
            int num4 = 0;
            while (num4 <= 4)
            {
                FileSystem.PrintLine(6, Option[(short)num4 + 10]);
                num4++;
            }
            FileSystem.FileClose(6);
        }



        public string ReadStringMLProg(string sSection, int iMaxLine)
        {
            var Value2 = "";
            // Pre-FileHandling
            try
            {
                FileSystem.FileClose(99);
                FileSystem.FileOpen(99, Path.Combine(GenFreeDir, sSection), OpenMode.Append);
                FileSystem.FileClose(99);
            }
            catch { }
            if (!File.Exists(Path.Combine(GenFreeDir, sSection)))
                return "";
            // File-Reading    
            FileSystem.FileOpen(99, Path.Combine(GenFreeDir, sSection), OpenMode.Input);
            var M1_Iter = 0;
            while (!FileSystem.EOF(99) && (++M1_Iter < iMaxLine))
            {
                Value2 += FileSystem.LineInput(99) + Environment.NewLine;
            }
            FileSystem.FileClose(99);
            return Value2;
        }
        public int[] ReadIntsProg(string sSection, int iCnt) => ReadInts(GenFreeDir, sSection, iCnt);

        public void ReadSuchDatMand<T>(string DateiName, IList<T> aeValues) where T : Enum
        {
            string DateiName2;
            if (!File.Exists(DateiName2 = Path.Combine(MandDir, DateiName)))
            {
                PutIntsMand(DateiName, new[] { 1, 1, 1 });
            }
            GetEnumsMand(DateiName, aeValues);
        }




        public void CopyDirectory(string v, string backupDir)
        {
            if (!Directory.Exists(v))
                return;
            if (!Directory.Exists(backupDir))
                Directory.CreateDirectory(backupDir);
            var di = new DirectoryInfo(v);
            foreach (var fi in di.GetFiles())
            {
                var destFile = Path.Combine(backupDir, fi.Name);
                File.Copy(fi.FullName, destFile, true);
            }
            foreach (var subDir in di.GetDirectories())
            {
                CopyDirectory(subDir.FullName, Path.Combine(backupDir, subDir.Name));
            }
        }








        public void GetEnumsMand<T>(string sSection, IList<T> enums) where T : Enum
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, MandDir + "\\" + sSection, OpenMode.Random, OpenAccess.Default, OpenShare.Default, sizeof(int));
            for (long i = 1L; i <= enums.Count; i++)
            {
                int iTmp = 0;
                FileSystem.FileGet(99, ref iTmp, i);
                enums[(int)i - 1] = (T)Enum.ToObject(typeof(T), iTmp);
            }
            FileSystem.FileClose(99);
        }

        public void PutEnumsMand<T>(string sSection, IList<T> enums) where T : Enum
        {
            FileSystem.FileOpen(99, MandDir + "\\" + sSection, OpenMode.Random);
            for (long i = 1L; i <= enums.Count; i++)
            {
                FileSystem.FilePut(99, Convert.ToInt32(enums[(int)i - 1]), i);
            }
            FileSystem.FileClose(99);
        }

        public int[] ReadIntsMand(string sSection, int cCnt)
        {

            var result = new int[cCnt];
            if (!File.Exists(Path.Combine(GenFreeDir, sSection)))
                return result;
            FileSystem.FileOpen(99, Path.Combine(GenFreeDir, sSection), OpenMode.Input);
            for (int i = 0; i < cCnt; i++)
            {
                FileSystem.Input(99, ref result[i]);
            }
            FileSystem.FileClose(99);
            return result;
        }
        public bool ReadBoolInit(string v)
        {
            var Value = false;
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, InitDir + v, OpenMode.Input);
            FileSystem.Input(99, ref Value);
            FileSystem.FileClose(99);
            return Value;
        }

        void IGenPersistence.ReadBoolsTemp(string v, IList<bool> Option) => ReadBoolsTemp(v, Option);

        public void ReadStringsOutput(string sSection, IList<string> asValue, int iCount)
        {
            if (!Directory.Exists(OutputDir))
                Directory.CreateDirectory(OutputDir);
            if (!File.Exists(Path.Combine(OutputDir, sSection)))
                return;
            FileSystem.FileOpen(99, Path.Combine(OutputDir, sSection), OpenMode.Input);
            var M1_Iter = 0;
            while (!FileSystem.EOF(99) && ++M1_Iter < iCount)
            {
                asValue[M1_Iter] = Strings.Trim(FileSystem.LineInput(99).Replace(":", ""));
            }
            FileSystem.FileClose(99);
        }

        public void WriteStringsOutput(string sSection, IList<string> asValue, int iCount)
        {
            if (!Directory.Exists(OutputDir))
                Directory.CreateDirectory(OutputDir);
            FileSystem.FileOpen(99, Path.Combine(OutputDir, sSection), OpenMode.Output);
            foreach (var verz in asValue)
                FileSystem.PrintLine(99, verz);
            FileSystem.FileClose(99);
        }

        public void DeleteTempFile(string v)
        {
            var filePath = Path.Combine(TempPath, v);
            if (File.Exists(filePath))
            {
                FileSystem.Kill(filePath);
            }
        }

        public bool ExistFileTemp(string v)
            => File.Exists(Path.Combine(TempPath, v));

        public int FileLengthTemp(string v)
        {
            var filePath = Path.Combine(TempPath, v);
            if (File.Exists(filePath))
            {
                return (int)new FileInfo(filePath).Length;
            }
            else
            {
                return 0;
            }
        }

        public void WriteStringsTemp(string sSection, IList<string> asValue)
        {
            FileSystem.FileOpen(99, Path.Combine(TempPath, sSection), OpenMode.Output);
            foreach (var verz in asValue)
                FileSystem.PrintLine(99, verz);
            FileSystem.FileClose(99);
        }

        public IList<string> ReadStringsMand(string sSection, int v2)
        {
            var result = new List<string>(v2);
            FileSystem.FileOpen(99, Path.Combine(MandDir, sSection), OpenMode.Input);
            for (int i = 0; i < v2; i++)
            {
                if (FileSystem.EOF(99))
                    break;
                string line = FileSystem.LineInput(99).Trim();
                result.Add(line);
            }
            FileSystem.FileClose(99);
            return result;
        }

        public void ReadStringsTemp(string v, IList<string> asOption)
        {
            FileSystem.FileOpen(99, Path.Combine(TempPath, v), OpenMode.Input);
            var M1_Iter = 0;
            while (!FileSystem.EOF(99) && ++M1_Iter < asOption.Count)
            {
                asOption[M1_Iter] = Strings.Trim(FileSystem.LineInput(99).Replace(":", ""));
            }
            FileSystem.FileClose(99);
        }

        public string CreateTempFilefromInit(string filename)
        {
            string path = Path.Combine(TempPath, filename);
            FileSystem.Kill(path);
            FileSystem.FileCopy(Path.Combine(InitDir, filename), path);
            return path;
        }

        public bool ExistFile(string v)
            => File.Exists((string?)v);

        public bool ExistFileMand(string dateiName)
            => File.Exists(Path.Combine(MandDir, dateiName));

        public void WriteStringsMand(string sSection, IList<string> items)
        {
            FileSystem.FileOpen(99, Path.Combine(MandDir, sSection), OpenMode.Output);
            foreach (var item in items)
            {
                FileSystem.PrintLine(99, item);
            }
            FileSystem.FileClose(99);
        }

        public bool ExistFileProg(string v)
            => File.Exists(Path.Combine(GenFreeDir, v));
    }
}
