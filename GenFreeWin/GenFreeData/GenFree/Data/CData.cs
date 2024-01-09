// ***********************************************************************
// Assembly         : GenFreeData
// Author           : Mir
// Created          : 01-09-2024
//
// Last Modified By : Mir
// Last Modified On : 01-09-2024
// ***********************************************************************
// <copyright file="CData.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenFree.Data;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;

/// <summary>
/// The GenFree namespace.
/// </summary>
namespace GenFree
{
    /// <summary>
    /// Class CData.
    /// Implements the <see cref="IHasPropEnum{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IHasPropEnum{T}" />
    public abstract class CData<T> : IHasPropEnum<T>
        where T : Enum
    {
        /// <summary>
        /// The changed props list
        /// </summary>
        protected List<T> _changedPropsList = new();
        /// <summary>
        /// Gets the changed props.
        /// </summary>
        /// <value>The changed props.</value>
        public IReadOnlyList<T> ChangedProps => _changedPropsList;

        /// <summary>
        /// Adds the changed property.
        /// </summary>
        /// <param name="prop">The property.</param>
        public void AddChangedProp(T prop)
        {
            if (!_changedPropsList.Contains(prop))
                _changedPropsList.Add(prop);
        }

        /// <summary>
        /// Clears the changed props.
        /// </summary>
        public void ClearChangedProps()
        {
            _changedPropsList.Clear();
        }

        /// <summary>
        /// Equalses the property.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool EqualsProp(T prop, object? value)
        {
            try {
                Type t = GetPropType(prop);
                return (bool)t.GetMethod("Equals", new[] { t })!.Invoke(GetPropValue(prop), new[] { value })!;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns>Type.</returns>
        public abstract Type GetPropType(T prop);
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns>System.Object.</returns>
        public abstract object GetPropValue(T prop);
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="prop">The property.</param>
        /// <returns>T2.</returns>
        public T2 GetPropValue<T2>(T prop) => (T2)GetPropValue(prop);

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="value">The value.</param>
        public abstract void SetPropValue(T prop, object value);
    }
}