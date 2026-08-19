// ***********************************************************************
// Assembly         : MauiApp1
// Author           : Mir
// Created          : 08-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="MauiApp1">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace MauiApp1
{
    /// <summary>
    /// Class App.
    /// Implements the <see cref="Microsoft.Maui.Controls.Application" />
    /// </summary>
    /// <seealso cref="Microsoft.Maui.Controls.Application" />
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <Docs>
        ///   <summary>Initializes a new <see cref="T:Microsoft.Maui.Controls.Application" /> instance.</summary>
        ///   <remarks>To be added.</remarks>
        /// </Docs>
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}