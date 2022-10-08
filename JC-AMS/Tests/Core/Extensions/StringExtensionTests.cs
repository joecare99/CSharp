using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Drawing;
using JCAMS.Core.Tests;
using System.IO;

namespace JCAMS.Core.Extensions.Tests
{
    [TestClass()]
    public class StringExtensionTests
    {
        protected static IEnumerable<object[]> AsATML1Data => TestData.AsATML1Data;

        protected static IEnumerable<object[]> StreamDumpData => new[]
        {
            new object[]{"Null",null,"" },
            new object[]{"Empty",StringExtension.String2Stream(""),"" },
            new object[]{"Hello World",StringExtension.String2Stream("Hello World"),"0000: 48 65 6C 6C 6F 20 57 6F : 72 6C 64                 | Hello World\r\n" },
            new object[]{"Lorem Ipsum...",StringExtension.String2Stream(TestData.cLoremIpsum),@"0000: 4C 6F 72 65 6D 20 69 70 : 73 75 6D 20 64 6F 6C 6F  | Lorem ipsum dolo
0010: 72 20 73 69 74 20 61 6D : 65 74 2C 20 63 6F 6E 73  | r sit amet, cons
0020: 65 63 74 65 74 75 72 20 : 61 64 69 70 69 73 69 63  | ectetur adipisic
0030: 69 20 65 6C 69 74 2C 20 : 73 65 64 20 65 69 75 73  | i elit, sed eius
0040: 6D 6F 64 20 74 65 6D 70 : 6F 72 20 0D 0A 69 6E 63  | mod tempor ..inc
0050: 69 64 75 6E 74 20 75 74 : 20 6C 61 62 6F 72 65 20  | idunt ut labore 
0060: 65 74 20 64 6F 6C 6F 72 : 65 20 6D 61 67 6E 61 20  | et dolore magna 
0070: 61 6C 69 71 75 61 2E 20 : 55 74 20 65 6E 69 6D 20  | aliqua. Ut enim 
0080: 61 64 20 6D 69 6E 69 6D : 20 76 65 6E 69 61 6D 2C  | ad minim veniam,
0090: 20 71 75 69 73 20 0D 0A : 6E 6F 73 74 72 75 64 20  |  quis ..nostrud 
00A0: 65 78 65 72 63 69 74 61 : 74 69 6F 6E 20 75 6C 6C  | exercitation ull
00B0: 61 6D 63 6F 20 6C 61 62 : 6F 72 69 73 20 6E 69 73  | amco laboris nis
00C0: 69 20 75 74 20 61 6C 69 : 71 75 69 64 20 65 78 20  | i ut aliquid ex 
00D0: 65 61 20 63 6F 6D 6D 6F : 64 69 20 63 6F 6E 73 65  | ea commodi conse
00E0: 71 75 61 74 2E 20 0D 0A : 51 75 69 73 20 61 75 74  | quat. ..Quis aut
00F0: 65 20 69 75 72 65 20 72 : 65 70 72 65 68 65 6E 64  | e iure reprehend
0100: 65 72 69 74 20 69 6E 20 : 76 6F 6C 75 70 74 61 74  | erit in voluptat
0110: 65 20 76 65 6C 69 74 20 : 65 73 73 65 20 63 69 6C  | e velit esse cil
0120: 6C 75 6D 20 64 6F 6C 6F : 72 65 20 65 75 20 66 75  | lum dolore eu fu
0130: 67 69 61 74 20 0D 0A 6E : 75 6C 6C 61 20 70 61 72  | giat ..nulla par
0140: 69 61 74 75 72 2E 20 45 : 78 63 65 70 74 65 75 72  | iatur. Excepteur
0150: 20 73 69 6E 74 20 6F 62 : 63 61 65 63 61 74 20 63  |  sint obcaecat c
0160: 75 70 69 64 69 74 61 74 : 20 6E 6F 6E 20 70 72 6F  | upiditat non pro
0170: 69 64 65 6E 74 2C 20 73 : 75 6E 74 20 69 6E 20 63  | ident, sunt in c
0180: 75 6C 70 61 20 0D 0A 71 : 75 69 20 6F 66 66 69 63  | ulpa ..qui offic
0190: 69 61 20 64 65 73 65 72 : 75 6E 74 20 6D 6F 6C 6C  | ia deserunt moll
01A0: 69 74 20 61 6E 69 6D 20 : 69 64 20 65 73 74 20 6C  | it anim id est l
01B0: 61 62 6F 72 75 6D 2E                               | aborum.
" },
            new object[]{"Lorem Ipsum...2",StringExtension.String2Stream(TestData.cBase64LoremIpsum,true),@"0000: 4C 6F 72 65 6D 20 69 70 : 73 75 6D 20 64 6F 6C 6F  | Lorem ipsum dolo
0010: 72 20 73 69 74 20 61 6D : 65 74 2C 20 63 6F 6E 73  | r sit amet, cons
0020: 65 63 74 65 74 75 72 20 : 61 64 69 70 69 73 69 63  | ectetur adipisic
0030: 69 20 65 6C 69 74 2C 20 : 73 65 64 20 65 69 75 73  | i elit, sed eius
0040: 6D 6F 64 20 74 65 6D 70 : 6F 72 20 0D 0A 69 6E 63  | mod tempor ..inc
0050: 69 64 75 6E 74 20 75 74 : 20 6C 61 62 6F 72 65 20  | idunt ut labore 
0060: 65 74 20 64 6F 6C 6F 72 : 65 20 6D 61 67 6E 61 20  | et dolore magna 
0070: 61 6C 69 71 75 61 2E 20 : 55 74 20 65 6E 69 6D 20  | aliqua. Ut enim 
0080: 61 64 20 6D 69 6E 69 6D : 20 76 65 6E 69 61 6D 2C  | ad minim veniam,
0090: 20 71 75 69 73 20 0D 0A : 6E 6F 73 74 72 75 64 20  |  quis ..nostrud 
00A0: 65 78 65 72 63 69 74 61 : 74 69 6F 6E 20 75 6C 6C  | exercitation ull
00B0: 61 6D 63 6F 20 6C 61 62 : 6F 72 69 73 20 6E 69 73  | amco laboris nis
00C0: 69 20 75 74 20 61 6C 69 : 71 75 69 64 20 65 78 20  | i ut aliquid ex 
00D0: 65 61 20 63 6F 6D 6D 6F : 64 69 20 63 6F 6E 73 65  | ea commodi conse
00E0: 71 75 61 74 2E 20 0D 0A : 51 75 69 73 20 61 75 74  | quat. ..Quis aut
00F0: 65 20 69 75 72 65 20 72 : 65 70 72 65 68 65 6E 64  | e iure reprehend
0100: 65 72 69 74 20 69 6E 20 : 76 6F 6C 75 70 74 61 74  | erit in voluptat
0110: 65 20 76 65 6C 69 74 20 : 65 73 73 65 20 63 69 6C  | e velit esse cil
0120: 6C 75 6D 20 64 6F 6C 6F : 72 65 20 65 75 20 66 75  | lum dolore eu fu
0130: 67 69 61 74 20 0D 0A 6E : 75 6C 6C 61 20 70 61 72  | giat ..nulla par
0140: 69 61 74 75 72 2E 20 45 : 78 63 65 70 74 65 75 72  | iatur. Excepteur
0150: 20 73 69 6E 74 20 6F 62 : 63 61 65 63 61 74 20 63  |  sint obcaecat c
0160: 75 70 69 64 69 74 61 74 : 20 6E 6F 6E 20 70 72 6F  | upiditat non pro
0170: 69 64 65 6E 74 2C 20 73 : 75 6E 74 20 69 6E 20 63  | ident, sunt in c
0180: 75 6C 70 61 20 0D 0A 71 : 75 69 20 6F 66 66 69 63  | ulpa ..qui offic
0190: 69 61 20 64 65 73 65 72 : 75 6E 74 20 6D 6F 6C 6C  | ia deserunt moll
01A0: 69 74 20 61 6E 69 6D 20 : 69 64 20 65 73 74 20 6C  | it anim id est l
01B0: 61 62 6F 72 75 6D 2E                               | aborum.
" },
            new object[]{"Binary Data",StringExtension.String2Stream(
                "tkSKmV0ot0HdTxKIAyBPxa0F3FNR7bRVvwY4iqDOt9nOXOKLM5GyIs5A1TlLh5" +
                "2MZ+OV4PczZt69mjoACanw1GCfEPkvWvOvlXTB1UmXcxV42YyHxujuk45391uf" +
                "LqgJrGeJCzw+M5JyYq4WXK4qcWp9PyGhwzr9waHUi3ZNQVj2otsnnDoJkGKxw1" +
                "yUE1z1m6p/KiWUGg73Un4iarI3cB/KWi4CpIsK+/TaUejzSHIV4l7xDxrL/Mt/" +
                "5/FYkzy+DMDjQEstjqooCkULknQzE/5vuan2VLTRRzXEsObecXdsXhkFZV1qao" +
                "QPtEgcVj6YrL0Fc7jvyF7eY81BvC8WEuyAP/nUqKuHeE1ly0da0pQSM4BDjMdZ" +
                "Ij8r8RxtlPZHWfcwMyzYCifMMFNT/BlLi0srvPAMMStVK37MqKDmvNmQurKmQV" +
                "zaJinu7OOOk2ayePy+0uVzfoGdxgGA8ISlPJHYm3dxoMxRv3Ldhlthip7sDJWa" +
                "YplBH5LsOsCOXCPR6+DV4ncc/uYgEyRpuJ8tX1CW/+e28xycp0UVPF7M0XqGxD" +
                "cytpki9ZUOLGb2J/Ypi9Ly+ZX+LYs=",true),
                @"0000: B6 44 8A 99 5D 28 B7 41 : DD 4F 12 88 03 20 4F C5  | .D..](.A.O... O.
0010: AD 05 DC 53 51 ED B4 55 : BF 06 38 8A A0 CE B7 D9  | ...SQ..U..8.....
0020: CE 5C E2 8B 33 91 B2 22 : CE 40 D5 39 4B 87 9D 8C  | .\..3.."".@.9K...
0030: 67 E3 95 E0 F7 33 66 DE : BD 9A 3A 00 09 A9 F0 D4  | g....3f...:.....
0040: 60 9F 10 F9 2F 5A F3 AF : 95 74 C1 D5 49 97 73 15  | `.../Z...t..I.s.
0050: 78 D9 8C 87 C6 E8 EE 93 : 8E 77 F7 5B 9F 2E A8 09  | x........w.[....
0060: AC 67 89 0B 3C 3E 33 92 : 72 62 AE 16 5C AE 2A 71  | .g..<>3.rb..\.*q
0070: 6A 7D 3F 21 A1 C3 3A FD : C1 A1 D4 8B 76 4D 41 58  | j}?!..:.....vMAX
0080: F6 A2 DB 27 9C 3A 09 90 : 62 B1 C3 5C 94 13 5C F5  | ...'.:..b..\..\.
0090: 9B AA 7F 2A 25 94 1A 0E : F7 52 7E 22 6A B2 37 70  | ..*%....R~""j.7p
00A0: 1F CA 5A 2E 02 A4 8B 0A : FB F4 DA 51 E8 F3 48 72  | ..Z........Q..Hr
00B0: 15 E2 5E F1 0F 1A CB FC : CB 7F E7 F1 58 93 3C BE  | ..^........X.<.
00C0: 0C C0 E3 40 4B 2D 8E AA : 28 0A 45 0B 92 74 33 13  | ...@K-..(.E..t3.
00D0: FE 6F B9 A9 F6 54 B4 D1 : 47 35 C4 B0 E6 DE 71 77  | .o...T..G5....qw
00E0: 6C 5E 19 05 65 5D 6A 6A : 84 0F B4 48 1C 56 3E 98  | l^..e]jj...H.V>.
00F0: AC BD 05 73 B8 EF C8 5E : DE 63 CD 41 BC 2F 16 12  | ...s...^.c.A./..
0100: EC 80 3F F9 D4 A8 AB 87 : 78 4D 65 CB 47 5A D2 94  | ..?.....xMe.GZ..
0110: 12 33 80 43 8C C7 59 22 : 3F 2B F1 1C 6D 94 F6 47  | .3.C..Y""?+..m..G
0120: 59 F7 30 33 2C D8 0A 27 : CC 30 53 53 FC 19 4B 8B  | Y.03,..'.0SS..K.
0130: 4B 2B BC F0 0C 31 2B 55 : 2B 7E CC A8 A0 E6 BC D9  | K+...1+U+~......
0140: 90 BA B2 A6 41 5C DA 26 : 29 EE EC E3 8E 93 66 B2  | ....A\.&).....f.
0150: 78 FC BE D2 E5 73 7E 81 : 9D C6 01 80 F0 84 A5 3C  | x....s~........<
0160: 91 D8 9B 77 71 A0 CC 51 : BF 72 DD 86 5B 61 8A 9E  | ...wq..Q.r..[a..
0170: EC 0C 95 9A 62 99 41 1F : 92 EC 3A C0 8E 5C 23 D1  | ....b.A...:..\#.
0180: EB E0 D5 E2 77 1C FE E6 : 20 13 24 69 B8 9F 2D 5F  | ....w... .$i..-_
0190: 50 96 FF E7 B6 F3 1C 9C : A7 45 15 3C 5E CC D1 7A  | P........E.<^..z
01A0: 86 C4 37 32 B6 99 22 F5 : 95 0E 2C 66 F6 27 F6 29  | ..72..""...,f.'.)
01B0: 8B D2 F2 F9 95 FE 2D 8B :                          | ......-.
" },
        };


        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Empty", "", false)]
        [DataRow("'Hello'", "Hello", false)]
        [DataRow("\\\\server1\\awd", "\\\\server1\\awd", true)]
        [DataRow("\\\\\\server1\\awd", "\\\\\\server1\\awd", true)] // ?
        public void IsUNCPathTest(string name, string s, bool xExp)
        {
            Assert.AreEqual(xExp, s.IsUNCPath(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Empty", "", false)]
        [DataRow("'Hello'", "Hello", false)]
        [DataRow("ftp://server1/awd", "ftp://server1/awd", true)]
        [DataRow("ftp://!server1/awd", "ftp://!server1/awd", true)] // ?
        public void IsFTPPathTest(string name, string s, bool xExp)
        {
            Assert.AreEqual(xExp, s.IsFTPPath(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Empty", "", false)]
        [DataRow("'Hello'", "Hello", false)]
        [DataRow("ftp://server1/awd", "ftp://server1/awd", true)]
        [DataRow("ftp://!server1/awd", "ftp://!server1/awd", true)] // ?
        public void IsDrivePathTest(string name, string s, bool xExp)
        {
            Assert.AreEqual(xExp, s.IsFTPPath(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Empty", "", false)]
        [DataRow("'Hello'", "Hello", false)]
        [DataRow("25.2.1983", "25.2.1983", true)]
        [DataRow("29.02.1900", "29.02.1900", false)]
        [DataRow("2022-01-01", "2022-01-01", true)]
        [DataRow("09/11/2001", "09/11/2001", true)]
        public void IsDateTimeTest(string name, string s, bool xExp)
        {
            Assert.AreEqual(xExp, s.IsDateTime(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Empty", "", false)]
        [DataRow("'Hello'", "Hello", false)]
        [DataRow("1", "1", true)]
        [DataRow(".1", ".1", true)]
        [DataRow("1e2", "1e2", true)]
        [DataRow("1.7e309", "1.7e309", true)] //?
        [DataRow("1.4e-309", "1.4e-309", true)] //?
        [DataRow("NaN", "NaN", false)] //?
        public void IsDoubleTest(string name, string s, bool xExp)
        {
            Assert.AreEqual(xExp, s.IsDouble(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("Empty", "", "")]
        [DataRow("'Hello'", "Hello", "Hello")]
        [DataRow("ftp://server1/awd", "ftp://server1/awd", "ftp://server1/awd")]
        [DataRow("1&2", "1&2", "1&amp;2")]
        [DataRow("1<2", "1<2", "1&lt;2")]
        [DataRow("1>2", "1>2", "1&gt;2")]
        [DataRow("Fuß", "Fuß", "Fu&szlig;")]
        [DataRow("Öl", "Öl", "&Ouml;l")]
        [DataRow("Ändern", "Ändern", "&Auml;ndern")]
        [DataRow("Über", "Über", "&Uuml;ber")]
        [DataRow("größe", "größe", "gr&ouml;&szlig;e")]
        [DataRow("hätte", "hätte", "h&auml;tte")]
        [DataRow("für", "für", "f&uuml;r")]
        public void AsHTMLTest(string name, string s, string sExp)
        {
            Assert.AreEqual(sExp, s.AsHTML(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData("AsATML1Data")]
        public void AsHTMLTest1(string name, Color c, string sExp)
        {
            Assert.AreEqual(sExp, c.AsHTML(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow(null, 0)]
        [DataRow("", 0)]
        [DataRow("1", 1)]
        [DataRow("-1", -1)]
        [DataRow("--2", -2)] // ??
        [DataRow("-Hallo-3", -3)] // ??
        [DataRow("-Hallo 4", 4)] // ??
        [DataRow("Hallo", 0)]
        [DataRow("Hallo-World", 0)]
        [DataRow("Hallo-1", -1)]
        [DataRow("Hallo 191238831290", 191238831290)]
        [DataRow("Hallo 1912-3883-1290", 1912)]
        [DataRow("Hallo 1912,3883,1290", 1912)]
        [DataRow("Ha11o", 11)]
        public void GetFirstNumberTest(string sVal, long lExp)
        {
            Assert.AreEqual(lExp, StringExtension.GetFirstNumber(sVal), $"Test: GetFirstNumber({sVal})");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow(null, 0)]
        [DataRow("", 0)]
        [DataRow("1", 1)]
        [DataRow("-1", -1)]
        [DataRow("--2", -2)] // ??
        [DataRow("-Hallo-3", -3)] // ??
        [DataRow("-Hallo 4", 4)] // ??
        [DataRow("Hallo", 0)]
        [DataRow("Hallo-World", 0)]
        [DataRow("Hallo-1", -1)]
        [DataRow("Hallo 191238831290", 191238831290)]
        [DataRow("Hallo 1912-3883-1290", 1912)]
        [DataRow("Hallo 1912,3883,1290", 1912)]
        [DataRow("Ha11o", 11)]
        public void GetFirstNumberTest1(string sVal, long lExp)
        {
            Assert.AreEqual(lExp, sVal.GetFirstNumber(), $"Test: GetFirstNumber({sVal})");
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow(null, '\0', 0)]
        [DataRow("", '\0', 0)]
        [DataRow("1", '1', 1)]
        [DataRow("-1", '-', 1)]
        [DataRow("--2", '-', 2)] // ??
        [DataRow("-Hallo-3", '-', 2)] // ??
        [DataRow("-Hallo 4", '-', 1)] // ??
        [DataRow("Hallo", '-', 0)]
        [DataRow("Hallo-World", '-', 1)]
        [DataRow("Hallo-1", '-', 1)]
        [DataRow("Hallo 191238831290", '-', 0)]
        [DataRow("Hallo 1912-3883-1290", '-', 2)]
        [DataRow("Hallo 1912,3883,1290", '1', 3)]
        [DataRow("Ha11o", '1', 2)]
        [DataRow(TestData.cLoremIpsum, ' ', 65)]
        [DataRow(TestData.cLoremIpsum, '\r', 5)]
        [DataRow(TestData.cLoremIpsum, 'l', 20)]
        public void CharCntTest(string sVal, char cCh, long lExp)
        {
            Assert.AreEqual(lExp, StringExtension.CharCnt(sVal, cCh), $"Test: CharCnt({sVal},{cCh})");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow(null, '\0', 0)]
        [DataRow("", '\0', 0)]
        [DataRow("1", '1', 1)]
        [DataRow("-1", '-', 1)]
        [DataRow("--2", '-', 2)] // ??
        [DataRow("-Hallo-3", '-', 2)] // ??
        [DataRow("-Hallo 4", '-', 1)] // ??
        [DataRow("Hallo", '-', 0)]
        [DataRow("Hallo-World", '-', 1)]
        [DataRow("Hallo-1", '-', 1)]
        [DataRow("Hallo 191238831290", '-', 0)]
        [DataRow("Hallo 1912-3883-1290", '-', 2)]
        [DataRow("Hallo 1912,3883,1290", '1', 3)]
        [DataRow("Ha11o", '1', 2)]
        [DataRow(TestData.cLoremIpsum, ' ', 65)]
        [DataRow(TestData.cLoremIpsum, '\r', 5)]
        [DataRow(TestData.cLoremIpsum, 'l', 20)]
        public void CharCntTest2(string sVal, char cCh, long lExp)
        {
            Assert.AreEqual(lExp, sVal.CharCnt(cCh), $"Test: CharCnt({sVal},{cCh})");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData("StreamDumpData")]
        public void DumpTest(string name, Stream sVal, string sExp)
        {
            var oPos = sVal?.Position;
            Assert.AreEqual(sExp, sVal.Dump(), $"Test: {name}");
            Assert.AreEqual(oPos, sVal?.Position, $"Test: {name}.Position");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData("StreamDumpData")]
        public void DumpTest2(string name, Stream sVal, string sExp)
        {
            var oPos = sVal?.Position;
            Assert.AreEqual(sExp, StringExtension.Dump(sVal), $"Test: Dump({name})");
            Assert.AreEqual(oPos, sVal?.Position, $"Test: {name}.Position");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow(null, false, "")]
        [DataRow("", false, "")]
        [DataRow("1", false, "1")]
        [DataRow("-1", false, "-1")]
        [DataRow("--2", false, "--2")] // ??
        [DataRow("-Hallo-3", false, "-Hallo-3")] // ??
        [DataRow("-Hallo 4", false, "-Hallo 4")] // ??
        [DataRow("Hallo", false, "Hallo")]
        [DataRow("Hallo-World", false, "Hallo-World")]
        [DataRow("Hallo-1", false, "Hallo-1")]
        [DataRow("Hallo 191238831290", false, "Hallo 191238831290")]
        [DataRow("Hallo 1912-3883-1290", false, "Hallo 1912-3883-1290")]
        [DataRow("Hallo 1912,3883,1290", false, "Hallo 1912,3883,1290")]
        [DataRow("Ha11o", false, "Ha11o")]
        [DataRow(TestData.cBase64LoremIpsum, true, TestData.cLoremIpsum)]
       
        [DataRow(TestData.cLoremIpsum, false, TestData.cLoremIpsum)]
        public void String2StreamTest(string sVal, bool xVal, string sExp)
        {
            var ms = StringExtension.String2Stream(sVal, xVal);
            if (ms != null)
            {
                Assert.IsInstanceOfType(ms, typeof(MemoryStream));
                var b = new byte[ms.Length];
                ms.Read(b, 0, b.Length);
                Assert.AreEqual(sExp, new UTF8Encoding().GetString(b), $"Test: ");
            }
            else
                Assert.AreEqual(sExp, "", $"Test: ");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow(null, false, "")]
        [DataRow("", false, "")]
        [DataRow("1", false,
#if NET6_0_OR_GREATER
            "H4sIAAAAAAAACj" +
#else
            "H4sIAAAAAAAEAD" +
#endif
            "MEALfv3IMBAAAA"
            )]
/*        [DataRow("-1", false, "-1")]
        [DataRow("--2", false, "--2")] // ??
        [DataRow("-Hallo-3", false, "-Hallo-3")] // ??
        [DataRow("-Hallo 4", false, "-Hallo 4")] // ??
        [DataRow("Hallo", false, "Hallo")]
        [DataRow("Hallo-World", false, "Hallo-World")]
        [DataRow("Hallo-1", false, "Hallo-1")]
        [DataRow("Hallo 191238831290", false, "Hallo 191238831290")]
        [DataRow("Hallo 1912-3883-1290", false, "Hallo 1912-3883-1290")]*/
        [DataRow("Hallo 1912,3883,1290", false,
#if NET6_0_OR_GREATER
            "H4sIAAAAAAAACv" +
#else
            "H4sIAAAAAAAEAP" +
#endif
            "NIzMnJVzC0NDTSMbawMNYxNLI0AABGfwU+FAAAAA=="
            )]
        [DataRow("Ha11o", false,
#if NET6_0_OR_GREATER
            "H4sIAAAAAAAACv" +
#else
            "H4sIAAAAAAAEAP" +
#endif
            "NINDTMBwAvgMITBQAAAA=="
            )]
        [DataRow("tkSKmV0ot0HdTxKIAyBPxa0F3FNR7bRVvwY4iqDOt9nOXOKLM5GyIs5A1TlLh52MZ+OV4PczZt69" +
            "mjoACanw1GCfEPkvWvOvlXTB1UmXcxV42YyHxujuk45391ufLqgJrGeJCzw+M5JyYq4WXK4qcWp9PyGhw" +
            "zr9waHUi3ZNQVj2otsnnDoJkGKxw1yUE1z1m6p/KiWUGg73Un4iarI3cB/KWi4CpIsK+/TaUejzSHIV4l" +
            "7xDxrL/Mt/5/FYkzy+DMDjQEstjqooCkULknQzE/5vuan2VLTRRzXEsObecXdsXhkFZV1qaoQPtEgcVj6" +
            "YrL0Fc7jvyF7eY81BvC8WEuyAP/nUqKuHeE1ly0da0pQSM4BDjMdZIj8r8RxtlPZHWfcwMyzYCifMMFNT" +
            "/BlLi0srvPAMMStVK37MqKDmvNmQurKmQVzaJinu7OOOk2ayePy+0uVzfoGdxgGA8ISlPJHYm3dxoMxRv" +
            "3Ldhlthip7sDJWaYplBH5LsOsCOXCPR6+DV4ncc/uYgEyRpuJ8tX1CW/+e28xycp0UVPF7M0XqGxDcytp" +
            "ki9ZUOLGb2J/Ypi9Ly+ZX+LYs=", true,
#if NET6_0_OR_GREATER
            "H4sIAAAAAAAACg" +
#else
            "H4sIAAAAAAAEAA" +
#endif
            "G4AUf+tkSKmV0ot0HdTxKIAyBPxa0F3FNR7bRVvwY4iqDOt9nOXOKLM5GyIs5A1TlLh" +
            "52MZ+OV4PczZt69mjoACanw1GCfEPkvWvOvlXTB1UmXcxV42YyHxujuk45391ufLqgJrGeJCzw+M5JyYq" +
            "4WXK4qcWp9PyGhwzr9waHUi3ZNQVj2otsnnDoJkGKxw1yUE1z1m6p/KiWUGg73Un4iarI3cB/KWi4CpIs" +
            "K+/TaUejzSHIV4l7xDxrL/Mt/5/FYkzy+DMDjQEstjqooCkULknQzE/5vuan2VLTRRzXEsObecXdsXhkF" +
            "ZV1qaoQPtEgcVj6YrL0Fc7jvyF7eY81BvC8WEuyAP/nUqKuHeE1ly0da0pQSM4BDjMdZIj8r8RxtlPZHW" +
            "fcwMyzYCifMMFNT/BlLi0srvPAMMStVK37MqKDmvNmQurKmQVzaJinu7OOOk2ayePy+0uVzfoGdxgGA8I" +
            "SlPJHYm3dxoMxRv3Ldhlthip7sDJWaYplBH5LsOsCOXCPR6+DV4ncc/uYgEyRpuJ8tX1CW/+e28xycp0U" +
            "VPF7M0XqGxDcytpki9ZUOLGb2J/Ypi9Ly+ZX+LYsh+JAiuAEAAA==")]
        
        [DataRow(TestData.cLoremIpsum, false,
#if NET6_0_OR_GREATER
            "H4sIAAAAAAAACjWQy22DMQyD7wW6AwcI/i1666WHDqDYSirAr9hSkPFLJ+3NgmTyIz/71AobKypyL31im" +
            "UOq+gmpt6XJ1WNCsg1blgxajLulGWqxas9wrYMf39+sJcvRHOEocqY01F+yiirXJpBit5AD3w5tVqmLav" +
            "tx5yj1hFvYolTry2fQ46EzmYtbb4hSpKb+0uZZI9H2eoraPoYKsSup7IVPMz+o97VlJVxhQZipY+qPtqy" +
            "Tca3h3ksM2ihBGBC6liJZKf+9MErgElcT33gbBUMmx5gHPh5JhyuLWsb8/ZxEEy9TDMsbH438Y3bL2nZ9" +
            "uyXapihDqEd89MuF/QqyLp17X3vZKLLr2enWX6tRj1834XojtwEAAA=="
#else
            "H4sIAAAAAAAEADWQQW5DMQhE95V6hzlA9G/RXTdd9ADEJimSsR0bohy/kLQ7I/DMm/kcixUytyvqaGNhi" +
            "4GU7YQy+uZibL5AVaZsKQJuErvNFSy+dVQY64yP72/Si1TvBjc0Ooc02F6yDKVrJ1CTm9OBbwN30dCFSj" +
            "7uMZKecHPZIdXHtuXh8eBVxMhkdHhrpGW8tOOsB1F6PUUlj8EU2BpU8sIPMztC7ytlyY0hHjCL5+If7pV" +
            "XxJWO+2g+w4YDJAKC92YUae2/l4jiuPhVyBIvUTBpxejrwMej8DT2rC/yj3MhLnFZfEpNfPTgn2tI5Z71" +
            "ZUthW7xNCr3Ax7hcol9C5c0r9zpaolDWk+n2X6uuxy834XojtwEAAA=="
#endif
            )]
        public void AsCompStringTest(string sVal, bool xVal, string sExp)
        {
            Assert.AreEqual(sExp, StringExtension.String2Stream(sVal, xVal).AsCompString());
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow(null, false, "")]
        [DataRow("", false, "")]
        [DataRow("1", false, "MQ==")]
        /*        [DataRow("-1", false, "-1")]
                [DataRow("--2", false, "--2")] // ??
                [DataRow("-Hallo-3", false, "-Hallo-3")] // ??
                [DataRow("-Hallo 4", false, "-Hallo 4")] // ??
        /*/        [DataRow("Hallo", false, "SGFsbG8=")]     
                [DataRow("Hallo-World", false, "SGFsbG8tV29ybGQ=")]   
                [DataRow("Hallo World", false, "SGFsbG8gV29ybGQ=")] /*/
                [DataRow("Hallo-1", false, "Hallo-1")]
                [DataRow("Hallo 191238831290", false, "Hallo 191238831290")]
                [DataRow("Hallo 1912-3883-1290", false, "Hallo 1912-3883-1290")]*/
        [DataRow("Hallo 1912,3883,1290", false, "SGFsbG8gMTkxMiwzODgzLDEyOTA=")]
        [DataRow("Ha11o", false, "SGExMW8=")]
        [DataRow("tkSKmV0ot0HdTxKIAyBPxa0F3FNR7bRVvwY4iqDOt9nOXOKLM5GyIs5A1TlLh52MZ+OV4PczZt69" +
            "mjoACanw1GCfEPkvWvOvlXTB1UmXcxV42YyHxujuk45391ufLqgJrGeJCzw+M5JyYq4WXK4qcWp9PyGhw" +
            "zr9waHUi3ZNQVj2otsnnDoJkGKxw1yUE1z1m6p/KiWUGg73Un4iarI3cB/KWi4CpIsK+/TaUejzSHIV4l" +
            "7xDxrL/Mt/5/FYkzy+DMDjQEstjqooCkULknQzE/5vuan2VLTRRzXEsObecXdsXhkFZV1qaoQPtEgcVj6" +
            "YrL0Fc7jvyF7eY81BvC8WEuyAP/nUqKuHeE1ly0da0pQSM4BDjMdZIj8r8RxtlPZHWfcwMyzYCifMMFNT" +
            "/BlLi0srvPAMMStVK37MqKDmvNmQurKmQVzaJinu7OOOk2ayePy+0uVzfoGdxgGA8ISlPJHYm3dxoMxRv" +
            "3Ldhlthip7sDJWaYplBH5LsOsCOXCPR6+DV4ncc/uYgEyRpuJ8tX1CW/+e28xycp0UVPF7M0XqGxDcytp" +
            "ki9ZUOLGb2J/Ypi9Ly+ZX+LYs=", true,
            "tkSKmV0ot0HdTxKIAyBPxa0F3FNR7bRVvwY4iqDOt9nOXOKLM5GyIs5A1TlLh52MZ+OV4PczZt69" +
            "mjoACanw1GCfEPkvWvOvlXTB1UmXcxV42YyHxujuk45391ufLqgJrGeJCzw+M5JyYq4WXK4qcWp9PyGhw" +
            "zr9waHUi3ZNQVj2otsnnDoJkGKxw1yUE1z1m6p/KiWUGg73Un4iarI3cB/KWi4CpIsK+/TaUejzSHIV4l" +
            "7xDxrL/Mt/5/FYkzy+DMDjQEstjqooCkULknQzE/5vuan2VLTRRzXEsObecXdsXhkFZV1qaoQPtEgcVj6" +
            "YrL0Fc7jvyF7eY81BvC8WEuyAP/nUqKuHeE1ly0da0pQSM4BDjMdZIj8r8RxtlPZHWfcwMyzYCifMMFNT" +
            "/BlLi0srvPAMMStVK37MqKDmvNmQurKmQVzaJinu7OOOk2ayePy+0uVzfoGdxgGA8ISlPJHYm3dxoMxRv" +
            "3Ldhlthip7sDJWaYplBH5LsOsCOXCPR6+DV4ncc/uYgEyRpuJ8tX1CW/+e28xycp0UVPF7M0XqGxDcytp" +
            "ki9ZUOLGb2J/Ypi9Ly+ZX+LYs=")]

        [DataRow(TestData.cLoremIpsum, false, TestData.cBase64LoremIpsum )]
        public void AsBase54StringTest(string sVal, bool xVal, string sExp)
        {
            Assert.AreEqual(sExp, StringExtension.String2Stream(sVal, xVal).AsBase64String());
        }
    }
}