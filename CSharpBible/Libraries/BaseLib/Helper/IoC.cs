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
using Microsoft.Extensions.DependencyInjection;
using System;

/// <summary>
/// The Extension namespace.
/// </summary>
namespace BaseLib.Helper;

/// <summary>
/// Class IoC.
/// Implements the <see cref="MarkupExtension" />
/// </summary>
/// <seealso cref="MarkupExtension" />
public class IoC
{
    /// <summary>
    /// Gets or sets the required service.
    /// </summary>
    /// <value>The fully initialized service.</value>
    /// <example>
    ///   <code>using Microsoft.Extensions.DependencyInjection
    /// // Build the DependencyInjection container
    /// var builder = new ServiceCollection();
    /// builder.AddTransient&lt;IInterface, CImplement1&gt;();
    /// builder.AddSingleton&lt;IInterface2, CSingleton&gt;();
    /// IoC.GetReqSrv = builder.BuildServiceProvider().GetRequiredService;</code>
    /// </example>
    public static Func<Type, object> GetReqSrv { get; set; }=(t)=>throw new NotImplementedException("Please initialize the service first.");
    /// <summary>
    /// Gets or sets the requested service (can be null).
    /// </summary>
    /// <value>The initialized service.</value>
    /// <example>
    ///   <code>using Microsoft.Extensions.DependencyInjection
    /// // Build the DependencyInjection container
    /// var builder = new ServiceCollection();
    /// builder.AddTransient&lt;IInterface, CImplement1&gt;();
    /// builder.AddSingleton&lt;IInterface2, CSingleton&gt;();
    /// IoC.GetSrv = builder.BuildServiceProvider().GetService;</code>
    /// </example>
    public static Func<Type, object?> GetSrv { get; set; } = (t) => null;
    /// <summary>
    /// Gets or sets the get scope.
    /// </summary>
    /// <value>The get scope.</value>
    public static Func<IServiceScope> GetScope { get; set; } = () => throw new NotImplementedException("Please initialize the service first.");

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    /// <summary>
    /// The scope
    /// </summary>
    private static IServiceScope _Scope;
    /// <summary>
    /// The base scope
    /// </summary>
    private static IServiceScope _BaseScope;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    /// <summary>
    /// Gets the scope.
    /// </summary>
    /// <value>The scope.</value>
    public static IServiceScope Scope => _Scope;
    /// <summary>
    /// Gets the required service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T, the initialized service(class)</returns>
    public static T GetRequiredService<T>() => (T)GetReqSrv.Invoke(typeof(T));
    /// <summary>
    /// Gets the service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>System.Nullable&lt;T&gt;, the initialized service(class)</returns>
    public static T? GetService<T>() => (T?)GetSrv.Invoke(typeof(T));

    /// <summary>Configures the <see cref="T:MVVM.View.Extension.IoC" /> class with the specified <see cref="IServiceProvider" /> sp.</summary>
    /// <param name="sp">The sp.</param>
    /// <example>
    ///   <code>using Microsoft.Extensions.DependencyInjection
    /// // Build the DependencyInjection container
    /// var builder = new ServiceCollection()
    ///   .AddTransient&lt;IInterface, CImplement1&gt;()
    ///   .AddSingleton&lt;IInterface2, CSingleton&gt;();
    /// IoC.Configure(builder.BuildServiceProvider());</code>
    /// </example>
    public static void Configure(IServiceProvider sp)
    {
        GetScope = sp.CreateScope;
        _Scope = _BaseScope = GetScope();
        GetReqSrv = (t) => { var s = sp.GetService(t); return s ?? throw new InvalidOperationException($"No service for {t}"); };
        GetSrv = sp.GetService;
    }

    /// <summary>
    /// Gets the new scope.
    /// </summary>
    /// <param name="aScope">a scope.</param>
    /// <returns><see cref="IServiceScope" />.</returns>
    public static IServiceScope GetNewScope(IServiceScope? aScope=null)
    {
        return _Scope = aScope ==null? GetScope(): aScope.ServiceProvider.CreateScope();
    }

    /// <summary>
    /// Sets the current scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    public static void SetCurrentScope(IServiceScope scope)
    {
        var sp = (_Scope = scope).ServiceProvider;
        GetReqSrv = (t) => { var s = sp.GetService(t); return s ?? throw new InvalidOperationException($"No service for {t}"); };
        GetSrv = sp.GetService;
    }
}
