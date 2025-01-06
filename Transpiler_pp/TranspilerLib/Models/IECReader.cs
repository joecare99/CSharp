using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using TranspilerLib.Interfaces;

namespace TranspilerLib.Models
{
    public class IECReader : IReader
    {
        private XmlReader _xr;

        public IECReader(XmlReader xr)
        {
            _xr = xr;
        }

        public bool Read()
        {
            return _xr.Read();
        }

        public bool EOF()
        {
            return _xr.EOF;
        }

        public bool IsStartElement()
        {
            return _xr.IsStartElement();
        }

        public bool IsEndElement()
        {
            return _xr.NodeType==XmlNodeType.EndElement;
        }
        public int GetAttributeCount()
        {
            return _xr.AttributeCount;
        }

        public object GetAttributeValue(int i)
        {
            return _xr.GetAttribute(i);
        }

        public string GetAttributeName(int i)
        {
            _xr.MoveToAttribute(i);
            return _xr.Name;
        }
        public string GetLocalName()
        {
            return _xr.LocalName;
        }

        public object getValue()
        {
            return _xr.Value;
        }

        public bool HasValue=>_xr.HasValue;

        public bool IsEmptyElement => _xr.IsEmptyElement;

    }
}
