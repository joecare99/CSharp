using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaseGenClasses.Model
{
    [JsonDerivedType(typeof(GenObject), typeDiscriminator: "base")]

    public abstract class GenObject : IGenObject
    {
        private Guid? _uid;

        [DataMember]
        public Guid UId { get => _uid ??= Guid.NewGuid(); init => _uid = value; }
        [DataMember]
        public abstract EGenType eGenType { get ; }
 
        public DateTime? LastChange { get; protected set; }
    }
}