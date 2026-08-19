// ***********************************************************************
// Assembly         : MauiApp1
// Author           : Mir
// Created          : 08-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="MainApplication.cs" company="MauiApp1">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Android.App;
using Android.Runtime;

namespace MauiApp1
{
    /// <summary>
    /// Class MainApplication.
    /// Implements the <see cref="Microsoft.Maui.MauiApplication" />
    /// </summary>
    /// <seealso cref="Microsoft.Maui.MauiApplication" />
    [Application]
    public class MainApplication : MauiApplication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainApplication"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        /// <summary>
        /// Creates the maui application.
        /// </summary>
        /// <returns>MauiApp.</returns>
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}