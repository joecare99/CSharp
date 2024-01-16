using System.Collections.Generic;

namespace MdbBrowser.Models
{
    //    public record DBMetaData(string Name, EKind Kind, IEnumerable<string> Data);
    public struct DBMetaData
    {
        public string Name { get; set; }
        public EKind Kind { get; set; }
        public object This;
        public IEnumerable<string> Data { get; set; }
        public DBMetaData(string Name, EKind Kind, object This, IEnumerable<string> Data) 
        {
            this.Name = Name;
            this.Kind = Kind;
            this.This = This;
            this.Data = Data;
        }
    }
}
