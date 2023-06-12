﻿// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 05-20-2023
//
// Last Modified By : Mir
// Last Modified On : 05-20-2023
// ***********************************************************************
// <copyright file="IoC.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Markup;

/// <summary>
/// The Extension namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM.View.Extension
{
    /// <summary>
    /// Class IoC.
    /// Implements the <see cref="MarkupExtension" />
    /// </summary>
    /// <seealso cref="MarkupExtension" />
    /// <autogeneratedoc />
    public class IoC : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the get req SRV.
        /// </summary>
        /// <value>The get req SRV.</value>
        /// <autogeneratedoc />
        public static Func<Type, object> GetReqSrv { get; set; }=(t)=>throw new NotImplementedException("Please initialize the service first.");
        /// <summary>
        /// Gets or sets the get SRV.
        /// </summary>
        /// <value>The get SRV.</value>
        /// <autogeneratedoc />
        public static Func<Type, object?> GetSrv { get; set; } = (t) => null;

        /// <summary>
        /// Gets the required service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        /// <autogeneratedoc />
        public static T GetRequiredService<T>() => (T)GetReqSrv.Invoke(typeof(T));
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.Nullable&lt;T&gt;.</returns>
        /// <autogeneratedoc />
        public static T? GetService<T>() => (T?)GetSrv.Invoke(typeof(T));
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        /// <autogeneratedoc />
        public Type Type { get; set; } = typeof(object);
        /// <summary>
        /// Gibt bei Implementierung in einer abgeleiteten Klasse ein Objekt zurück, das als Wert der Zieleigenschaft für diese Markuperweiterung bereitgestellt wird.
        /// </summary>
        /// <param name="serviceProvider">Ein Dienstanbieter-Hilfsprogramm, das Dienste für die Markuperweiterung bereitstellen kann.</param>
        /// <returns>Der Objektwert, der für die Eigenschaft festgelegt werden soll, auf die die Erweiterung angewendet wird.</returns>
        /// <autogeneratedoc />
        public override object? ProvideValue(IServiceProvider serviceProvider) => GetReqSrv.Invoke(Type);
    }
}
