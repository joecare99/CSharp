using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System.Runtime.Serialization;

namespace BaseGenClasses.Model
{
    abstract public class GenObject : IGenObject
    {
        [DataMember]
        public Guid UId { get ; init ; }
        [DataMember]
        public EGenType eGenType { get ; init ; }
    }
}