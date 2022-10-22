using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSFreeVision.Base
{
    public interface IHasCanvas
    {
        TCanvas Canvas { get; }
    }
    public class TCanvas
    {
       // private ref Byte[] _ABuffer();
        private VideoCell[] _CBuffer;
        int _locks;
        Point _PenPos;
        Boolean _Clipping;
        Rectangle _ClipRect;
        CustomBrush _Brush;
        CustomPen _Pen;
        CustomFont _Font;
        Rectangle _Dimension;

        TCanvas()
        {
            Pixels = new _Index2<VideoCell>(this);
            Colors = new _Index2<TColor>(this);
        }
        // properties
        int LockCount => _locks;
        CustomFont Font { get => _Font; set => SetFont(value); }

        CustomPen Pen { get => _Pen; set => SetPen(value); }

        CustomBrush Brush { get => _Brush; set => SetBrush(value); }

        public class _Index2<TValue>
        {
            public _Index2(TCanvas parent)
            {
                _parent = parent;
            }

            private TCanvas _parent;
            public TValue this[int x,int y] { get => (TValue)_parent.GetValue(x, y, typeof(TValue)); set => _parent.SetValue(x, y, value); }
            
        }


        public _Index2<VideoCell> Pixels;
        public _Index2<TColor> Colors; 
        Rectangle ClipRect { get => _ClipRect; set => SetClipRect(value); }
        Boolean Clipping { get => _Clipping; set => SetClipping(value); }
        Point PenPos { get => _PenPos; set => SetPenPos(value); }
        int Height { get => _Dimension.Height; set => _Dimension.Height=value; }
        int Width { get => _Dimension.Width; set => _Dimension.Width = value; }
        ref Byte[] Buffer => _Buffer;
        private void SetFont(CustomFont value)
        {
            throw new NotImplementedException();
        }

        private void SetPen(CustomPen value)
        {
            throw new NotImplementedException();
        }

        private object GetValue(int x, int y, Type t)
        {
            throw new NotImplementedException();
        }

        private void SetValue<TValue>(int x, int y, TValue value)
        {
            throw new NotImplementedException();
        }

    }
}
