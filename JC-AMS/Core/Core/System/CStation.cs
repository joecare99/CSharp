// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-18-2022
// ***********************************************************************
// <copyright file="CStation.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using JCAMS.Core.DataOperations;
using JCAMS.Core.Extensions;
using JCAMS.Core.System.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JCAMS.Core.System
{
    /// <summary>
    /// Class Station. Logical station of the System
    /// </summary>
    public class CStation : CPropNotificationClass, IHasID , IHasDescription, ISerializable, IXmlSerializable
    {
        #region Properties
        #region private Properties
        private string _Description;
        private long _idStation;
        #endregion
        /// <summary>
        /// The identifier station
        /// </summary>
        public long idStation { get => _idStation; private set => SetValue(value,ref _idStation, OnChangeIdStation); }


        /// <summary>
        /// The description
        /// </summary>
        public string Description { get => _Description; private set => SetValue(value,ref _Description); }

        public Dictionary<string,CSubStation> SubStations = new Dictionary<string, CSubStation>();
        public Dictionary<string, CSystemValueDef> ValueDefs = new Dictionary<string, CSystemValueDef>();

        public CSubStation this[int i] => SubStations.Values.Count > i ? SubStations.Values.ToArray()[i]:null;
        public CSubStation this[string s] { get => SubStations.ContainsKey(s)? SubStations[s]:null; set => SubStations[s] = value; }
        public CSystemValueDef ValDef(int i) => ValueDefs.Values.Count > i ? ValueDefs.Values.ToArray()[i] : null;
        public CSystemValueDef ValDef(string s) => ValueDefs.ContainsKey(s) ? ValueDefs[s] : null;
        long IHasID.ID => idStation;

        #region static properties
        public static event EventHandler<long> OnNewStation=default;

        protected static Dictionary<long, CStation> Stations = new Dictionary<long, CStation>();

        // Some "well known" Stations
        public static CStation System { get; private set; }
        #endregion
        #endregion
        #region Methods
        static CStation()
        {
            // Initialize some few! "well-known" Stations
            System = new CStation(0, "System");    
        }
        private void OnChangeIdStation(long oldId, long newId, string name)
        {
            if (oldId > 0)
                RemoveStation(oldId);
            if (newId > 0)
                Stations[newId] = this;
        }

        public static CStation GetStation(long idStation)
        {
            return Stations.ContainsKey(idStation) ? Stations[idStation]:null;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(idStation), idStation, typeof(long));
            info.AddValue(nameof(Description), Description, typeof(string));

            info.AddValue(nameof(ValueDefs)+"."+nameof(ValueDefs.Count), ValueDefs.Count, typeof(long));
            for (int i = 0; i < ValueDefs.Count; i++)
                info.AddValue(nameof(ValueDefs)+"."+nameof(ValueDefs.Values)+$"[{i}]", ValueDefs.Values.ToArray()[i], typeof(CSystemValueDef));

            info.AddValue(nameof(SubStations) + "." + nameof(SubStations.Count), SubStations.Count, typeof(long));
            for (int i = 0; i < SubStations.Count; i++)
                info.AddValue(nameof(SubStations) + "." + nameof(SubStations.Values) + $"[{i}]", SubStations.Values.ToArray()[i], typeof(CSubStation));
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToAttribute(nameof(idStation));
            var _idStation = reader.ReadContentAsLong();
            idStation = _idStation;

            reader.MoveToAttribute(nameof(Description));
            Description = reader.ReadContentAsString();

            reader.MoveToAttribute(nameof(ValueDefs) + "." + nameof(ValueDefs.Count));
            var _idValDefCount = reader.ReadContentAsInt();           

            reader.MoveToAttribute(nameof(SubStations) + "." + nameof(SubStations.Count));
            var _idSubStCount = reader.ReadContentAsInt();

            reader.ReadToFollowing(nameof(ValueDefs));
            for (int i = 0; i < _idValDefCount; i++)
            {
                reader.ReadToFollowing(nameof(CSystemValueDef));
                var _svd = new CSystemValueDef();
                _svd.ReadXml(reader);
                if (_svd is CSystemValueDef svd)
                    ValueDefs.Add(svd.Description,svd);
            }

            reader.ReadToFollowing(nameof(SubStations));
            for (int i = 0; i < _idSubStCount; i++)
            {
                reader.ReadToFollowing(nameof(CSubStation));
                var _sst = new CSubStation();
                _sst.ReadXml(reader);
                if (_sst is CSubStation sst)
                    SubStations.Add(sst.Description, sst);
            }

        }

        private static void RemoveStation(long idStation)
        {
            if (Stations.ContainsKey(idStation))
                Stations.Remove(idStation);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute(nameof(idStation));
            writer.WriteValue(idStation);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Description));
            writer.WriteValue(Description);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(ValueDefs)+"."+nameof(ValueDefs.Count));
            writer.WriteValue(ValueDefs.Count);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(SubStations) + "." + nameof(SubStations.Count));
            writer.WriteValue(SubStations.Count);
            writer.WriteEndAttribute();

            writer.WriteStartElement(nameof(ValueDefs));
            var aVD = ValueDefs.Values.ToArray();
            for (int i = 0; i < ValueDefs.Count; i++)
            {
                writer.WriteStartElement(aVD[i].GetType().Name);
                if (aVD[i] is IXmlSerializable ix)
                  ix.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(SubStations));
            var aSS = SubStations.Values.ToArray();
            for (int i = 0; i < SubStations.Count; i++)
            {
                writer.WriteStartElement(aSS[i].GetType().Name);
                if (aSS[i] is IXmlSerializable ix)
                    ix.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public CStation()
        {
            idStation = Stations.Keys.Max()+1;
            Description = "";
            Stations[idStation] = this;
            OnNewStation?.Invoke(this, idStation);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CStation" /> class.
        /// </summary>
        /// <param name="lID">The ID</param>
        /// <param name="sDescription">The Description</param>
        public CStation(long lID, string sDescription = "")
        {
            idStation = lID;
            Description = sDescription;
            Stations[lID] = this;
            OnNewStation?.Invoke(this, lID);
        }
        public CStation(SerializationInfo info, StreamingContext context)
        {
            idStation = info.GetInt64(nameof(idStation));
            Description = info.GetString(nameof(Description));
            Stations[idStation] = this;
            ValueDefs = (Dictionary<string, CSystemValueDef>)info.GetValue(nameof(ValueDefs), typeof(Dictionary<string, CSystemValueDef>));
            SubStations = (Dictionary<string, CSubStation>)info.GetValue(nameof(SubStations), typeof(Dictionary<string, CSubStation>));
        }
        #endregion
    }
}
