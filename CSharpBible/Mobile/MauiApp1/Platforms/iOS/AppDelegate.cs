// ***********************************************************************
// Assembly         : MauiApp1
// Author           : Mir
// Created          : 08-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="AppDelegate.cs" company="MauiApp1">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Foundation;

namespace MauiApp1
{
    /// <summary>
    /// Class AppDelegate.
    /// </summary>
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        /// <summary>
        /// Creates the maui application.
        /// </summary>
        /// <returns>MauiApp.</returns>
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}