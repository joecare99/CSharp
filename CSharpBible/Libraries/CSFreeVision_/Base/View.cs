using System;
using System.Drawing;
using System.ComponentModel;

namespace CSFreeVision.Base
{
    public class View : Component , IView
    {
        #region Properties
        private Rectangle _Dimension;
        private Group _Parent;
        private TCanvas _canvas;
        public int Top { get => _Dimension.Y; set => SetTop(value); }
        public int Left { get => _Dimension.X; set => SetLeft(value); }
        public int Width { get => _Dimension.Width; set => SetWidth(value); }
        public int Height { get => _Dimension.Height; set => SetHeight(value); }
        public Point Origin { get => _Dimension.Location; set => SetOrigin(value); }
        public Group Parent { get => _Parent; set => SetParent(value); }
        public TCanvas Canvas { get => _canvas; set => SetCanvas(value); }

        #endregion
        public View()
        {
            InitializeComponent();
        }

        public View(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected virtual void InitializeComponent() 
        {
            
        }

        private void SetParent(Group value)
        {
            throw new NotImplementedException();
        }

        private void SetOrigin(Point value)
        {
            if (_Dimension.Location == value) return;
            _Dimension.X = value.X;
            _Dimension.Y = value.Y;
        }

        private void SetTop(int value)
        {
            if (_Dimension.Y == value) return;
            _Dimension.Y = value;
        }


        private void SetLeft(int value)
        {
            if (_Dimension.X == value) return;
            _Dimension.X = value;
        }


        private void SetWidth(int value)
        {
            if (_Dimension.Width == value) return;
            _Dimension.Width = value;
        }


        private void SetHeight(int value)
        {
            if (_Dimension.Height == value) return;
            _Dimension.Height = value;
        }

        private void SetCanvas(TCanvas value)
        {
            if (_canvas == value) return;
            _canvas = value;
        }

    }
}
