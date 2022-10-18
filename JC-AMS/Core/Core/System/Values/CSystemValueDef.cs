using JCAMS.Core.DataOperations;
using JCAMS.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JCAMS.Core.System.Values
{
    /// <summary>The defintion of Values</summary>
    [Serializable]
    public class CSystemValueDef : CPropNotificationClass, IHasDescription, IHasParent, ISerializable , IXmlSerializable, IHasID
    {
        #region Properties
        #region private properties
        private string _Description;
        private long _idValueDef;
        #endregion
        public long idValueDef { get => _idValueDef; private set => SetValue(value, ref _idValueDef); }
        /// <summary>Gets the description.</summary>
        /// <value>The description of the Value(def)</value>
        public string Description { get=> _Description; private set=> SetValue(value, ref _Description); }
        /// <summary>Gets the station.</summary>
        /// <value>The station of the value-defintion</value>
        public CStation Station { get; private set; }
        /// <summary>Gets the type of the data.</summary>
        /// <value>The type of the data.</value>
        public Type DataType { get; private set; }

        public CSystemValueDef? StructParent;
        public List<CSystemValueDef> Children=new List<CSystemValueDef>();

        public long idStation => Station?.idStation ?? -1;
        public bool IsStruct => Children != null;
        public bool IsArray => (MinIndex < MaxIndex);

        object IHasParent.Parent { get => Station; set => Station = value as CStation; }
        public int MinIndex { get; private set; }
        public int MaxIndex { get; private set; }

        public long ID => throw new NotImplementedException();
        #endregion

        #region Methods
        public CSystemValueDef()
        {
        }

        public CSystemValueDef(string sDesc,Type tDType,CStation station)
        {
            Description = sDesc;
            DataType = tDType;
            Station = station;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Description), Description, typeof(string));
            info.AddValue(nameof(idStation), idStation, typeof(long));
            info.AddValue(nameof(DataType), Type.GetTypeCode(DataType), typeof(TypeCode));
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToAttribute(nameof(idValueDef));
            idValueDef = reader.ReadContentAsLong();

            reader.MoveToAttribute(nameof(Description));
            Description = reader.ReadContentAsString();

            reader.MoveToAttribute(nameof(idStation));
            var _idStation = reader.ReadContentAsLong();
            Station = CStation.GetStation(_idStation);

            reader.MoveToAttribute(nameof(DataType));
            DataType = Type.GetType($"System.{reader.ReadContentAsString()}");

            reader.MoveToAttribute(nameof(MinIndex));
            MinIndex = reader.ReadContentAsInt();

            reader.MoveToAttribute(nameof(MaxIndex));
            MaxIndex = reader.ReadContentAsInt();

            reader.MoveToAttribute(nameof(Children) + ".Count");
            var _childCount = reader.ReadContentAsInt();

            for (int i = 0; i < _childCount; i++)
            {
                var _svd = reader.ReadContentAsObject();
                if (_svd is CSystemValueDef svd)
                   Children.Add(svd);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute(nameof(idValueDef));
            writer.WriteValue(idValueDef);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Description));
            writer.WriteValue(Description);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(idStation));
            writer.WriteValue(idStation);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(DataType));
            writer.WriteValue(Type.GetTypeCode(DataType).ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(MinIndex));
            writer.WriteValue(MinIndex);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(MaxIndex));
            writer.WriteValue(MaxIndex);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Children)+".Count");
            writer.WriteValue(Children.Count);
            writer.WriteEndAttribute();
            if (Children.Count > 0)
            {
                writer.WriteStartElement(nameof(Children));
                foreach (var ch in Children)
                {
                    writer.WriteValue(ch);
                }
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
