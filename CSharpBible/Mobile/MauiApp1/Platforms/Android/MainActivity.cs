// ***********************************************************************
// Assembly         : MauiApp1
// Author           : Mir
// Created          : 08-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="MainActivity.cs" company="MauiApp1">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MauiApp1
{
    /// <summary>
    /// Class MainActivity.
    /// Implements the <see cref="Microsoft.Maui.MauiAppCompatActivity" />
    /// </summary>
    /// <seealso cref="Microsoft.Maui.MauiAppCompatActivity" />
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}