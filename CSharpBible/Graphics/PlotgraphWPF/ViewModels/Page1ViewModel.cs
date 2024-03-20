// ***********************************************************************
// Assembly         : PlotgraphWPF
// Author           : Mir
// Created          : 07-01-2022
//
// Last Modified By : Mir
// Last Modified On : 07-17-2022
// ***********************************************************************
// <copyright file="Page1ViewModel.cs" company="PlotgraphWPF">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlotgraphWPF.Model;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MVVM.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PlotgraphWPF.ViewModels
{
    /// <summary>
    /// Class Page1ViewModel.
    /// </summary>
    public partial class Page1ViewModel: BaseViewModelCT
    {
        /// <summary>
        /// The current second
        /// </summary>
        public int currentSecond = 0;
        /// <summary>
        /// The rd
        /// </summary>
        Random rd = new Random();

        /// <summary>
        /// The lt point
        /// </summary>
        [ObservableProperty]
        private PointCollection _ltPoint = new PointCollection();

        /// <summary>
        /// Gets or sets my model.
        /// </summary>
        /// <value>My model.</value>
        public MyModel myModel { get; set; } = new MyModel();

        /// <summary>
        /// Finalizes an instance of the <see cref="Page1ViewModel"/> class.
        /// </summary>
        public Page1ViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Start();
            myModel = new MyModel()
            {
                points = LtPoint,
                PlotColor = Colors.Blue
            };
        }

        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            currentSecond++;
            double x = currentSecond * 10;
            double y = rd.Next(1, 200);
            LtPoint.Add(new Point(x, y));

            OnPropertyChanged(nameof(myModel));
        }
    }
}
