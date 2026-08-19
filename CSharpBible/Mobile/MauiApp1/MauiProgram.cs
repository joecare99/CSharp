// ***********************************************************************
// Assembly         : MauiApp1
// Author           : Mir
// Created          : 08-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="MauiProgram.cs" company="MauiApp1">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace MauiApp1
{
    /// <summary>
    /// Class MauiProgram.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates the maui application.
        /// </summary>
        /// <returns>MauiApp.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            return builder.Build();
        }
    }
}