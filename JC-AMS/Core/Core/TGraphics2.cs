// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="T.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using JCAMS.Core.Components.Coloring;
using JCAMS.Core.Extensions;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml;
namespace JCAMS.Core
{
    public static class TGraphics2
    {
        
        static TGraphics2()
        {
            
        }

        public static Color FindKnownColor(Color c)
        {
            Color result = Color.Empty;
            Color c2;
            foreach (KnownColor kc in Enum.GetValues(typeof(KnownColor)))
                if (ColorDiff(c,c2= Color.FromKnownColor(kc))+(c2.IsSystemColor?0:-0.0000001) < ColorDiff(c, result))
                    result = c2;
            return result;

            float ColorDiff(Color c1, Color c2) =>
                Math.Abs(c1.A - c2.A)/256f + Math.Abs(c1.R - c2.R) / 256f +
                Math.Abs(c1.G - c2.G) / 256f + Math.Abs(c1.B - c2.B) / 256f;
        }
        /// <summary>
        /// Color2s the string.
        /// </summary>
        /// <param name="Col">The col.</param>
        /// <returns>System.String.</returns>
        public static string Color2String(Color Col)
        {
            return $"{Col.A:X};{Col.R:X};{Col.G:X};{Col.B:X}"; //!!
        }

        /// <summary>
        /// Colors the cube2 string.
        /// </summary>
        /// <param name="Col">The col.</param>
        /// <returns>System.String.</returns>
        public static string ColorCube2String(CColorCube Col)
        {
            return $"{Col.Nr}~{Col.Label}~{Color2String(Col.List[0])}~{Color2String(Col.List[1])}~{Color2String(Col.List[2])}~{Color2String(Col.List[3])}";
        }

        /// <summary>
        /// Font2s the string.
        /// </summary>
        /// <param name="Font">The font.</param>
        /// <returns>System.String.</returns>
        public static string Font2String(Font Font)
        {
                return Font==null?"":$"{Font.Name};{Font.Size * 100f};{(int)Font.Style}";
        }

        /// <summary>
        /// Font2s the XML.
        /// </summary>
        /// <param name="Font">The font.</param>
        /// <param name="XMLNode">The XML node.</param>
        /// <returns>XmlNode.</returns>
        public static XmlNode Font2XML(Font Font, XmlNode XMLNode)
        {
            XmlAttribute Attribute = XMLNode.OwnerDocument.CreateAttribute("Name");
            Attribute.Value = Font.Name.ToString();
            XMLNode.Attributes.Append(Attribute);
            Attribute = XMLNode.OwnerDocument.CreateAttribute("Size");
            Attribute.Value = (Font.Size * 100f).ToString();
            XMLNode.Attributes.Append(Attribute);
            Attribute = XMLNode.OwnerDocument.CreateAttribute("Style");
            Attribute.Value = ((int)Font.Style).ToString();
            XMLNode.Attributes.Append(Attribute);
            return XMLNode;
        }

        /// <summary>
        /// Pen2s the string.
        /// </summary>
        /// <param name="Pen">The pen.</param>
        /// <returns>System.String.</returns>
        public static string Pen2String(Pen Pen)
        {
            return Pen==null?"":$"{Color2String(Pen.Color)};{Pen.Width:0.00}"; // CultureInfo ??
        }

        /// <summary>
        /// Point2s the string.
        /// </summary>
        /// <param name="Pt">The pt.</param>
        /// <returns>System.String.</returns>
        public static string Point2String(Point Pt)
        {
            return Pt==null?";":$"{Pt.X};{Pt.Y}";
        }

        /// <summary>
        /// Point2s the XML.
        /// </summary>
        /// <param name="Pt">The pt.</param>
        /// <param name="XMLNode">The XML node.</param>
        /// <returns>XmlNode.</returns>
        public static XmlNode Point2XML(Point Pt, XmlNode XMLNode)
        {
            XmlAttribute Attribute = XMLNode.OwnerDocument.CreateAttribute("X");
            Attribute.Value = Pt.X.ToString();
            XMLNode.Attributes.Append(Attribute);
            Attribute = XMLNode.OwnerDocument.CreateAttribute("Y");
            Attribute.Value = Pt.Y.ToString();
            XMLNode.Attributes.Append(Attribute);
            return XMLNode;
        }

        /// <summary>
        /// Rectangle2s the string.
        /// </summary>
        /// <param name="Rect">The rect.</param>
        /// <returns>System.String.</returns>
        public static string Rectangle2String(Rectangle Rect)
        {
            return Rect == null ? ";;;" : $"{Rect.X};{Rect.Y};{Rect.Width};{Rect.Height}";
        }

        /// <summary>
        /// String2s the color.
        /// </summary>
        /// <param name="Col">The col.</param>
        /// <returns>Color.</returns>
        public static Color String2Color(string Col)
        {
            string[] aC = Col.Split(';');
            if (aC.Length != 4)
            {
                return Color.Empty;
            }
            return Color.FromArgb(Convert.ToInt32(aC[0], 16), Convert.ToInt32(aC[1], 16), Convert.ToInt32(aC[2], 16), Convert.ToInt32(aC[3], 16));
        }

        /// <summary>
        /// String2s the color cube.
        /// </summary>
        /// <param name="Col">The col.</param>
        /// <returns>CColorCube.</returns>
        public static CColorCube String2ColorCube(string Col)
        {
            string[] aC = Col.Split('~');
            if (aC.Length != 6) //!!
            {
                return new CColorCube();
            }

            return new CColorCube(aC[0].AsInt32(), aC[1], String2Color(aC[2]), String2Color(aC[3]), String2Color(aC[4]), String2Color(aC[5]));
        }

        /// <summary>
        /// String2s the font.
        /// </summary>
        /// <param name="sFont">The font.</param>
        /// <returns>Font.</returns>
        public static Font String2Font(string sFont)
        {
            if (string.IsNullOrEmpty(sFont)) return null;
            string[] aF = sFont.Split(';');
            if (aF.Length != 3)
            {
                return SystemFonts.DefaultFont;
            }
            return new Font(aF[0], aF[1].AsFloat() / 100f, (FontStyle)aF[2].AsInt32(), GraphicsUnit.Point);
        }

        /// <summary>
        /// String2s the pen.
        /// </summary>
        /// <param name="sPen">The pen.</param>
        /// <returns>Pen.</returns>
        public static Pen String2Pen(string sPen)
        {
            if (string.IsNullOrEmpty(sPen)) return null;
            string[] aP = sPen.Split(';');
            if (aP.Length != 5)
            {
                return Pens.Black;
            }
            return new Pen(String2Color(sPen.Substring(0,sPen.Length- aP[4].Length-1)), aP[4].AsFloat());
        }

        /// <summary>
        /// String2s the point.
        /// </summary>
        /// <param name="Pen">The pen.</param>
        /// <returns>Point.</returns>
        public static Point String2Point(string Pen)
        {
            string[] aP = Pen.Split(';');
            if (aP.Length != 2)
            {
                return Point.Empty;
            }

            return new Point(aP[0].AsInt32(), aP[1].AsInt32());
        }

        /// <summary>
        /// String2s the rectangle.
        /// </summary>
        /// <param name="Rect">The rect.</param>
        /// <returns>Rectangle.</returns>
        public static Rectangle String2Rectangle(string Rect)
        {
            string[] aF = Rect.Split(';');
            if (aF.Length != 4)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(aF[0].AsInt32(), aF[1].AsInt32(), aF[2].AsInt32(), aF[3].AsInt32());
        }

        /// <summary>
        /// Xms the l2 font.
        /// </summary>
        /// <param name="XMLNode">The XML node.</param>
        /// <returns>Font.</returns>
        public static Font XML2Font(XmlNode XMLNode)
        {
            return new Font(XMLNode.Attributes["Name"].Value, XMLNode.Attributes["Size"].Value.AsFloat() / 100f, (FontStyle)XMLNode.Attributes["Style"].Value.AsInt32(), GraphicsUnit.Point);
        }

        /// <summary>
        /// Xms the l2 point.
        /// </summary>
        /// <param name="XMLNode">The XML node.</param>
        /// <returns>Point.</returns>
        public static Point XML2Point(XmlNode XMLNode)
        {
            return new Point(XMLNode.Attributes["X"].Value.AsInt32(), XMLNode.Attributes["Y"].Value.AsInt32());
        }
    }
}