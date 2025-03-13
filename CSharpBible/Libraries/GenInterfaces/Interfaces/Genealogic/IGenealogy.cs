﻿using GenFree2Base.Interfaces;
using System;
using System.Collections.Generic;

namespace GenInterfaces.Interfaces.Genealogic
{
    /// <summary>The Interface for a complete Genealogy-Class</summary>
    public interface IGenealogy : IGenBase
    {

        /// <summary>Gets or sets the "get entity function".</summary>
        /// <value>The get entity function. This function should get a new instance of IGenEntity or deliver an existing one due to the parameters</value>
        Func<IList<object>,IGenEntity> GetEntity { get; set; }
        /// <summary>Gets or sets the "get fact/event function".</summary>
        /// <value>The get fact function. This function should get a new instance of IGenFact or deliver an existing one due to the parameters</value>
        Func<IList<object>, IGenFact> GetFact { get; set; }
        /// <summary>Gets or sets the "get source function".</summary>
        /// <value>The get source function. This function should get a new instance of IGenSource or deliver an existing one due to the parameters</value>
        Func<IList<object>, IGenEntity> GetSource { get; set; }
        /// <summary>Gets or sets the "get transaction function".</summary>
        /// <value>The get entity function. This function should get a new instance of IGenTransaction or deliver an existing one due to the parameters</value>
        Func<IList<object>, IGenEntity> GetTransaction { get; set; }
      
        /// <summary>The List of all entities of the genealogy (persons and families).</summary>
        /// <value>The list of entitys.</value>        
        IList<IGenEntity> Entitys { get; init; }
        /// <summary>Gets all sources of the genealogy.</summary>
        /// <value>All sources of the genealogy.</value>
        IList<IGenSources> Sources { get; init; }
        /// <summary>Gets all places of the genealogy.</summary>
        /// <value>All places of the genealogy.</value>
        IList<IGenPlace> Places { get; init; }
        /// <summary>Gets all the transactions done in the genealogy.</summary>
        /// <value>The transactions of the genealogy.</value>
        /// <remarks>The list of transactions is used for undo and syncing.</remarks>
        IList<IGenTransaction> Transactions { get; init; }
    }
}