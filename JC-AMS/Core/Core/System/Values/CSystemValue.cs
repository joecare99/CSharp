using JCAMS.Core.DataOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCAMS.Core.System.Values
{
    public class CSystemValue : CPropNotificationClass, IHasDescription, IHasParent, IHasID
    {
        #region Properties
        #region private properties
        private string _Description="";
        private long _idValue=-1;
        private int _Index;
        private CSystemValueDef _valDef;
        #endregion
        public string Description { get => _Description; set => SetValue(value, ref _Description); }

        public CSubStation? SubStation { get; private set; }

        public CSystemValueDef ValueDef => _valDef;
        public int Index => _Index;

        public CSystemValue? StructParent;
        public CSystemValue[] Children;

        public long idValue { get => _idValue; private set => SetValue(value, ref _idValue); }
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
        }

        public CSystemValue(CSystemValueDef valDef,CSubStation sSt,int idx):this()
        {
            _valDef = valDef;
            SubStation = sSt;
            _Index = idx;    
        }
        #endregion
    }
}
