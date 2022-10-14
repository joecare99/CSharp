using JCAMS.Core.DataOperations;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using static System.Collections.Specialized.BitVector32;

namespace JCAMS.Core.System.Values
{
    public class CSystemValue : CPropNotificationClass, IHasDescription, IHasParent, IHasID, IXmlSerializable
    {
        #region Properties
        #region private properties
        private long _idValue=-1;
        private string _Description="";
        private CSystemValueDef? _valDef;
        private int _Index;
        private object? _Value;
        #endregion
        public string Description { get => _Description; set => SetValue(value, ref _Description); }

        public object? Value { get => _Value; set => SetValue(value, ref _Value); }

        public CSubStation? SubStation { get; private set; }

        public CSystemValueDef ValueDef => _valDef;
        public int Index => _Index;

        public CSystemValue? StructParent;
        public CSystemValue[]? Children;

        public long idValue { get => _idValue; private set => SetValue(value, ref _idValue); }
        public long idValueDef => ValueDef?.idValueDef ?? -1;
        public long idValue_SParent => StructParent?.idValue ?? -1;
        public long idSubStation => SubStation?.idSubStation ?? -1;

        #region interface properties
        object IHasParent.Parent { get => SubStation; set => SubStation = value as CSubStation ; }

        long IHasID.ID => _idValue;
        #endregion
        #region static properties
        public static Dictionary<long, CSystemValue> Values=new Dictionary<long, CSystemValue>();
        #endregion
        #endregion
        #region Methods
        public CSystemValue()
        {
            _idValue=Values.Keys.Max()+1;
            Values[_idValue]= this;
            _Index = 0;
        }

        public CSystemValue(CSystemValueDef valDef,CSubStation sSt,int idx):this()
        {
            _valDef = valDef;
            SubStation = sSt;
            _Index = idx;    
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            throw new global::System.NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute(nameof(idValue));
            writer.WriteValue(idValue);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Description));
            writer.WriteValue(Description);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(idSubStation));
            writer.WriteValue(idSubStation);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(idValueDef));
            writer.WriteValue(idValueDef);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Value));
            writer.WriteValue(Value);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Index));
            writer.WriteValue(Index);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(idValue_SParent));
            writer.WriteValue(idValue_SParent);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Children)+".Count");
            writer.WriteValue(Children.Count());
            writer.WriteEndAttribute();
        }
        #endregion
    }
}
