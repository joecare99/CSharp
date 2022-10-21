﻿// ***********************************************************************
// Assembly         : Core
// Author           : Mir
// Created          : 10-18-2022
//
// Last Modified By : Mir
// Last Modified On : 10-17-2022
// ***********************************************************************
// <copyright file="CSystemValueDef.cs" company="HP Inc.">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using JCAMS.Core.DataOperations;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

/// <summary>
/// The Values namespace.
/// </summary>
/// <autogeneratedoc />
namespace JCAMS.Core.System.Values
{
    /// <summary>
    /// The defintion of Values
    /// </summary>
    [Serializable]
    public class CSystemValueDef : CPropNotificationClass, IHasDescription, IHasParent, ISerializable , IXmlSerializable, IHasID
    {
        #region Properties
        #region private properties
        /// <summary>
        /// The description
        /// </summary>
        /// <autogeneratedoc />
        private string _Description;
        /// <summary>
        /// The identifier value definition
        /// </summary>
        /// <autogeneratedoc />
        private long _idValueDef;
        #endregion
        /// <summary>
        /// Gets the identifier value definition.
        /// </summary>
        /// <value>The identifier value definition.</value>
        /// <autogeneratedoc />
        public long idValueDef { get => _idValueDef; private set => SetValue(value, ref _idValueDef); }
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description of the Value(def)</value>
        public string Description { get=> _Description; private set=> SetValue(value, ref _Description); }
        /// <summary>
        /// Gets the station.
        /// </summary>
        /// <value>The station of the value-defintion</value>
        public CStation Station { get; private set; }
        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public Type DataType { get; private set; }

        /// <summary>
        /// The structure parent
        /// </summary>
        /// <autogeneratedoc />
        public CSystemValueDef? StructParent;
        /// <summary>
        /// The children
        /// </summary>
        /// <autogeneratedoc />
        public List<CSystemValueDef> Children=new List<CSystemValueDef>();

        /// <summary>
        /// Gets the identifier station.
        /// </summary>
        /// <value>The identifier station.</value>
        /// <autogeneratedoc />
        public long idStation => Station?.idStation ?? -1;
        /// <summary>
        /// Gets a value indicating whether this instance is structure.
        /// </summary>
        /// <value><c>true</c> if this instance is structure; otherwise, <c>false</c>.</value>
        /// <autogeneratedoc />
        public bool IsStruct => Children != null;
        /// <summary>
        /// Gets a value indicating whether this instance is array.
        /// </summary>
        /// <value><c>true</c> if this instance is array; otherwise, <c>false</c>.</value>
        /// <autogeneratedoc />
        public bool IsArray => (MinIndex < MaxIndex);

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <autogeneratedoc />
        object IHasParent.Parent { get => Station; set => Station = value as CStation; }
        /// <summary>
        /// Gets the minimum index.
        /// </summary>
        /// <value>The minimum index.</value>
        /// <autogeneratedoc />
        public int MinIndex { get; private set; }
        /// <summary>
        /// Gets the maximum index.
        /// </summary>
        /// <value>The maximum index.</value>
        /// <autogeneratedoc />
        public int MaxIndex { get; private set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <autogeneratedoc />
        public long ID => throw new NotImplementedException();
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CSystemValueDef"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public CSystemValueDef()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSystemValueDef"/> class.
        /// </summary>
        /// <param name="sDesc">The s desc.</param>
        /// <param name="tDType">Type of the t d.</param>
        /// <param name="station">The station.</param>
        /// <autogeneratedoc />
        public CSystemValueDef(string sDesc,Type tDType,CStation station)
        {
            Description = sDesc;
            DataType = tDType;
            Station = station;
        }

        /// <summary>
        /// Füllt eine <see cref="T:System.Runtime.Serialization.SerializationInfo" /> mit den Daten auf, die zum Serialisieren des Zielobjekts erforderlich sind.
        /// </summary>
        /// <param name="info">Die mit Daten zu füllende <see cref="T:System.Runtime.Serialization.SerializationInfo" />.</param>
        /// <param name="context">Das Ziel (siehe <see cref="T:System.Runtime.Serialization.StreamingContext" />) dieser Serialisierung.</param>
        /// <autogeneratedoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Description), Description, typeof(string));
            info.AddValue(nameof(idStation), idStation, typeof(long));
            info.AddValue(nameof(DataType), Type.GetTypeCode(DataType), typeof(TypeCode));
        }

        /// <summary>
        /// Diese Methode ist reserviert und sollte nicht verwendet werden.
        /// Bei der Implementierung der <see langword="IXmlSerializable" />-Schnittstelle sollte von dieser Methode <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) zurückgegeben werden. Wenn die Angabe eines benutzerdefinierten Schemas erforderlich ist, sollten Sie stattdessen das das <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> auf die Klasse anwenden.
        /// </summary>
        /// <returns>Ein <see cref="T:System.Xml.Schema.XmlSchema" /> zur Beschreibung der XML-Darstellung des Objekts, das von der <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" />-Methode erstellt und von der <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />-Methode verwendet wird.</returns>
        /// <autogeneratedoc />
        public XmlSchema GetSchema() => null;

        /// <summary>
        /// Generiert ein Objekt aus seiner XML-Darstellung.
        /// </summary>
        /// <param name="reader">Der <see cref="T:System.Xml.XmlReader" />-Stream, aus dem das Objekt deserialisiert wird.</param>
        /// <autogeneratedoc />
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToAttribute(nameof(idValueDef));
            idValueDef = reader.ReadContentAsLong();

            reader.MoveToAttribute(nameof(Description));
            Description = reader.ReadContentAsString();

            reader.MoveToAttribute(nameof(idStation));
            var _idStation = reader.ReadContentAsLong();
            Station = CStation.GetStation(_idStation);

            reader.MoveToAttribute(nameof(DataType));
            DataType = Type.GetType($"System.{reader.ReadContentAsString()}");

            reader.MoveToAttribute(nameof(MinIndex));
            MinIndex = reader.ReadContentAsInt();

            reader.MoveToAttribute(nameof(MaxIndex));
            MaxIndex = reader.ReadContentAsInt();

            reader.MoveToAttribute(nameof(Children) + ".Count");
            var _childCount = reader.ReadContentAsInt();

            for (int i = 0; i < _childCount; i++)
            {
                var _svd = reader.ReadContentAsObject();
                if (_svd is CSystemValueDef svd)
                   Children.Add(svd);
            }
        }

        /// <summary>
        /// Konvertiert ein Objekt in seine XML-Darstellung.
        /// </summary>
        /// <param name="writer">Der <see cref="T:System.Xml.XmlWriter" />-Stream, in den das Objekt serialisiert wird.</param>
        /// <autogeneratedoc />
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute(nameof(idValueDef));
            writer.WriteValue(idValueDef);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Description));
            writer.WriteValue(Description);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(idStation));
            writer.WriteValue(idStation);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(DataType));
            writer.WriteValue(Type.GetTypeCode(DataType).ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(MinIndex));
            writer.WriteValue(MinIndex);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(MaxIndex));
            writer.WriteValue(MaxIndex);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute(nameof(Children)+".Count");
            writer.WriteValue(Children.Count);
            writer.WriteEndAttribute();
            if (Children.Count > 0)
            {
                writer.WriteStartElement(nameof(Children));
                foreach (var ch in Children)
                {
                    writer.WriteValue(ch);
                }
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
