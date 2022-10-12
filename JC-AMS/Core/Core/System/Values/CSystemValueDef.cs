using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JCAMS.Core.System.Values
{
    [Serializable]
    public class CSystemValueDef : IHasDescription, IHasParent, ISerializable , IXmlSerializable
    {
        public string Description { get; private set; }
        public CStation Station { get; private set; }
        public Type DataType { get; private set; }

        public CSystemValueDef[] Children;

        public CSystemValueDef()
        {
        }

        public CSystemValueDef(string sDesc,Type tDType,CStation station)
        {
            Description = sDesc;
            DataType = tDType;
            Station = station;
        }

        public long idStation => Station?.idStation ?? -1;
        public bool IsStruct => Children != null; 
        
        object IHasParent.Parent { get => Station; set => Station = value as CStation; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute(nameof(Description));
            writer.WriteValue(Description);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(idStation));
            writer.WriteValue(idStation);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(DataType));
            writer.WriteValue(Type.GetTypeCode(DataType).ToString());
            writer.WriteEndAttribute();

        }

    }
}
