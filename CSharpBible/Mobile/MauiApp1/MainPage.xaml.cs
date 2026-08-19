// ***********************************************************************
// Assembly         : MauiApp1
// Author           : Mir
// Created          : 08-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="MainPage.xaml.cs" company="MauiApp1">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace MauiApp1
{
    /// <summary>
    /// Class MainPage.
    /// Implements the <see cref="Microsoft.Maui.Controls.ContentPage" />
    /// </summary>
    /// <seealso cref="Microsoft.Maui.Controls.ContentPage" />
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// The count
        /// </summary>
        int count = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the <see cref="E:CounterClicked" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}