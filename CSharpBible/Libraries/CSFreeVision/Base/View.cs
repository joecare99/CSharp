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
        Rectangle _dimension;
        /// <summary>
        /// The parent
        /// </summary>
//        private readonly Group _parent;
        /// <summary>
        /// The canvas
        /// </summary>
        private readonly TCanvas _canvas;

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
        public int Top { get => _dimension.Y; set => _SetTop(value); }
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public int Left { get => _dimension.X; set => _SetLeft(value); }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get => _dimension.Width; set => _SetWidth(value); }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get => _dimension.Height; set => _SetHeight(value); }
        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Point Origin { get => _dimension.Location; set => _SetOrigin(value); }
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
//*        public Group Parent { get => _parent; set => _SetParent(value); }
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
        private void _SetParent(Group value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the origin.
        /// </summary>
        /// <param name="value">The value.</param>
        private void _SetOrigin(Point value)
        {
            if (_dimension.Location == value) return;
            _dimension.X = value.X;
            _dimension.Y = value.Y;
        }

        /// <summary>
        /// Sets the top.
        /// </summary>
        /// <param name="value">The value.</param>
        private void _SetTop(int value)
        {
            if (_dimension.Y == value) return;
            _dimension.Y = value;
        }


        /// <summary>
        /// Sets the left.
        /// </summary>
        /// <param name="value">The value.</param>
        private void _SetLeft(int value)
        {
            if (_dimension.X == value) return;
            _dimension.X = value;
        }


        /// <summary>
        /// Sets the width.
        /// </summary>
        /// <param name="value">The value.</param>
        private void _SetWidth(int value)
        {
            if (_dimension.Width == value) return;
            _dimension.Width = value;
        }


        /// <summary>
        /// Sets the height.
        /// </summary>
        /// <param name="value">The value.</param>
        private void _SetHeight(int value)
        {
            if (_dimension.Height == value) return;
            _dimension.Height = value;
        }


    }
}
