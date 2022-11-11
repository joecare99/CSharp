// ***********************************************************************
// Assembly         : CSFreeVision
// Author           : Mir
// Created          : 06-16-2022
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="View.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using System.ComponentModel;

/// <summary>
/// The Base namespace.
/// </summary>
namespace CSFreeVision.Base
{
    /// <summary>
    /// Class View.
    /// Implements the <see cref="Component" />
    /// Implements the <see cref="IComponent" />
    /// </summary>
    /// <seealso cref="Component" />
    /// <seealso cref="IComponent" />
    public class View : Component, IComponent
    {
        /// <summary>
        /// The dimension
        /// </summary>
        Rectangle _Dimension;
        /// <summary>
        /// The parent
        /// </summary>
        Group _Parent;
        /// <summary>
        /// The canvas
        /// </summary>
        private TCanvas _canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="View" /> class.
        /// </summary>
        public View()
        {
          //  InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="View" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public View(IContainer container)
        {
            container.Add(this);

          //  InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public int Top { get => _Dimension.Y; set => SetTop(value); }
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public int Left { get => _Dimension.X; set => SetLeft(value); }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get => _Dimension.Width; set => SetWidth(value); }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get => _Dimension.Height; set => SetHeight(value); }
        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Point Origin { get => _Dimension.Location; set => SetOrigin(value); }
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Group Parent { get => _Parent; set => SetParent(value); }
        /// <summary>
        /// Gets the canvas.
        /// </summary>
        /// <value>The canvas.</value>
        public TCanvas Canvas { get => _canvas; }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SetParent(Group value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the origin.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetOrigin(Point value)
        {
            if (_Dimension.Location == value) return;
            _Dimension.X = value.X;
            _Dimension.Y = value.Y;
        }

        /// <summary>
        /// Sets the top.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetTop(int value)
        {
            if (_Dimension.Y == value) return;
            _Dimension.Y = value;
        }


        /// <summary>
        /// Sets the left.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetLeft(int value)
        {
            if (_Dimension.X == value) return;
            _Dimension.X = value;
        }


        /// <summary>
        /// Sets the width.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetWidth(int value)
        {
            if (_Dimension.Width == value) return;
            _Dimension.Width = value;
        }


        /// <summary>
        /// Sets the height.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetHeight(int value)
        {
            if (_Dimension.Height == value) return;
            _Dimension.Height = value;
        }


    }
}
