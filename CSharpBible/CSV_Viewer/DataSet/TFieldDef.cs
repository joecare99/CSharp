// ***********************************************************************
// Assembly         : CSV_Viewer
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 04-06-2020
// ***********************************************************************
// <copyright file="TFieldDef.cs" company="JC Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace CSharpBible.CSV_Viewer.CustomDataSet
{
    /// <summary>
    /// Class TFieldDef.
    /// Implements the <see cref="System.IEquatable{CSharpBible.CSV_Viewer.CustomDataSet.TFieldDef}" />
    /// </summary>
    /// <seealso cref="System.IEquatable{CSharpBible.CSV_Viewer.CustomDataSet.TFieldDef}" />
    public class TFieldDef : IEquatable<TFieldDef>
    {
        //
        /// <summary>
        /// The f name
        /// </summary>
        private string FName = "";
        /// <summary>
        /// The f type
        /// </summary>
        private Type FType = typeof(String);
        /// <summary>
        /// The f owner
        /// </summary>
        internal object FOwner = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TFieldDef"/> class.
        /// </summary>
        /// <param name="aName">a name.</param>
        /// <param name="aType">a type.</param>
        public TFieldDef(string aName, Type aType)
        {
            FName = aName;
            FType = aType;
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName { get => FName; }
        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        /// <value>The type of the field.</value>
        public Type FieldType { get => FType; set => FType = value; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Das Objekt, das mit dem aktuellen Objekt verglichen werden soll.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as TFieldDef);
        }

        /// <summary>
        /// Gibt an, ob das aktuelle Objekt gleich einem anderen Objekt des gleichen Typs ist.
        /// </summary>
        /// <param name="other">Ein Objekt, das mit diesem Objekt verglichen werden soll.</param>
        /// <returns><see langword="true" />, wenn das aktuelle Objekt gleich dem <paramref name="other" />-Parameter ist, andernfalls <see langword="false" />.</returns>
        public bool Equals(TFieldDef other)
        {
            return other != null &&
                   FName == other.FName &&
                   EqualityComparer<Type>.Default.Equals(FType, other.FType);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            var hashCode = 1883604371;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FName);
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(FType);
            return hashCode;
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TFieldDef left, TFieldDef right)
        {
            return EqualityComparer<TFieldDef>.Default.Equals(left, right);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TFieldDef left, TFieldDef right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}(\"{1}\", {2})", this.GetType().FullName, this.FName, this.FType.FullName);
        }
    }
}
