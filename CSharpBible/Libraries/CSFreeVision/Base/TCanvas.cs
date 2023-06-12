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
         /// <summary>
        /// The dimension
        /// </summary>
        private Rectangle _dimension;
        
#pragma warning disable IDE0051 // Nicht verwendete private Member entfernen
        /// <summary>
        /// The pen position
        /// </summary>
        private Point _penPos;
        /// <summary>
        /// The clip rectangle
        /// </summary>
        private Rectangle _clipRect;
       

        // private ref Byte[] _ABuffer();
        /// <summary>
        /// The c buffer
        /// </summary>
 //       private readonly VideoCell[] _cBuffer;
        /// <summary>
        /// The locks
        /// </summary>
        private readonly int _locks;
        /// <summary>
        /// The clipping
        /// </summary>
        private readonly Boolean _clipping;
        /// <summary>
        /// The brush
        /// </summary>
        private readonly CustomBrush _brush;
        /// <summary>
        /// The pen
        /// </summary>
        private readonly CustomPen _pen;
        /// <summary>
        /// The font
        /// </summary>
        private readonly CustomFont _font;
#pragma warning restore IDE0051 // Nicht verwendete private Member entfernen

        /// <summary>
        /// Prevents a default instance of the <see cref="TCanvas" /> class from being created.
        /// </summary>
        TCanvas()
        {
            pixels = new _Index2<VideoCell>(this);
            colors = new _Index2<TColor>(this);
        }
        // properties
        /// <summary>
        /// Gets the lock count.
        /// </summary>
        /// <value>The lock count.</value>
#pragma warning disable IDE0051 // Nicht verwendete private Member entfernen
        int LockCount => _locks;
        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        CustomFont Font { get => _font; set => _SetFont(value); }

        /// <summary>
        /// Gets or sets the pen.
        /// </summary>
        /// <value>The pen.</value>
        CustomPen Pen { get => _pen; set => _SetPen(value); }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        /// <value>The brush.</value>
        CustomBrush Brush { get => _brush; set => _SetBrush(value); }
#pragma warning restore IDE0051 // Nicht verwendete private Member entfernen

        /// <summary>
        /// Sets the brush.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _SetBrush(CustomBrush value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class _Index2.
        /// </summary>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
#pragma warning disable IDE1006 // Benennungsstile
        public class _Index2<TValue>
#pragma warning restore IDE1006 // Benennungsstile
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
            private readonly TCanvas _parent;
            /// <summary>
            /// Gets or sets the <see cref="TValue" /> with the specified x.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns>TValue.</returns>
            public TValue this[int x,int y] { get => (TValue)_parent._GetValue(x, y, typeof(TValue)); set => _parent._SetValue(x, y, value); }
            
        }


        /// <summary>
        /// The pixels
        /// </summary>
        public _Index2<VideoCell> pixels;
        /// <summary>
        /// The colors
        /// </summary>
        public _Index2<TColor> colors;
        /// <summary>
        /// The buffer
        /// </summary>
        private readonly byte[] _buffer;

        /// <summary>
        /// Gets or sets the clip rect.
        /// </summary>
        /// <value>The clip rect.</value>
#pragma warning disable IDE0051 // Nicht verwendete private Member entfernen
        Rectangle ClipRect { get => _clipRect; set => _SetClipRect(value); }

        /// <summary>
        /// Gets or sets the clipping.
        /// </summary>
        /// <value>The clipping.</value>
        Boolean Clipping { get => _clipping; set => _SetClipping(value); }

        /// <summary>
        /// Gets or sets the pen position.
        /// </summary>
        /// <value>The pen position.</value>
        Point PenPos { get => _penPos; set => _SetPenPos(value); }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        int Height { get => _dimension.Height; set => _dimension.Height = value; }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        int Width { get => _dimension.Width; set => _dimension.Width = value; }
        /// <summary>
        /// Gets the buffer.
        /// </summary>
        /// <value>The buffer.</value>
        Byte[] Buffer => _buffer;
        /// <summary>
        /// Sets the font.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
#pragma warning restore IDE0051 // Nicht verwendete private Member entfernen

        /// <summary>
        /// Sets the clip rect.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _SetClipRect(Rectangle value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the clipping.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _SetClipping(bool value)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Sets the pen position.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _SetPenPos(Point value)
        {
            throw new NotImplementedException();
        }

        private void _SetFont(CustomFont value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the pen.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _SetPen(CustomPen value)
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
        private object _GetValue(int x, int y, Type t)
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
        private void _SetValue<TValue>(int x, int y, TValue value)
        {
            throw new NotImplementedException();
        }

    }
}
