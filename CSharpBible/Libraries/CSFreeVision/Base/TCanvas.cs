// ***********************************************************************
// Assembly         : CSFreeVision
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="TCanvas.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

/// <summary>
/// The Base namespace.
/// </summary>
namespace CSFreeVision.Base
{
    /// <summary>
    /// Interface IHasCanvas
    /// </summary>
    public interface IHasCanvas
    {
        /// <summary>
        /// Gets the canvas.
        /// </summary>
        /// <value>The canvas.</value>
        TCanvas Canvas { get; }
    }
    /// <summary>
    /// Class TCanvas.
    /// </summary>
    public class TCanvas
    {
        // private ref Byte[] _ABuffer();
        /// <summary>
        /// The c buffer
        /// </summary>
        private VideoCell[] _CBuffer;
        /// <summary>
        /// The locks
        /// </summary>
        int _locks;
        /// <summary>
        /// The pen position
        /// </summary>
        Point _PenPos;
        /// <summary>
        /// The clipping
        /// </summary>
        Boolean _Clipping;
        /// <summary>
        /// The clip rect
        /// </summary>
        Rectangle _ClipRect;
        /// <summary>
        /// The brush
        /// </summary>
        CustomBrush _Brush;
        /// <summary>
        /// The pen
        /// </summary>
        CustomPen _Pen;
        /// <summary>
        /// The font
        /// </summary>
        CustomFont _Font;
        /// <summary>
        /// The dimension
        /// </summary>
        Rectangle _Dimension;

        /// <summary>
        /// Prevents a default instance of the <see cref="TCanvas" /> class from being created.
        /// </summary>
        TCanvas()
        {
            Pixels = new _Index2<VideoCell>(this);
            Colors = new _Index2<TColor>(this);
        }
        // properties
        /// <summary>
        /// Gets the lock count.
        /// </summary>
        /// <value>The lock count.</value>
        int LockCount => _locks;
        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        CustomFont Font { get => _Font; set => SetFont(value); }

        /// <summary>
        /// Gets or sets the pen.
        /// </summary>
        /// <value>The pen.</value>
        CustomPen Pen { get => _Pen; set => SetPen(value); }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        /// <value>The brush.</value>
        CustomBrush Brush { get => _Brush; set => SetBrush(value); }

        /// <summary>
        /// Sets the brush.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SetBrush(CustomBrush value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class _Index2.
        /// </summary>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        public class _Index2<TValue>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="_Index2{TValue}" /> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public _Index2(TCanvas parent)
            {
                _parent = parent;
            }

            /// <summary>
            /// The parent
            /// </summary>
            private TCanvas _parent;
            /// <summary>
            /// Gets or sets the <see cref="TValue" /> with the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns>TValue.</returns>
            public TValue this[int x,int y] { get => (TValue)_parent.GetValue(x, y, typeof(TValue)); set => _parent.SetValue(x, y, value); }
            
        }


        /// <summary>
        /// The pixels
        /// </summary>
        public _Index2<VideoCell> Pixels;
        /// <summary>
        /// The colors
        /// </summary>
        public _Index2<TColor> Colors;
        /// <summary>
        /// The buffer
        /// </summary>
        private byte[] _Buffer;

        /// <summary>
        /// Gets or sets the clip rect.
        /// </summary>
        /// <value>The clip rect.</value>
        Rectangle ClipRect { get => _ClipRect; set => SetClipRect(value); }

        /// <summary>
        /// Sets the clip rect.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SetClipRect(Rectangle value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the clipping.
        /// </summary>
        /// <value>The clipping.</value>
        Boolean Clipping { get => _Clipping; set => SetClipping(value); }

        /// <summary>
        /// Sets the clipping.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SetClipping(bool value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the pen position.
        /// </summary>
        /// <value>The pen position.</value>
        Point PenPos { get => _PenPos; set => SetPenPos(value); }

        /// <summary>
        /// Sets the pen position.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SetPenPos(Point value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        int Height { get => _Dimension.Height; set => _Dimension.Height=value; }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        int Width { get => _Dimension.Width; set => _Dimension.Width = value; }
        /// <summary>
        /// Gets the buffer.
        /// </summary>
        /// <value>The buffer.</value>
        Byte[] Buffer => _Buffer;
        /// <summary>
        /// Sets the font.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SetFont(CustomFont value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the pen.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SetPen(CustomPen value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="t">The t.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private object GetValue(int x, int y, Type t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SetValue<TValue>(int x, int y, TValue value)
        {
            throw new NotImplementedException();
        }

    }
}
