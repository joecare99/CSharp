// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-18-2022
// ***********************************************************************
// <copyright file="SSubstation.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JCAMS.Core.System
{
    /// <summary>
    /// Class SSubstation.
    /// </summary>
    [Serializable]
    public class CSubStation : IHasParent , IHasID , IXmlSerializable, IHasDescription
    {
        #region Properties
        /// <summary>
        /// The identifier substation
        /// </summary>
        public long idSubStation;

        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The identifier station
        /// </summary>
        public long idStation { get => Station?.idStation ?? -1; }
        public CStation Station { get; private set; }

        #region static properties
        // Some "well known" SubStations
        public static CSubStation System { get; private set; }
        #endregion

        #region Interface properties
        object IHasParent.Parent { get => Station; set { if (value is CStation cs) Station = cs; } }

        long IHasID.ID => idSubStation;
        #endregion
        #endregion
        #region Methods
        static CSubStation()
        {
            System = new CSubStation(CStation.System, 0, "System");
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CSubStation" /> class.
        /// </summary>
        /// <param name="Q">The q.</param>
        public CSubStation(CStation cStation, long idSubstation,string sDescription)
        {
            Station = cStation;
            this.idSubStation = idSubstation;
            Description = sDescription;
        }

        #region Interface methods
        public CSubStation(SerializationInfo info, StreamingContext context)
        {
            idSubStation = info.GetInt64(nameof(idSubStation));
            Description = info.GetString(nameof(Description));
            var _idStation = info.GetInt64(nameof(idStation));
            Station = CStation.GetStation(_idStation);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(idSubStation), idSubStation, typeof(long));
            info.AddValue(nameof(Description), Description, typeof(string));
            info.AddValue(nameof(idStation), idStation, typeof(long));           
        }

        public void CompleteDeserialization(object deserialized)
        {
            throw new global::System.NotImplementedException();
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute(nameof(idSubStation));
            writer.WriteValue(idSubStation);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Description));
            writer.WriteValue(Description);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(idStation));
            writer.WriteValue(idStation);
            writer.WriteEndAttribute();

        }
        #endregion
        #endregion
    }
}
