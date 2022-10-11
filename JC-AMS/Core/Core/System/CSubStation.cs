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
using System.Runtime.Serialization;

namespace JCAMS.Core.System
{
    /// <summary>
    /// Class SSubstation.
    /// </summary>
    public class CSubStation : IHasParent , IHasID , ISerializable, IHasDescription
    {
        #region Properties
        /// <summary>
        /// The identifier substation
        /// </summary>
        public int idSubstation;

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

        long IHasID.ID => idSubstation;
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
        public CSubStation(CStation cStation,int idSubstation,string sDescription)
        {
            Station = cStation;
            this.idSubstation = idSubstation;
            Description = sDescription;
        }

        #region Interface properties
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new global::System.NotImplementedException();
        }
        #endregion
        #endregion
    }
}
