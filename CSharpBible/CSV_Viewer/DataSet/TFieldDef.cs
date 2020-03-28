using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBible.CSV_Viewer.DataSet
{
    public class TFieldDef : IEquatable<TFieldDef>
    {
        //
        private string FName = "";
        private Type FType = typeof(String);
        internal object FOwner = null;

        public TFieldDef(string aName, Type aType)
        {
            FName = aName;
            FType = aType;
        } 

        public string FieldName { get => FName; }
        public Type FieldType { get => FType; set => FType = value; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TFieldDef);
        }

        public bool Equals(TFieldDef other)
        {
            return other != null &&
                   FName == other.FName &&
                   EqualityComparer<Type>.Default.Equals(FType, other.FType);
        }

        public override int GetHashCode()
        {
            var hashCode = 1883604371;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FName);
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(FType);
            return hashCode;
        }

        public static bool operator ==(TFieldDef left, TFieldDef right)
        {
            return EqualityComparer<TFieldDef>.Default.Equals(left, right);
        }

        public static bool operator !=(TFieldDef left, TFieldDef right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return string.Format("{0}(\"{1}\", {2})", this.GetType().FullName, this.FName, this.FType.FullName);
        }
    }
}
