// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 07-13-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="XmlNS.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Xml;
using System.IO;

namespace TestStatements.SystemNS.Xml
{
    /// <summary>
    /// Class XmlNS.
    /// </summary>
    public class XmlNS
    {
        /// <summary>
        /// XMLs the test1.
        /// </summary>
        public static void XmlTest1()
        {
            var xml = new XmlDocument();
			xml.PreserveWhitespace = true;
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF8", ""));

            var xmlNode = xml.CreateNode(XmlNodeType.Element, "ROOT", "");
            var xAttr = xml.CreateAttribute("Charset");
            xAttr.Value = "UTF8";
            xmlNode.Attributes?.Append(xAttr);
            xml.AppendChild(xmlNode);

            var xmlNode2 = xml.CreateNode(XmlNodeType.Element, "Node", "");
            xmlNode2.InnerText = "Node Text";
            xmlNode.AppendChild(xmlNode2);

            xmlNode2 = xml.CreateNode(XmlNodeType.Element, "Node2", "");
            xmlNode2.InnerText = "Node2 Text";
            xmlNode.AppendChild(xmlNode2);

            Console.WriteLine(xml.OuterXml);
            using (FileStream fs = new FileStream("test.xml", FileMode.OpenOrCreate))
            using (XmlWriter xw = new XmlTextWriter(fs, System.Text.Encoding.UTF8))
            {
                xml.WriteTo(xw);
            }
        }
    }
}
