using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleLib
{
    public class Control 
    {
        protected Rectangle _dimension;
        private bool _active;
        private bool _shaddow;
        private bool _visible = true;

        public Point position { get => _dimension.Location; 
            set 
            { 
                if (_dimension.Location == value) return;
                Rectangle _lastdim = _dimension;
                _dimension.Location = value;
                if (parent == null)
                {
                    ConsoleFramework.Canvas.FillRect(_lastdim, ConsoleFramework.Canvas.BackgroundColor, ConsoleFramework.chars[4]);
                }
                else
                {
                    parent.ReDraw(_lastdim);
                }
                if (_visible) 
                {
                    Draw();
                } 
                OnMove?.Invoke(this, null);
            }
        }

        public virtual void ReDraw(Rectangle dimension)
        {
            if (_visible && dimension.IntersectsWith(_dimension))
                Draw();
        }

        public Size size
        {
            get => _dimension.Size; set
            {
                if (_dimension.Size == value) return;
                Rectangle _lastdim = _dimension;
                _dimension.Size = value;
                if (parent == null)
                {
                    ConsoleFramework.Canvas.FillRect(_lastdim, ConsoleFramework.Canvas.BackgroundColor, ConsoleFramework.chars[4]);
                }
                else
                {
                    parent.ReDraw(_lastdim);
                }
                if (_visible)
                {
                    Draw();
                }
                OnResize?.Invoke(this, null);
            }
        }
        public bool Over(Point M) => _dimension.Contains(M);

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

        public bool shaddow
        {
            get => _shaddow; set
            {
                if (_shaddow == value) return;
                _shaddow = value;
                parent?.Invalidate();
            }
        }

        private void Invalidate()
        {
            // Todo:
        }

        public event EventHandler OnClick;
        public event EventHandler OnMove;
        public event EventHandler OnResize;
        public event EventHandler OnChange;
        public event EventHandler OnActivate;
        public event EventHandler OnMouseEnter;
        public event EventHandler OnMouseLeave;

        public ConsoleColor BackColor;
        public string Text;

        public List<Control> children = new List<Control>();
        public Control ActiveControl;
        public Control parent { get; private set; }

        public Control Add(Control control)
        {
            control.parent?.Remove(control);
            children.Add(control);
            control.parent = this;
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
            Console.SetCursorPosition(_dimension.X, _dimension.Y);
            Console.BackgroundColor = BackColor;
            Console.Write($"[{Text}]");
            Console.BackgroundColor = ConsoleColor.Black;
            
        }

    }

}
