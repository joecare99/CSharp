﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using JCAMS.Core.Components.Coloring;
using System.Xml;
using System.IO;

namespace JCAMS.Core.Tests
{
    [TestClass()]
    public class SGraphics2Tests_Base
    {
        #region Properties
        protected static IEnumerable<object[]> Color2StringData => new[]
        {
            new object[]{ "Null",null,"0;0;0;0" },
            new object[]{ "Empty",Color.Empty,"0;0;0;0" },
            new object[]{ "Black",Color.Black,"FF;0;0;0" },
            new object[]{ "White", Color.White,"FF;FF;FF;FF" },
            new object[]{ "Transparent",Color.Transparent,"0;FF;FF;FF" },
            new object[]{ "Blue", Color.Blue,"FF;0;0;FF" },
            new object[]{ "Cyan", Color.Cyan,"FF;0;FF;FF" },
            new object[]{ "Lime!", Color.Lime,"FF;0;FF;0" },
            new object[]{ "Yellow", Color.Yellow,"FF;FF;FF;0" },
            new object[]{ "Red", Color.Red,"FF;FF;0;0" },
            new object[]{ "Magenta", Color.Magenta, "FF;FF;0;FF" },
            new object[]{ "DarkCyan", Color.DarkCyan,"FF;0;8B;8B" },
            new object[]{ "Green", Color.Green,"FF;0;80;0" },
            new object[]{ "DarkGreen", Color.DarkGreen,"FF;0;64;0" },
            new object[]{ "DarkOrange", Color.DarkOrange,"FF;FF;8C;0" },
            new object[]{ "DarkRed", Color.DarkRed,"FF;8B;0;0" },
            new object[]{ "Magenta2", SGraphics2.FindKnownColor(Color.FromArgb(255, 255, 0, 255)),"FF;FF;0;FF" },
            new object[]{ "Magenta1", Color.FromArgb(255,255,0,255),"FF;FF;0;FF" },
            new object[]{ "DarkBlue", Color.DarkBlue,"FF;0;0;8B" },
            new object[]{ "DarkKhaki", Color.DarkKhaki,"FF;BD;B7;6B" },
            new object[]{ "DarkGray", Color.DarkGray,"FF;A9;A9;A9" },
            new object[]{ "Lime?", Color.FromArgb(255,0,255,0),"FF;0;FF;0" },
            new object[]{ "GreenYellow", Color.GreenYellow,"FF;AD;FF;2F" },
            new object[]{ "Orange", Color.Orange,"FF;FF;A5;0" },
            new object[]{ "OrangeRed", Color.OrangeRed,"FF;FF;45;0" },
            new object[]{ "Purple", Color.Purple,"FF;80;0;80" }
        };

        protected static IEnumerable<object?[]> Color2StringXData => new[]
{
            new object?[]{ "Null",null,"0;0;0;0",new string[] { "<Color color=\"#00000000\" />", "color=\"#00000000\"" } },
            new object[]{ "Empty",Color.Empty,"0;0;0;0",new string[] { "<Color color=\"#00000000\" />", "color=\"#00000000\"" } },
            new object[]{ "Black",Color.Black,"FF;0;0;0",new string[] { "<Color Name=\"Black\" />", "Name=\"Black\"" } },
            new object[]{ "White", Color.White,"FF;FF;FF;FF",new string[] { "<Color Name=\"White\" />", "Name=\"White\"" } },
            new object[]{ "Transparent",Color.Transparent,"0;FF;FF;FF",new string[] { "<Color Name=\"Transparent\" />", "Name=\"Transparent\"" } },
            new object[]{ "Blue", Color.Blue,"FF;0;0;FF",new string[] { "<Color Name=\"Blue\" />", "Name=\"Blue\"" } },
            new object[]{ "Cyan", Color.Cyan,"FF;0;FF;FF",new string[] { "<Color Name=\"Cyan\" />", "Name=\"Cyan\"" } },
            new object[]{ "Lime!", Color.Lime,"FF;0;FF;0",new string[] { "<Color Name=\"Lime\" />", "Name=\"Lime\"" } },
            new object[]{ "Yellow", Color.Yellow,"FF;FF;FF;0",new string[] { "<Color Name=\"Yellow\" />", "Name=\"Yellow\"" } },
            new object[]{ "Red", Color.Red,"FF;FF;0;0",new string[] { "<Color Name=\"Red\" />", "Name=\"Red\"" } },
            new object[]{ "Magenta", Color.Magenta, "FF;FF;0;FF",new string[] { "<Color Name=\"Magenta\" />", "Name=\"Magenta\"" } },
            new object[]{ "DarkCyan", Color.DarkCyan,"FF;0;8B;8B",new string[] { "<Color Name=\"DarkCyan\" />", "Name=\"DarkCyan\"" } },
            new object[]{ "Green", Color.Green,"FF;0;80;0",new string[] { "<Color Name=\"Green\" />", "Name=\"Green\"" } },
            new object[]{ "DarkGreen", Color.DarkGreen,"FF;0;64;0",new string[] { "<Color Name=\"DarkGreen\" />", "Name=\"DarkGreen\"" } },
            new object[]{ "DarkOrange", Color.DarkOrange,"FF;FF;8C;0",new string[] { "<Color Name=\"DarkOrange\" />", "Name=\"DarkOrange\"" } },
            new object[]{ "DarkRed", Color.DarkRed,"FF;8B;0;0",new string[] { "<Color Name=\"DarkRed\" />", "Name=\"DarkRed\"" } },
            new object[]{ "Magenta2", SGraphics2.FindKnownColor(Color.FromArgb(255, 255, 0, 255)),"FF;FF;0;FF",new string[] { "<Color Name=\"Magenta\" />", "Name=\"Magenta\"" } },
            new object[]{ "Magenta1", Color.FromArgb(255,255,0,255),"FF;FF;0;FF",new string[] { "<Color color=\"#FFFF00FF\" />", "color=\"#FFFF00FF\"" } },
            new object[]{ "DarkBlue", Color.DarkBlue,"FF;0;0;8B",new string[] { "<Color Name=\"DarkBlue\" />", "Name=\"DarkBlue\"" } },
            new object[]{ "DarkKhaki", Color.DarkKhaki,"FF;BD;B7;6B",new string[] { "<Color Name=\"DarkKhaki\" />", "Name=\"DarkKhaki\"" } },
            new object[]{ "DarkGray", Color.DarkGray,"FF;A9;A9;A9",new string[] { "<Color Name=\"DarkGray\" />", "Name=\"DarkGray\"" } },
            new object[]{ "Lime?", Color.FromArgb(255,0,255,0),"FF;0;FF;0",new string[] { "<Color color=\"#FF00FF00\" />", "color=\"#FF00FF00\"" } },
            new object[]{ "GreenYellow", Color.GreenYellow,"FF;AD;FF;2F",new string[] { "<Color Name=\"GreenYellow\" />", "Name=\"GreenYellow\"" } },
            new object[]{ "Orange", Color.Orange,"FF;FF;A5;0",new string[] { "<Color Name=\"Orange\" />", "Name=\"Orange\"" } },
            new object[]{ "OrangeRed", Color.OrangeRed,"FF;FF;45;0",new string[] { "<Color Name=\"OrangeRed\" />", "Name=\"OrangeRed\"" } },
            new object[]{ "Purple", Color.Purple,"FF;80;0;80", new string[] { "<Color Name=\"Purple\" />", "Name=\"Purple\"" } }
        };

        protected static IEnumerable<object[]> ColorCube2StringData => new[]
        {
            new object[]{ "Empty",CColorCube.Empty,"0~~FF;FF;FF;FF~FF;FF;FF;FF~FF;FF;FF;FF~FF;FF;FF;FF" },
            new object[]{ "Black", CColorCube.Create(0,"Black",Color.Black, Color.Black, Color.Black, Color.Black), "0~Black~FF;0;0;0~FF;0;0;0~FF;0;0;0~FF;0;0;0" },
            new object[]{ "White", CColorCube.Create(1, "White", Color.White, Color.White, Color.White, Color.White), "1~White~FF;FF;FF;FF~FF;FF;FF;FF~FF;FF;FF;FF~FF;FF;FF;FF" },
            new object[]{ "Transparent", CColorCube.Create(2, "Transparent", Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent), "2~Transparent~0;FF;FF;FF~0;FF;FF;FF~0;FF;FF;FF~0;FF;FF;FF" },
            new object[]{ "Blue", CColorCube.Create(3, "Blue", Color.Blue, Color.Blue, Color.Blue, Color.Blue), "3~Blue~FF;0;0;FF~FF;0;0;FF~FF;0;0;FF~FF;0;0;FF" },
            new object[]{ "Cyan", CColorCube.Create(4, "Cyan", Color.Cyan, Color.Cyan, Color.Cyan, Color.Cyan), "4~Cyan~FF;0;FF;FF~FF;0;FF;FF~FF;0;FF;FF~FF;0;FF;FF" },
            new object[]{ "Lime", CColorCube.Create(5, "Lime", Color.Lime, Color.Lime, Color.Lime, Color.Lime), "5~Lime~FF;0;FF;0~FF;0;FF;0~FF;0;FF;0~FF;0;FF;0" },
            new object[]{ "Yellow", CColorCube.Create(6, "Yellow", Color.Yellow, Color.Yellow, Color.Yellow, Color.Yellow), "6~Yellow~FF;FF;FF;0~FF;FF;FF;0~FF;FF;FF;0~FF;FF;FF;0" },
            new object[]{ "Red", CColorCube.Create(7, "Red", Color.Red, Color.Red, Color.Red, Color.Red), "7~Red~FF;FF;0;0~FF;FF;0;0~FF;FF;0;0~FF;FF;0;0" },
            new object[]{ "Magenta", CColorCube.Create(8, "Magenta", Color.Magenta, Color.Magenta, Color.Magenta, Color.Magenta), "8~Magenta~FF;FF;0;FF~FF;FF;0;FF~FF;FF;0;FF~FF;FF;0;FF" },
            new object[]{ "Colorset1", CColorCube.Create(9, "Colorset1", Color.Blue, Color.Cyan, Color.Lime, Color.Yellow), "9~Colorset1~FF;0;0;FF~FF;0;FF;FF~FF;0;FF;0~FF;FF;FF;0" },
            new object[]{ "Colorset2", CColorCube.Create(10, "Colorset2", Color.Lime, Color.Yellow, Color.Red, Color.Magenta), "10~Colorset2~FF;0;FF;0~FF;FF;FF;0~FF;FF;0;0~FF;FF;0;FF" },
            
            /*
            new object[]{ "DarkCyan", Color.DarkCyan,"FF;0;8B;8B" },
            new object[]{ "Green", Color.Green,"FF;0;80;0" },
            new object[]{ "DarkGreen", Color.DarkGreen,"FF;0;64;0" },
            new object[]{ "DarkOrange", Color.DarkOrange,"FF;FF;8C;0" },
            new object[]{ "DarkRed", Color.DarkRed,"FF;8B;0;0" },
            new object[]{ "Magenta2", SGraphics2.FindKnownColor(Color.FromArgb(255, 255, 0, 255)),"FF;FF;0;FF" },
            new object[]{ "Magenta1", Color.FromArgb(255,255,0,255),"FF;FF;0;FF" },
            new object[]{ "DarkBlue", Color.DarkBlue,"FF;0;0;8B" },
            new object[]{ "DarkKhaki", Color.DarkKhaki,"FF;BD;B7;6B" },
            new object[]{ "DarkGray", Color.DarkGray,"FF;A9;A9;A9" },
            new object[]{ "Lime?", Color.FromArgb(255,0,255,0),"FF;0;FF;0" },
            new object[]{ "GreenYellow", Color.GreenYellow,"FF;AD;FF;2F" },
            new object[]{ "Orange", Color.Orange,"FF;FF;A5;0" },
            new object[]{ "OrangeRed", Color.OrangeRed,"FF;FF;45;0" },
            new object[]{ "Purple", Color.Purple,"FF;80;0;80" }*/
        };

        protected static IEnumerable<object?[]> Font2StringData => new[]
        {
            new object?[]{ "null", null, "" },
            new object[]{ "DefaultFont", SystemFonts.DefaultFont, "Microsoft Sans Serif;825;0" },
            new object[]{ "DialogFont", SystemFonts.DialogFont, "Tahoma;800;0" },
            new object[]{ "MenuFont", SystemFonts.MenuFont, "Segoe UI;900;0" },
            new object[]{ "TimesNewRoman,12", new Font("Times New Roman",12), "Times New Roman;1200;0" },
            new object[]{ "Courier New,18,Bold & Italic", new Font("Courier New",18,FontStyle.Bold | FontStyle.Italic), "Courier New;1800;3" },
            new object[]{ "MS Sans,8,Underline", new Font("MS Sans Serif",8,FontStyle.Underline), "Microsoft Sans Serif;800;4" },
        };

        protected static IEnumerable<object?[]> Font2StringXData => new[]
{
            new object?[]{ "null", null,new[] { "<NULL />","" } },
            new object[]{ "DefaultFont", SystemFonts.DefaultFont,new[] { "<Font Name=\"Microsoft Sans Serif\" Size=\"8.25\" Style=\"0\" />", "Name=\"Microsoft Sans Serif\" Size=\"8.25\" Style=\"0\"" } },
            new object[]{ "DialogFont", SystemFonts.DialogFont, new[] { "<Font Name=\"Tahoma\" Size=\"8\" Style=\"0\" />", "Name=\"Tahoma\" Size=\"8\" Style=\"0\"" } },
            new object[]{ "MenuFont", SystemFonts.MenuFont,new[] { "<Font Name=\"Segoe UI\" Size=\"9\" Style=\"0\" />", "Name=\"Segoe UI\" Size=\"9\" Style=\"0\"" } },
            new object[]{ "TimesNewRoman,12", new Font("Times New Roman",12),new[] { "<Font Name=\"Times New Roman\" Size=\"12\" Style=\"0\" />", "Name=\"Times New Roman\" Size=\"12\" Style=\"0\"" } },
            new object[]{ "Courier New,18,Bold & Italic", new Font("Courier New",18,FontStyle.Bold | FontStyle.Italic), new[] { "<Font Name=\"Courier New\" Size=\"18\" Style=\"3\" />", "Name=\"Courier New\" Size=\"18\" Style=\"3\"" } },
            new object[]{ "MS Sans,8,Underline", new Font("MS Sans Serif",8,FontStyle.Underline),new[] { "<Font Name=\"Microsoft Sans Serif\" Size=\"8\" Style=\"4\" />", "Name=\"Microsoft Sans Serif\" Size=\"8\" Style=\"4\"" } },
        };

        protected static IEnumerable<object?[]> Pen2StringXData => new[]
        {
            new object?[]{ "null", null, "",new[] { "<NULL />", "" } },
            new object[]{ "Black", Pens.Black, "FF;0;0;0;1,00",new[] { "<Pen Width=\"1.00\" PenType=\"0\">\r\n  <Color Name=\"Black\" />\r\n</Pen>", "Width=\"1.00\" PenType=\"0\" Color.Name=\"Black\"" } },
            new object[]{ "Blue-1.1", new Pen(new SolidBrush(Color.Blue),1.1f), "FF;0;0;FF;1,10",new[] { "<Pen Width=\"1.10\" PenType=\"0\">\r\n  <Color color=\"#FF0000FF\" />\r\n</Pen>", "Width=\"1.10\" PenType=\"0\" Color.color=\"#FF0000FF\"" } },
            new object[]{ "Cyan-1.1", new Pen(new SolidBrush(Color.Cyan),1.2f), "FF;0;FF;FF;1,20",new[] { "<Pen Width=\"1.20\" PenType=\"0\">\r\n  <Color color=\"#FF00FFFF\" />\r\n</Pen>", "Width=\"1.20\" PenType=\"0\" Color.color=\"#FF00FFFF\"" } },
            new object[]{ "Green-1.3", new Pen(Color.Lime,1.3f), "FF;0;FF;0;1,30",new[] { "<Pen Width=\"1.30\" PenType=\"0\">\r\n  <Color Name=\"Lime\" />\r\n</Pen>", "Width=\"1.30\" PenType=\"0\" Color.Name=\"Lime\"" } },
            new object[]{ "Yellow-1.3", new Pen(Color.Yellow,1.4f), "FF;FF;FF;0;1,40",new[] { "<Pen Width=\"1.40\" PenType=\"0\">\r\n  <Color Name=\"Yellow\" />\r\n</Pen>", "Width=\"1.40\" PenType=\"0\" Color.Name=\"Yellow\"" } },
            new object[]{ "Red-1.5", new Pen(new SolidBrush(Color.Red),1.5f), "FF;FF;0;0;1,50",new[] { "<Pen Width=\"1.50\" PenType=\"0\">\r\n  <Color color=\"#FFFF0000\" />\r\n</Pen>", "Width=\"1.50\" PenType=\"0\" Color.color=\"#FFFF0000\"" } },
            new object[]{ "Magenta-1.6", new Pen(new SolidBrush(Color.Magenta),1.6f), "FF;FF;0;FF;1,60",new[] { "<Pen Width=\"1.60\" PenType=\"0\">\r\n  <Color color=\"#FFFF00FF\" />\r\n</Pen>", "Width=\"1.60\" PenType=\"0\" Color.color=\"#FFFF00FF\"" } },
            new object[]{ "White-1.7", new Pen(Color.White,1.7f), "FF;FF;FF;FF;1,70",new[] { "<Pen Width=\"1.70\" PenType=\"0\">\r\n  <Color Name=\"White\" />\r\n</Pen>", "Width=\"1.70\" PenType=\"0\" Color.Name=\"White\"" } },
            new object[]{ "Transparent-1.8", new Pen(Color.Transparent,1.8f), "0;FF;FF;FF;1,80",new[] { "<Pen Width=\"1.80\" PenType=\"0\">\r\n  <Color Name=\"Transparent\" />\r\n</Pen>", "Width=\"1.80\" PenType=\"0\" Color.Name=\"Transparent\"" } },
        };

        protected static IEnumerable<object?[]> Point2StringXData => new[]
        {
            new object?[]{ "null", null, "0;0",new[] { "<Point X=\"0\" Y=\"0\" />", "X=\"0\" Y=\"0\"" } }, // ?
            new object[]{ "Empty", Point.Empty, "0;0",new[] { "<Point X=\"0\" Y=\"0\" />", "X=\"0\" Y=\"0\"" } },
            new object[]{ "MinValue", new Point(int.MinValue,int.MinValue), "-2147483648;-2147483648",new[] { "<Point X=\"-2147483648\" Y=\"-2147483648\" />", "X=\"-2147483648\" Y=\"-2147483648\"" } },
            new object[]{ "MaxValue", new Point(int.MaxValue, int.MaxValue), "2147483647;2147483647",new[] { "<Point X=\"2147483647\" Y=\"2147483647\" />", "X=\"2147483647\" Y=\"2147483647\"" } },
            new object[]{ "(1;2)", new Point(1, 2), "1;2",new[] { "<Point X=\"1\" Y=\"2\" />", "X=\"1\" Y=\"2\"" } },
        };

        protected static IEnumerable<object?[]> Rectangle2StringData => new[]
        {
            new object?[]{ "null", null,"0;0;0;0" },
            new object[]{ "Empty", Rectangle.Empty,"0;0;0;0" },
            new object[]{ "MinMax", new Rectangle(int.MinValue, int.MinValue,int.MaxValue, int.MaxValue),"-2147483648;-2147483648;2147483647;2147483647" },
            new object[]{ "MaxMin", new Rectangle(int.MaxValue, int.MaxValue,int.MinValue, int.MinValue),"2147483647;2147483647;-2147483648;-2147483648" },
            new object[]{ "1,2,3,4", new Rectangle(1, 2,3, 4),"1;2;3;4" },
        };
        #endregion

        #region Methods
        protected void AssertAreEqualPen(Pen pExp, Pen pAct, string v)
        {
            if (pExp == null && pAct == null) return;
            Assert.AreEqual(pExp == null, pAct == null, $"{v}.IsNull");
            Assert.AreEqual(pExp.Color.ToArgb(), pAct.Color.ToArgb(), $"{v}.Color");
            Assert.AreEqual(pExp.Width, pAct.Width, $"{v}.Width");
            Assert.AreEqual(pExp.Transform, pAct.Transform, $"{v}.Transform");
        }
        #endregion
    }
    [TestClass()]
    public class SGraphics2Tests_Std : SGraphics2Tests_Base
    {
        #region Properties
        protected new static IEnumerable<object[]> Color2StringData => SGraphics2Tests_Base.Color2StringData;
        protected new static IEnumerable<object[]> ColorCube2StringData => SGraphics2Tests_Base.ColorCube2StringData;
        protected new static IEnumerable<object[]> Font2StringData => SGraphics2Tests_Base.Font2StringData;
        protected new static IEnumerable<object[]> Pen2StringXData => SGraphics2Tests_Base.Pen2StringXData;
        protected new static IEnumerable<object[]> Point2StringXData => SGraphics2Tests_Base.Point2StringXData;
        protected new static IEnumerable<object[]> Rectangle2StringData => SGraphics2Tests_Base.Rectangle2StringData;
        #endregion

        #region Methods
        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Color2StringData))]
        public void Color2StringTest(string name, Color cVal, string sExp)
        {
            Assert.AreEqual(sExp, SGraphics2.Color2String(cVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(ColorCube2StringData))]
        public void ColorCube2StringTest(string name, CColorCube cVal, string sExp)
        {
            Assert.AreEqual(sExp, SGraphics2.ColorCube2String(cVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Font2StringData))]
        public void Font2StringTest(string name, Font fVal, string sExp)
        {
            Assert.AreEqual(sExp, SGraphics2.Font2String(fVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Pen2StringXData))]
        public void Pen2StringTest(string name, Pen pVal, string sExp, string[] xExp)
        {
            Assert.AreEqual(sExp, SGraphics2.Pen2String(pVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Point2StringXData))]
        public void Point2StringTest(string name, Point pVal, string sExp, string[] xExp)
        {
            Assert.AreEqual(sExp, SGraphics2.Point2String(pVal), $"Test: {name}");
        }


        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Rectangle2StringData))]
        public void Rectangle2StringTest(string name, Rectangle rVal, string sExp)
        {
            Assert.AreEqual(sExp, SGraphics2.Rectangle2String(rVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Color2StringData))]
        public void String2ColorTest(string name, Color cExp, string sVal)
        {
            Assert.AreEqual(Color.FromArgb(cExp.ToArgb()), SGraphics2.String2Color(sVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(ColorCube2StringData))]
        public void String2ColorCubeTest(string name, CColorCube cExp, string sVal)
        {
            Assert.AreEqual(cExp, SGraphics2.String2ColorCube(sVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Font2StringData))]
        public void String2FontTest(string name, Font fExp, string sVal)
        {
            Font fAct = default;
            try
            {
                Assert.AreEqual(fExp, fAct = SGraphics2.String2Font(sVal), $"Test: {name}");
            }
            catch (AssertFailedException afe)
            {
                if (fExp?.Name != fAct?.Name
                    || fExp?.Size != fAct?.Size
                    || fExp?.Style != fAct?.Style) throw;
                if (fExp?.GdiCharSet == 1) throw;
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Pen2StringXData))]
        public void String2PenTest(string name, Pen pExp, string sVal, string[] xExp)
        {
            AssertAreEqualPen(pExp, SGraphics2.String2Pen(sVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Point2StringXData))]
        public void String2PointTest(string name, Point pExp, string sVal, string[] xVal)
        {
            Assert.AreEqual(pExp, SGraphics2.String2Point(sVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Rectangle2StringData))]
        public void String2RectangleTest(string name, Rectangle rExp, string sVal)
        {
            Assert.AreEqual(rExp, SGraphics2.String2Rectangle(sVal), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Color2StringData))]
        public void FindKnownColorTest(string name, Color cVal, string sExp)
        {
            Color c = SGraphics2.FindKnownColor(cVal);
            if (c != Color.Empty)
            {
                Assert.AreEqual(true, c.IsNamedColor);
                Assert.AreEqual(name.Substring(0, Math.Min(c.Name.Length, name.Length)), c.Name, $"Test: {name}");
            }
            else
                Assert.AreEqual(cVal, c);
        }
        #endregion
    }
    [TestClass()]
    public class SGraphics2Tests_Xml : SGraphics2Tests_Base
    {
        protected new static IEnumerable<object[]> Color2StringXData => SGraphics2Tests_Base.Color2StringXData;
        protected new static IEnumerable<object[]> ColorCube2StringData => SGraphics2Tests_Base.ColorCube2StringData;
        protected new static IEnumerable<object[]> Font2StringXData => SGraphics2Tests_Base.Font2StringXData;
        protected new static IEnumerable<object[]> Pen2StringXData => SGraphics2Tests_Base.Pen2StringXData;
        protected new static IEnumerable<object[]> Point2StringXData => SGraphics2Tests_Base.Point2StringXData;
        /*
        [TestMethod()]
        public void Font2XMLTest(string name, Font fVal, string sExp)
        {
            XmlNode xn = null;
            Assert.AreEqual(sExp, SGraphics2.Font2XML(fVal,xn), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Point2StringXData))]
        public void Point2XMLTest(string name, Point pVal, string sExp, string[] xExp)
        {
            var xNode = new XmlDocument();
            Assert.AreEqual(xExp, SGraphics2.Point2XML(pVal, xNode)?.InnerXml, $"Test: {name}");
        }
        */
        [TestMethod()]
        public void XML2FontTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void XML2PointTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Font2StringXData))]
        public void WriteToXMLTest_Font(string sVal,Font fVal,string[] sExp)
        {
            using (var tw = new StringWriter())
            {
            using (var xsw = new XmlTextWriter(tw))
            {
                xsw.Formatting = Formatting.Indented;
                xsw.Indentation = 2;
                SGraphics2.WriteToXML(fVal,xsw,true);           
            }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Font2StringXData))]
        public void WriteToXMLTest1_Font(string sVal, Font fVal, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    SGraphics2.WriteToXML(fVal, xsw);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[1], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Font2StringXData))]
        public void WriteToXMLTest2_Font(string sVal, Font fVal, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    fVal.WriteToXML(xsw,true);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Font2StringXData))]
        public void WriteToXMLTest3_Font(string sVal, Font fVal, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    fVal.WriteToXML(xsw);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[1], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Point2StringXData))]
        public void WriteToXMLTest_Point(string sVal, Point fVal,string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    SGraphics2.WriteToXML(fVal, xsw, true);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Point2StringXData))]
        public void WriteToXMLTest1_Point(string sVal, Point fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    SGraphics2.WriteToXML(fVal, xsw);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[1], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Point2StringXData))]
        public void WriteToXMLTest2_Point(string sVal, Point fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    fVal.WriteToXML(xsw, true);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Point2StringXData))]
        public void WriteToXMLTest3_Point(string sVal, Point fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    fVal.WriteToXML(xsw);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[1], s);
            }
        }


        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Pen2StringXData))]
        public void WriteToXMLTest_Pen(string sVal, Pen fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    SGraphics2.WriteToXML(fVal, xsw, true);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Pen2StringXData))]
        public void WriteToXMLTest1_Pen(string sVal, Pen fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    SGraphics2.WriteToXML(fVal, xsw);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[1], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Pen2StringXData))]
        public void WriteToXMLTest2_Pen(string sVal, Pen fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    fVal.WriteToXML(xsw, true);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Pen2StringXData))]
        public void WriteToXMLTest3_Pen(string sVal, Pen fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    fVal.WriteToXML(xsw);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[1], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Pen2StringXData))]
        public void WriteToXMLTest4_Pen(string sVal, Pen fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    if (fVal == null) xsw.WriteElementString("NULL", "");
                    else
                    {
                        xsw.WriteStartElement(fVal.GetType().Name);
                        SGraphics2.WriteToXML(fVal, xsw);
                        xsw.WriteEndElement();
                    }
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Pen2StringXData))]
        public void WriteToXMLTest5_Pen(string sVal, Pen fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    if (fVal == null) xsw.WriteElementString("NULL", "");
                    else
                    {
                        xsw.WriteStartElement(fVal.GetType().Name);
                        fVal.WriteToXML(xsw);
                        xsw.WriteEndElement();
                    }
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }
        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Color2StringXData))]
        public void WriteToXMLTest_Color(string sVal, Color fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    SGraphics2.WriteToXML(fVal, xsw, true);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Color2StringXData))]
        public void WriteToXMLTest1_Color(string sVal, Color fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    SGraphics2.WriteToXML(fVal, xsw);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[1], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Color2StringXData))]
        public void WriteToXMLTest2_Color(string sVal, Color fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    fVal.WriteToXML(xsw, true);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Color2StringXData))]
        public void WriteToXMLTest3_Color(string sVal, Color fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    fVal.WriteToXML(xsw);
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[1], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Color2StringXData))]
        public void WriteToXMLTest4_Color(string sVal, Color fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    xsw.WriteStartElement(fVal.GetType().Name);
                    SGraphics2.WriteToXML(fVal, xsw);
                    xsw.WriteEndElement();
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DynamicData(nameof(Color2StringXData))]
        public void WriteToXMLTest5_Color(string sVal, Color fVal, string sP, string[] sExp)
        {
            using (var tw = new StringWriter())
            {
                using (var xsw = new XmlTextWriter(tw))
                {
                    xsw.Formatting = Formatting.Indented;
                    xsw.Indentation = 2;
                    xsw.WriteStartElement(fVal.GetType().Name);
                    fVal.WriteToXML(xsw);
                    xsw.WriteEndElement();
                }
                var s = tw.ToString();
                Assert.AreEqual(sExp[0], s);
            }
        }
    }
}
