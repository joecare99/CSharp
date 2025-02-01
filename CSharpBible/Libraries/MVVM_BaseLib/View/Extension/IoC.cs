// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 05-20-2023
//
// Last Modified By : Mir
// Last Modified On : 09-26-2023
// ***********************************************************************
// <copyright file="IoC.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Markup;

/// <summary>
/// The Extension namespace.
/// </summary>
namespace MVVM.View.Extension;

/// <summary>
/// Class IoC.
/// Implements the <see cref="MarkupExtension" />
/// </summary>
/// <seealso cref="MarkupExtension" />
public class IoC2 : MarkupExtension , IIoC
{
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The requested type</value>
    public Type Type { get; set; } = typeof(object);
    /// <summary>
    /// Gibt bei Implementierung in einer abgeleiteten Klasse ein Objekt zurück, das als Wert der Zieleigenschaft für diese Markuperweiterung bereitgestellt wird.
    /// </summary>
    /// <param name="serviceProvider">Ein Dienstanbieter-Hilfsprogramm, das Dienste für die Markuperweiterung bereitstellen kann.</param>
    /// <returns>Der Objektwert, der für die Eigenschaft festgelegt werden soll, auf die die Erweiterung angewendet wird.</returns>
    public override object? ProvideValue(IServiceProvider serviceProvider) => IoC.GetReqSrv.Invoke(Type);

}
