using System;
using System.Drawing;

namespace CSFreeVision.Base
{
    public class TCanvas : ICanvas
    {
        public class _Index2<TValue>
        {
            public _Index2(TCanvas parent)
            {
                _parent = parent;
            }

            private TCanvas _parent;
            public TValue this[int x, int y] { get => (TValue)_parent.GetValue(x, y, typeof(TValue)); set => _parent.SetValue(x, y, value); }

        }

        #region Properties
        #region private Fields
        // private ref Byte[] _ABuffer();
        private VideoCell[] _CBuffer;
        private int _locks;
        private Point _PenPos;
        private Boolean _Clipping;
        private Rectangle _ClipRect;
        private CustomBrush _Brush;
        private CustomPen _Pen;
        private CustomFont _Font;
        private Rectangle _Dimension;
        #endregion
        
        // properties
        public int LockCount => _locks;
        public CustomFont Font { get => _Font; set => SetFont(value); }
        public CustomPen Pen { get => _Pen; set => SetPen(value); }
        public CustomBrush Brush { get => _Brush; set => SetBrush(value); }
        public Rectangle ClipRect { get => _ClipRect; set => SetClipRect(value); }
        public Boolean Clipping { get => _Clipping; set => SetClipping(value); }
        public Point PenPos { get => _PenPos; set => SetPenPos(value); }
        public int Height { get => _Dimension.Height; set => _Dimension.Height=value; }
        public int Width { get => _Dimension.Width; set => _Dimension.Width = value; }
 // Todo?:       ref Byte[] Buffer => _Buffer;

        public _Index2<VideoCell> Pixels;
        public _Index2<TColor> Colors; 
        #endregion

        TCanvas()
        {
            Pixels = new _Index2<VideoCell>(this);
            Colors = new _Index2<TColor>(this);
        }

        #region Setters
        private void SetBrush(CustomBrush value)
        {
            // Todo: Implement this method
            throw new NotImplementedException();
        }


        private void SetFont(CustomFont value)
        {
            // Todo: Implement this method
            throw new NotImplementedException();
        }

        private void SetPen(CustomPen value)
        {
            // Todo: Implement this method
            throw new NotImplementedException();
        }

        private object GetValue(int x, int y, Type t)
        {
            // Todo: Implement this method
            throw new NotImplementedException();
        }

        private void SetValue<TValue>(int x, int y, TValue value)
        {
            // Todo: Implement this method
            throw new NotImplementedException();
        }
        private void SetClipRect(Rectangle value)
        {
            // Todo: Implement this method
            throw new NotImplementedException();
        }

        private void SetClipping(bool value)
        {
            // Todo: Implement this method
            throw new NotImplementedException();
        }
        private void SetPenPos(Point value)
        {
            // Todo: Implement this method
            throw new NotImplementedException();
        }
        #endregion
    }
}
