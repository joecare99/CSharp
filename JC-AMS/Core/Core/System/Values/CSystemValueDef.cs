using JCAMS.Core.Extensions;
using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JCAMS.Core.System.Values
{
    /// <summary>The defintion of Values</summary>
    [Serializable]
    public class CSystemValueDef : IHasDescription, IHasParent, ISerializable , IXmlSerializable
    {
        /// <summary>Gets the description.</summary>
        /// <value>The description of the Value(def)</value>
        public string Description { get; private set; }
        /// <summary>Gets the station.</summary>
        /// <value>The station of the value-defintion</value>
        public CStation Station { get; private set; }
        /// <summary>Gets the type of the data.</summary>
        /// <value>The type of the data.</value>
        public Type DataType { get; private set; }

        public CSystemValueDef? StructParent;
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
            info.AddValue(nameof(Description), Description, typeof(string));
            info.AddValue(nameof(idStation), idStation, typeof(long));
            info.AddValue(nameof(DataType), Type.GetTypeCode(DataType), typeof(TypeCode));
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToAttribute(nameof(Description));
            Description = reader.ReadString();

            reader.MoveToAttribute(nameof(idStation));
            var _idStation = reader.ReadString().AsInt64();
            Station = CStation.GetStation(_idStation);

            reader.MoveToAttribute(nameof(DataType));
            DataType = Type.GetType($"System.{reader.ReadString()}");
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
