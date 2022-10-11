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
using System;
using System.Collections.Generic;
using System.Linq;

namespace JCAMS.Core.System
{
    /// <summary>
    /// Class Station. Logical station of the System
    /// </summary>
    public class CStation : IHasID , IHasDescription
    {
        #region Properties
        /// <summary>
        /// The identifier station
        /// </summary>
        public long idStation { get; private set; }

        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; private set; }

        public Dictionary<string,CSubStation> aSubStations = new Dictionary<string, CSubStation>();

        public CSubStation this[int i] => aSubStations.Values.Count > i ? aSubStations.Values.ToArray()[i]:null;
        public CSubStation this[string s] { get => aSubStations.ContainsKey(s)? aSubStations[s]:null; set => aSubStations[s] = value; }
        long IHasID.ID => idStation;

        #region static properties
        public static event EventHandler<long> OnNewStation;  
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
        /// <summary>
        /// Initializes a new instance of the <see cref="CStation" /> class.
        /// </summary>
        /// <param name="Q">The q.</param>
        public CStation(long lID,string sDescription="")
        {
            idStation = lID;
            Description = sDescription;
            OnNewStation?.Invoke(this,lID);
        }
        #endregion
    }
}
