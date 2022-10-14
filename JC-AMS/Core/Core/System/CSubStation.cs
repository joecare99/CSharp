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
using JCAMS.Core.DataOperations;
using JCAMS.Core.System.Values;
using System;
using System.Collections.Generic;
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
    public class CSubStation : CPropNotificationClass, IHasParent , IHasID , IXmlSerializable, IHasDescription
    {
        #region Properties
        #region private properties
        private string _Description;
        #endregion
        /// <summary>
        /// The identifier substation
        /// </summary>
        public long idSubStation;

        /// <summary>
        /// The description
        /// </summary>
        public string Description { get=>_Description; private set => SetValue(value, ref _Description); }

        /// <summary>
        /// The identifier station
        /// </summary>
        public long idStation { get => Station?.idStation ?? -1; }
        public CStation Station { get; private set; }

        /// <summary>The values of this instance. Contains ALL values of this instance (flat).</summary>
        public Dictionary<string, CSystemValue> Values = new Dictionary<string, CSystemValue>();

        #region static properties
        // Some "well known" SubStations
        public static CSubStation System { get; private set; }

        public static Dictionary<long, CSubStation> SubStations = new Dictionary<long, CSubStation>();
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
            SubStations[idSubStation] = this;
        }

        #region Interface methods
        public CSubStation(SerializationInfo info, StreamingContext context):
            this(CStation.GetStation(info.GetInt64(nameof(idStation))), 
                info.GetInt64(nameof(idSubStation)),
                info.GetString(nameof(Description)))
        {
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(idStation), idStation, typeof(long));           
            info.AddValue(nameof(idSubStation), idSubStation, typeof(long));
            info.AddValue(nameof(Description), Description, typeof(string));
        }

        public XmlSchema? GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
//            _idStation = reader.AttributeValue(nameof(idStation));
//            Station = CStation.GetStation();
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
        #region static Methods
        public static CSubStation GetSubStationGetStation(long idSubStation)
        {
            return SubStations.ContainsKey(idSubStation) ? SubStations[idSubStation] : null;
        }
        #endregion
        #endregion
    }
}
