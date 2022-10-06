// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-05-2022
// ***********************************************************************
// <copyright file="Control.cs" company="ConsoleLib">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static ConsoleLib.NativeMethods;

namespace ConsoleLib
{
    /// <summary>
    /// This is the basic class of all TextControls
    /// </summary>
    public class Control 
    {
        /// <summary>
        /// The dimension
        /// </summary>
        protected Rectangle _dimension;
        /// <summary>
        /// The active
        /// </summary>
        private bool _active;
        /// <summary>
        /// The valid
        /// </summary>
        private bool _valid;
        /// <summary>
        /// The shaddow
        /// </summary>
        private bool _shaddow;
        /// <summary>
        /// The visible
        /// </summary>
        private bool _visible = true;
        /// <summary>
        /// Gets or sets the message queue.
        /// </summary>
        /// <value>The message queue.</value>
        public static Stack<(Action<object, EventArgs>, object, EventArgs)> MessageQueue { get; set; } = default;

        /// <summary>
        /// Gets or sets the dimension.
        /// </summary>
        /// <value>The dimension.</value>
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
                    OnMove?.Invoke(this, EventArgs.Empty);
                if (_lastdim.Size != _dimension.Size)
                    OnResize?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point position
        {
            get => _dimension.Location;
            set
            {
                if (_dimension.Location == value) return;
                Rectangle _lastdim = _dimension;
                _dimension.Location = value;
                HandleControlMove(_lastdim);
                OnMove?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size size
        {
            get => _dimension.Size; set
            {
                if (_dimension.Size == value) return;
                Rectangle _lastdim = _dimension;
                _dimension.Size = value;
                HandleControlMove(_lastdim);
                OnResize?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the control move.
        /// </summary>
        /// <param name="_lastdim">The lastdim.</param>
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

        /// <summary>
        /// Gets the real dim.
        /// </summary>
        /// <value>The real dim.</value>
        public Rectangle realDim => realDimOf(_dimension);

        /// <summary>
        /// Reals the dim of.
        /// </summary>
        /// <param name="aDim">a dim.</param>
        /// <returns>Rectangle.</returns>
        public Rectangle realDimOf(Rectangle aDim)
        {
            var result = aDim;
            if (parent != null)
            {
                result.Offset(parent.realDim.Location);
            }
            return result;
        }

#if NET5_0_OR_GREATER
        public Rectangle localDimOf(Rectangle aDim, Control? ancestor = null)
#else
        /// <summary>
        /// Locals the dim of.
        /// </summary>
        /// <param name="aDim">a dim.</param>
        /// <param name="ancestor">The ancestor.</param>
        /// <returns>Rectangle.</returns>
        public Rectangle localDimOf(Rectangle aDim, Control ancestor = null)
#endif  
        {
            var result = aDim;
            result.Location = Point.Subtract(result.Location,(Size)_dimension.Location);
            if (parent != null && parent != ancestor)
            {
                result = parent.localDimOf(result, ancestor);                 
            }
            return result;
        }

        /// <summary>
        /// Res the draw.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        public virtual void ReDraw(Rectangle dimension)
        {
            if (_visible && dimension.IntersectsWith(_dimension))
            {
                Draw();
                _valid = true;
            }
        }
        /// <summary>
        /// Overs the specified m.
        /// </summary>
        /// <param name="M">The m.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Over(Point M) => realDim.Contains(M);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Control"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
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
                        if (parent.ActiveControl!=null)
                          parent.ActiveControl.active = false;
                        parent.ActiveControl = this;
                    }
                }
                _active = value;
                if (value)
                    OnActivate?.Invoke(this, EventArgs.Empty);
                OnChange?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Control"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
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
                OnChange?.Invoke(this, EventArgs.Empty);
                parent?.Invalidate();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        public bool IsVisible => _visible && (parent?.IsVisible ?? true);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Control"/> is shaddow.
        /// </summary>
        /// <value><c>true</c> if shaddow; otherwise, <c>false</c>.</value>
        public bool shaddow
        {
            get => _shaddow; set
            {
                if (_shaddow == value) return;
                _shaddow = value;
                parent?.Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Control"/> is valid.
        /// </summary>
        /// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
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

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        public void Invalidate()
        {
            _valid = false;
            MessageQueue?.Push((DoRedraw, this, new EventArgs()));
        }

        /// <summary>
        /// Does the redraw.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="a">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DoRedraw(object sender, EventArgs a)
        {
            if (!_valid)
                ReDraw(((Control)sender).dimension);
        }

#if NET5_0_OR_GREATER
        public event EventHandler? OnClick;
        public event EventHandler? OnMove;
        public event EventHandler? OnResize;
        public event EventHandler? OnChange;
        public event EventHandler? OnActivate;
        public event EventHandler? OnMouseEnter;
        public event EventHandler? OnMouseLeave;
        public event EventHandler<MouseEventArgs>? OnMouseMove;
        public event EventHandler<KeyEventArgs>? OnKeyPressed;
#else
        /// <summary>
        /// Occurs when [on click].
        /// </summary>
        public event EventHandler OnClick;
        /// <summary>
        /// Occurs when [on move].
        /// </summary>
        public event EventHandler OnMove;
        /// <summary>
        /// Occurs when [on resize].
        /// </summary>
        public event EventHandler OnResize;
        /// <summary>
        /// Occurs when [on change].
        /// </summary>
        public event EventHandler OnChange;
        /// <summary>
        /// Occurs when [on activate].
        /// </summary>
        public event EventHandler OnActivate;
        /// <summary>
        /// Occurs when [on mouse enter].
        /// </summary>
        public event EventHandler OnMouseEnter;
        /// <summary>
        /// Occurs when [on mouse leave].
        /// </summary>
        public event EventHandler OnMouseLeave;
        /// <summary>
        /// Occurs when [on mouse move].
        /// </summary>
        public event EventHandler<MouseEventArgs> OnMouseMove;
        /// <summary>
        /// Occurs when [on key pressed].
        /// </summary>
        public event EventHandler<KeyEventArgs> OnKeyPressed;
#endif

        /// <summary>
        /// The back color
        /// </summary>
        public ConsoleColor BackColor;
        /// <summary>
        /// The fore color
        /// </summary>
        public ConsoleColor ForeColor;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get => _text; set => SetText(value); }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void SetText(string value)
        {
            if (_text == value) return;
            _text = value;
            OnChange?.Invoke(this, new EventArgs());
            Invalidate();
        }

        /// <summary>
        /// The children
        /// </summary>
        public List<Control> children = new List<Control>();
#if NET5_0_OR_GREATER
        public Control? ActiveControl; 
        private Control? _parent;
#else
        /// <summary>
        /// The active control
        /// </summary>
        public Control ActiveControl;
        /// <summary>
        /// The parent
        /// </summary>
        private Control _parent;
#endif
        /// <summary>
        /// The text
        /// </summary>
        private string _text="";

#if NET5_0_OR_GREATER
        public Control? parent 
#else
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Control parent
#endif
        { get => _parent; set => SetParent(value); }

#if NET5_0_OR_GREATER
        private void SetParent(Control? value) 
#else
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetParent(Control value) 
#endif
        {
            if (_parent == value) return;
            var oldPar = _parent;
            _parent = value;
            oldPar?.Remove(this);
            value?.Add(this);
        }

        /// <summary>
        /// Adds the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>Control.</returns>
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

        /// <summary>
        /// Removes the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>Control.</returns>
        public Control Remove(Control control)
        {
            control.parent = null;
            children.Remove(control);
            return this;
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public virtual void Draw()
        {
            // Draw Background
            lock(this)
            {
                if (realDim.Height>1)
                {
                    ConsoleFramework.Canvas.FillRect(realDim, ForeColor, BackColor, ConsoleFramework.chars[4]);
                }
                Console.ForegroundColor = ForeColor;
                Console.BackgroundColor = BackColor;
                Console.SetCursorPosition(realDim.X, realDim.Y+realDim.Height/2);
                Console.Write($"[{Text}]");
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        /// <summary>
        /// Clicks this instance.
        /// </summary>
        public virtual void Click()
        {
            OnClick?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Mouses the enter.
        /// </summary>
        /// <param name="M">The m.</param>
        public virtual void MouseEnter(Point M)
        {
            OnMouseEnter?.Invoke(this, EventArgs.Empty);
            foreach (var ctrl in children)
            {
                if (ctrl.Over(M))
                    ctrl.MouseEnter(M);
            }
        }
        /// <summary>
        /// Mouses the leave.
        /// </summary>
        /// <param name="M">The m.</param>
        public virtual void MouseLeave(Point M)
        {
            OnMouseLeave?.Invoke(this, EventArgs.Empty);
            foreach (var ctrl in children)
            {
                if (ctrl.Over(M))
                    ctrl.MouseLeave(M);
            }
        }

        /// <summary>
        /// Mouses the move.
        /// </summary>
        /// <param name="M">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="lastMousePos">The last mouse position.</param>
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
        /// <summary>
        /// Mouses the click.
        /// </summary>
        /// <param name="M">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
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
                OnClick?.Invoke(this, EventArgs.Empty);

        }

        /// <summary>
        /// Handles the press key events.
        /// </summary>
        /// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
        public virtual void HandlePressKeyEvents(KeyPressEventArgs e)
        {
            if (e.KeyChar == Accellerator)
            {
                Click();
                e.Handled = true;
            }
            else
            { 
                ActiveControl?.HandlePressKeyEvents(e);
                if (!e.Handled) foreach (var ctrl in children){
                        ctrl.HandlePressKeyEvents(e);
                        if (e.Handled) break;
                    }
            }

        }
        /// <summary>
        /// Does the update.
        /// </summary>
        public virtual void DoUpdate()
        {
            foreach (var ctrl in children)
            {
                ctrl.DoUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public int Tag { get; set; }
        /// <summary>
        /// Gets or sets the accellerator.
        /// </summary>
        /// <value>The accellerator.</value>
        public Char Accellerator { get; set; }
    }
}
