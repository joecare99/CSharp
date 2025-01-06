using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace WinAhnenCls.Model.HejInd
{
    public class CHejIndiData : IGenIndividual
    {
        private Dictionary<EHejIndDataFields, object> _data = new();

        public int ID { get => (int)_data[EHejIndDataFields.hind_ID]; set => _data[EHejIndDataFields.hind_ID] = value; }
        public int idFather { get => (int)_data[EHejIndDataFields.hind_idFather]; set => _data[EHejIndDataFields.hind_idFather] = value; }
        public int idMother { get => (int)_data[EHejIndDataFields.hind_idMother]; set => _data[EHejIndDataFields.hind_idMother] = value; }

        public string FamilyName { get => (string)_data[EHejIndDataFields.hind_FamilyName]; set => _data[EHejIndDataFields.hind_FamilyName] = value; }
        public string GivenName { get => (string)_data[EHejIndDataFields.hind_GivenName]; set => _data[EHejIndDataFields.hind_GivenName] = value; }
        public string Sex { get => (string)_data[EHejIndDataFields.hind_Sex]; set => _data[EHejIndDataFields.hind_Sex] = value; }

        //public string BirthDate { get => (string)_data[EHejIndDataFields.hind_BirthDate]; set => _data[EHejIndDataFields.hind_BirthDate] = value; }
        public string BirthPlace { get => (string)_data[EHejIndDataFields.hind_BirthPlace]; set => _data[EHejIndDataFields.hind_BirthPlace] = value; }
        //public string BaptDate { get => (string)_data[EHejIndDataFields.hind_BaptDate]; set => _data[EHejIndDataFields.hind_BaptDate] = value; }
        public string BaptPlace { get => (string)_data[EHejIndDataFields.hind_BaptPlace]; set => _data[EHejIndDataFields.hind_BaptPlace] = value; }
        //public string DeathDate { get => (string)_data[EHejIndDataFields.hind_DeathDate]; set => _data[EHejIndDataFields.hind_DeathDate] = value; }
        public string DeathPlace { get => (string)_data[EHejIndDataFields.hind_DeathPlace]; set => _data[EHejIndDataFields.hind_DeathPlace] = value; }
        //public string BurialDate { get => (string)_data[EHejIndDataFields.hind_BurialDate]; set => _data[EHejIndDataFields.hind_BurialDate] = value; }
        public string BurialPlace { get => (string)_data[EHejIndDataFields.hind_BurialPlace]; set => _data[EHejIndDataFields.hind_BurialPlace] = value; }


        public CHejIndiData()
        {
            Data = new _HejIndiDataProxy(this);
            Clear();
        }

        public int[] Marriages { get; set; }
        public _HejIndiDataProxy Data { get; }

        public void Append(object self)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            foreach (EHejIndDataFields i in Enum.GetValues(typeof(EHejIndDataFields)))
            {
                if ((int)i <= (int)EHejIndDataFields.hind_idMother)
                    _data[i] = 0;
                else
                    _data[i] = "";
            }
        }

        public class _HejIndiDataProxy
        {
            public object this[EHejIndDataFields index] { get => This.GetProperty(index); set => This.SetProperty(index, value); }
            //   public string this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            private CHejIndiData This { get; }
            public _HejIndiDataProxy(CHejIndiData This)
            {
                this.This = This;
            }
        }

        private void SetProperty(EHejIndDataFields index, object value)
        {
            _data[index] = value;
        }

        private object GetProperty(EHejIndDataFields index)
        {
            return _data[index];
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            bool _result = false;
            result = null;
            try
            {
                if (_result = Enum.TryParse($"hind_{binder.Name}", out EHejIndDataFields field))
                    result = Data[field];
            }
            finally { }
            return _result;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new NotImplementedException();
        }

        public bool ReadFromStream(StreamReader sr)
        {
            var _result = true;
            if (sr.EndOfStream || (char)(sr.Peek()) == 'm') // 'mrt
                return false;
            var _line = sr.ReadLine().Split('');
            for (int i = 0; i < _line.Length; i++)
            {
                var field = (EHejIndDataFields)(i-1); 
                if (int.TryParse(_line[i], out int _int))
                    Data[field] = _int;
                else
                    Data[field] = _line[i];
            }
            return _result;
        }
    }
}