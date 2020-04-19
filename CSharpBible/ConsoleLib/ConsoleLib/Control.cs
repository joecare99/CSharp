using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ConsoleLib
{
    /// <summary>This is the basic class of all TextControls</summary>
    public class Control 
    {
        protected Rectangle _dimension;
        private bool _active;
        private bool _valid;
        private bool _shaddow;
        private bool _visible = true;
        public static Stack<(Action<object, EventArgs>, object, EventArgs)> MessageQueue { get;  set; }

        public Rectangle dimension
        {
            get => _dimension;
            set
            {
                if (_dimension == value) return;
                Rectangle _lastdim = _dimension;
                _dimension = value;
                HandleControlMove(_lastdim);
                if (_lastdim.Location != _dimension.Location)
                    OnMove?.Invoke(this, null);
                if (_lastdim.Size != _dimension.Size)
                    OnResize?.Invoke(this, null);
            }
        }

        public Point position
        {
            get => _dimension.Location;
            set
            {
                if (_dimension.Location == value) return;
                Rectangle _lastdim = _dimension;
                _dimension.Location = value;
                HandleControlMove(_lastdim);
                OnMove?.Invoke(this, null);
            }
        }

        public Size size
        {
            get => _dimension.Size; set
            {
                if (_dimension.Size == value) return;
                Rectangle _lastdim = _dimension;
                _dimension.Size = value;
                HandleControlMove(_lastdim);
                OnResize?.Invoke(this, null);
            }
        }

        private void HandleControlMove(Rectangle _lastdim)
        {
            if (parent == null)
            {
                // Todo: Restore From Background
                ConsoleFramework.Canvas.FillRect(_lastdim, ConsoleFramework.Canvas.ForegroundColor, ConsoleFramework.Canvas.BackgroundColor, ConsoleFramework.chars[4]);
            }
            else
            {
                _lastdim.Location = Point.Add(_lastdim.Location, (Size)parent.position);
                parent.ReDraw(_lastdim);
            }
            if (IsVisible)
            {
                Invalidate();
            }
        }

        public Rectangle realDim => realDimOf(_dimension);

        public Rectangle realDimOf(Rectangle aDim)
        {
            var result = aDim;
            if (parent != null)
            {
                result.Offset(parent.realDim.Location);
            }
            return result;
        }

        public Rectangle localDimOf(Rectangle aDim,Control ancestor = null)
        {
            var result = aDim;
            result.Location = Point.Subtract(result.Location,(Size)_dimension.Location);
            if (parent != null && parent != ancestor)
            {
                result = parent.localDimOf(result, ancestor);                 
            }
            return result;
        }

        public virtual void ReDraw(Rectangle dimension)
        {
            if (_visible && dimension.IntersectsWith(_dimension))
            {
                Draw();
                _valid = true;
            }
        }
        public bool Over(Point M) => realDim.Contains(M);

        public bool active
        {
            get => _active; set
            {
                if (!_visible || _active == value) return;
                if (parent != null)
                {
                    if (!value)
                        parent.ActiveControl = null;
                    else
                    {
                        parent.ActiveControl.active = false;
                        parent.ActiveControl = this;
                    }
                }
                _active = value;
                if (value)
                    OnActivate?.Invoke(this, null);
                OnChange?.Invoke(this, null);
                Invalidate();
            }
        }

        public bool visible
        {
            get => _visible; set
            {
                if (_visible == value) return;
                if (!value)
                {
                    active = false;
                }
                _visible = value;
                if (value)
                OnChange?.Invoke(this, null);
                parent?.Invalidate();
                Invalidate();
            }
        }

        public bool IsVisible => _visible && (parent?.IsVisible ?? true);

        public bool shaddow
        {
            get => _shaddow; set
            {
                if (_shaddow == value) return;
                _shaddow = value;
                parent?.Invalidate();
            }
        }
        public bool valid
        {
            get => _valid; set
            {
                if (_valid == value) return;
                if (!value)
                {
                    Invalidate();
                }
                else
                {
                    _valid = value;
                }
            }
        }

        public void Invalidate()
        {
            _valid = false;
            MessageQueue?.Push((DoRedraw, this, null));
        }

        private void DoRedraw(object sender, EventArgs a)
        {
            if (!_valid)
                ReDraw(((Control)sender).dimension);
        }

        public event EventHandler OnClick;
        public event EventHandler OnMove;
        public event EventHandler OnResize;
        public event EventHandler OnChange;
        public event EventHandler OnActivate;
        public event EventHandler OnMouseEnter;
        public event EventHandler OnMouseLeave;
        public event EventHandler<MouseEventArgs> OnMouseMove;

        public ConsoleColor BackColor;
        public ConsoleColor ForeColor;

        public string Text { get => _text; set => SetText(value); }

        public virtual void SetText(string value)
        {
            if (_text == value) return;
            _text = value;
            OnChange?.Invoke(this, null);
            Invalidate();
        }

        public List<Control> children = new List<Control>();
        public Control ActiveControl;
        private Control _parent;
        private string _text;

        public Control parent { get => _parent; set => SetParent(value); }

        private void SetParent(Control value)
        {
            if (_parent == value) return;
            var oldPar = _parent;
            _parent = value;
            oldPar?.Remove(this);
            value?.Add(this);
        }

        public Control Add(Control control)
        {
            if (control.parent != this)
               control.parent?.Remove(control);
            if (control.parent == null)
            {
                children.Add(control);
                control.parent = this;
            }
            else if (!children.Contains(control))
                children.Add(control);

            return this;
        }

        public Control Remove(Control control)
        {
            control.parent = null;
            children.Remove(control);
            return this;
        }

        public virtual void Draw()
        {
            // Draw Background
            Console.SetCursorPosition(realDim.X, realDim.Y);
            Console.ForegroundColor = ForeColor;
            Console.BackgroundColor = BackColor;
            Console.Write($"[{Text}]");
            Console.BackgroundColor = ConsoleColor.Black;            
        }

        public virtual void Click()
        {
            OnClick?.Invoke(this, null);
        }

        public virtual void MouseEnter(Point M)
        {
            OnMouseEnter?.Invoke(this, null);
            foreach (var ctrl in children)
            {
                if (ctrl.Over(M))
                    ctrl.MouseEnter(M);
            }
        }
        public virtual void MouseLeave(Point M)
        {
            OnMouseLeave?.Invoke(this, null);
            foreach (var ctrl in children)
            {
                if (ctrl.Over(M))
                    ctrl.MouseLeave(M);
            }
        }

        public virtual void MouseMove(MouseEventArgs M, Point lastMousePos)
        {
            OnMouseMove?.Invoke(this, M);
            foreach (var ctrl in children)
            {
                bool xoHit = ctrl.Over(lastMousePos);
                bool xnHit = ctrl.Over(M.Location);
                if (xoHit && !xnHit)
                    ctrl.MouseLeave(lastMousePos);
                // Invoke Mouse Leave
                if (!xoHit && xnHit)
                    ctrl.MouseEnter(M.Location);
                // Invoke Mouse Enter
                if (xoHit && xnHit)
                    ctrl.MouseMove(M, lastMousePos);
            }
        }
        public virtual void MouseClick(MouseEventArgs M)
        {
            bool xFlag = false;
            foreach (var ctrl in children)
            {
                if (ctrl.Over(M.Location))
                {
                    xFlag = true;
                    ctrl.MouseClick(M);
                }
            }
            if (!xFlag && M.Button== MouseButtons.Left)
                OnClick?.Invoke(this, null);

        }

        public virtual void DoUpdate()
        {
            foreach (var ctrl in children)
            {
                ctrl.DoUpdate();
            }
        }

    }
}
