// ***********************************************************************
// Assembly         : PolySpline_net
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 08-23-2022
// ***********************************************************************
// <copyright file="Segment.cs" company="PolySpline_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using System.Windows;

namespace PolySpline.ViewModels
{
    /// <summary>
    /// Class Segment.
    /// </summary>
    public partial class Segment:NotificationObjectCT
    {
#if NET50_OR_GREATER
        public Coordinate? Start { get; set; }
        public Coordinate? Middle { get; set; }
        public Coordinate? End { get; set; }
#else
        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Middle1))]
        private Coordinate _start;
        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Middle1))]
        [NotifyPropertyChangedFor(nameof(Middle2))]
        private Coordinate _middle;
        public Point Middle1 => new Point((Start.X * 0.333 + Middle.X * 0.667) , (Start.Y *0.333+ Middle.Y * 0.667) );
        public Point Middle2 => new Point((End.X * 0.333 + Middle.X * 0.667), (End.Y * 0.333 + Middle.Y * 0.667) );
        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>The end.</value>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Middle2))]
        private Coordinate _end;

        partial void OnEndChanged(Coordinate? oldValue, Coordinate newValue)
        {
            if (newValue != null)
            {
               newValue.PropertyChanged+=(e,a)=>OnPropertyChanged(nameof(Middle2));
            }
        }
        partial void OnStartChanged(Coordinate? oldValue, Coordinate newValue)
        {
            if (newValue != null)
            {
                newValue.PropertyChanged += (e, a) => OnPropertyChanged(nameof(Middle1));
            }
        }
        partial void OnMiddleChanged(Coordinate? oldValue, Coordinate newValue)
        {
            if (newValue != null)
            {
                newValue.PropertyChanged += (e, a) =>
                {
                    OnPropertyChanged(nameof(Middle1));
                    OnPropertyChanged(nameof(Middle2));
                };
            }
        }
#endif
    }
}
