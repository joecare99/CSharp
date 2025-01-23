// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-05-2022
// ***********************************************************************
// <copyright file="Control.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ConsoleLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ConsoleLib
{
    /// <summary>
    /// This is the basic class of all TextControls
    /// </summary>
    public class Control : IControl
    {
        /// <summary>
        /// The Dimension
        /// </summary>
        protected Rectangle _dimension;
        /// <summary>
        /// The Active
        /// </summary>
        private bool _active;
        /// <summary>
        /// The Valid
        /// </summary>
        private bool _valid;
        /// <summary>
        /// The Shadow
        /// </summary>
        private bool _shadow;
        /// <summary>
        /// The Visible
        /// </summary>
        private bool _visible = true;
        /// <summary>
        /// Gets or sets the message queue.
        /// </summary>
        /// <value>The message queue.</value>
        
        public static Stack<(Action<object, EventArgs>, object, EventArgs)>? MessageQueue { get; set; } = default;

        /// <summary>
        /// Gets or sets the Dimension.
        /// </summary>
        /// <value>The Dimension.</value>
        public Rectangle Dimension
        {
            get => _dimension;
            set
            {
                if (_dimension == value) return;
                Rectangle _lastDim = _dimension;
                _dimension = value;
                HandleControlMove(_lastDim);
                if (_lastDim.Location != _dimension.Location)
                    OnMove?.Invoke(this, EventArgs.Empty);
                if (_lastDim.Size != _dimension.Size)
                    OnResize?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the Position.
        /// </summary>
        /// <value>The Position.</value>
        public Point Position
        {
            get => _dimension.Location;
            set
            {
                if (_dimension.Location == value) return;
                Rectangle _lastDim = _dimension;
                _dimension.Location = value;
                HandleControlMove(_lastDim);
                OnMove?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the Size.
        /// </summary>
        /// <value>The Size.</value>
        public Size Size
        {
            get => _dimension.Size; set
            {
                if (_dimension.Size == value) return;
                Rectangle _lastDim = _dimension;
                _dimension.Size = value;
                HandleControlMove(_lastDim);
                OnResize?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the real dim.
        /// </summary>
        /// <value>The real dim.</value>
        public Rectangle RealDim => RealDimOf(_dimension);

        /// <summary>
        /// Handles the control move.
        /// </summary>
        /// <param name="_lastDim">The last Dimension.</param>
        private void HandleControlMove(Rectangle _lastDim)
        {
            if (Parent == null)
            {
                // Todo: Restore From Background
                ConsoleFramework.Canvas.FillRect(_lastDim, ConsoleFramework.Canvas.ForegroundColor, ConsoleFramework.Canvas.BackgroundColor, ConsoleFramework.chars[4]);
            }
            else
            {
                _lastDim.Location = Point.Add(_lastDim.Location, (Size)Parent.Position);
                Parent.ReDraw(_lastDim);
            }
            if (IsVisible)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Reals the dim of.
        /// </summary>
        /// <param name="aDim">a dim.</param>
        /// <returns>Rectangle.</returns>
        public Rectangle RealDimOf(Rectangle aDim)
        {
            var result = aDim;
            if (Parent != null)
            {
                result.Offset(Parent.RealDim.Location);
            }
            return result;
        }

        /// <summary>
        /// Locals the dim of.
        /// </summary>
        /// <param name="aDim">a dim.</param>
        /// <param name="ancestor">The ancestor.</param>
        /// <returns>Rectangle.</returns>
        public Rectangle LocalDimOf(Rectangle aDim, IControl? ancestor = null)
 
        {
            var result = aDim;
            result.Location = Point.Subtract(result.Location,(Size)_dimension.Location);
            if (Parent != null && Parent != ancestor)
            {
                result = Parent.LocalDimOf(result, ancestor);                 
            }
            return result;
        }

        /// <summary>
        /// Res the draw.
        /// </summary>
        /// <param name="dimension">The Dimension.</param>
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
        public bool Over(Point M) => RealDim.Contains(M);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Control"/> is Active.
        /// </summary>
        /// <value><c>true</c> if Active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get => _active; set
            {
                if (!_visible || _active == value) return;
                if (Parent != null)
                {
                    if (!value)
                        Parent.ActiveControl = null;
                    else
                    {
                        if (Parent.ActiveControl!=null)
                          Parent.ActiveControl.Active = false;
                        Parent.ActiveControl = this;
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
        /// Gets or sets a value indicating whether this <see cref="Control"/> is Visible.
        /// </summary>
        /// <value><c>true</c> if Visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get => _visible; set
            {
                if (_visible == value) return;
                if (!value)
                {
                    Active = false;
                }
                _visible = value;
                if (value)
                OnChange?.Invoke(this, EventArgs.Empty);
                Parent?.Invalidate();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is Visible.
        /// </summary>
        /// <value><c>true</c> if this instance is Visible; otherwise, <c>false</c>.</value>
        public bool IsVisible => _visible && (Parent?.IsVisible ?? true);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Control"/> is Shadow.
        /// </summary>
        /// <value><c>true</c> if Shadow; otherwise, <c>false</c>.</value>
        public bool Shadow
        {
            get => _shadow; set
            {
                if (_shadow == value) return;
                _shadow = value;
                Parent?.Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Control"/> is Valid.
        /// </summary>
        /// <value><c>true</c> if Valid; otherwise, <c>false</c>.</value>
        public bool Valid
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
                ReDraw(((Control)sender).Dimension);
        }

        /// <summary>
        /// Occurs when [on click].
        /// </summary>
        public event EventHandler? OnClick;
        /// <summary>
        /// Occurs when [on move].
        /// </summary>
        public event EventHandler? OnMove;
        /// <summary>
        /// Occurs when [on resize].
        /// </summary>
        public event EventHandler? OnResize;
        /// <summary>
        /// Occurs when [on change].
        /// </summary>
        public event EventHandler? OnChange;
        /// <summary>
        /// Occurs when [on activate].
        /// </summary>
        public event EventHandler? OnActivate;
        /// <summary>
        /// Occurs when [on mouse enter].
        /// </summary>
        public event EventHandler? OnMouseEnter;
        /// <summary>
        /// Occurs when [on mouse leave].
        /// </summary>
        public event EventHandler? OnMouseLeave;
        /// <summary>
        /// Occurs when [on mouse move].
        /// </summary>
        public event EventHandler<MouseEventArgs>? OnMouseMove;
        /// <summary>
        /// Occurs when [on key pressed].
        /// </summary>
        public event EventHandler<KeyEventArgs>? OnKeyPressed;

        /// <summary>
        /// The back color
        /// </summary>
        public ConsoleColor BackColor { get; set; }
        /// <summary>
        /// The fore color
        /// </summary>
        public ConsoleColor ForeColor { get; set; }

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
        /// The Children
        /// </summary>
        public List<IControl> Children { get; } = new();

        /// <summary>
        /// The Active control
        /// </summary>
        public IControl? ActiveControl { get; set; }
        /// <summary>
        /// The Parent
        /// </summary>
        private IControl? _parent;
        /// <summary>
        /// The text
        /// </summary>
        private string _text="";

        /// <summary>
        /// Gets or sets the Parent.
        /// </summary>
        /// <value>The Parent.</value>
        public IControl? Parent { get => _parent; set => SetParent(value); }

        /// <summary>
        /// Sets the Parent.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetParent(IControl? value) 
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
        public IControl Add(IControl control)
        {
            if (control.Parent != this)
               control.Parent?.Remove(control);
            if (control.Parent == null)
            {
                Children.Add(control);
                control.Parent = this;
            }
            else if (!Children.Contains(control))
                Children.Add(control);

            return this;
        }

        /// <summary>
        /// Removes the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>Control.</returns>
        public IControl Remove(IControl control)
        {
            control.Parent = null;
            Children.Remove(control);
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
                if (RealDim.Height>1)
                {
                    ConsoleFramework.Canvas.FillRect(RealDim, ForeColor, BackColor, ConsoleFramework.chars[4]);
                }
                ConsoleFramework.console.ForegroundColor = ForeColor;
                ConsoleFramework.console.BackgroundColor = BackColor;
                ConsoleFramework.console.SetCursorPosition(RealDim.X, RealDim.Y+RealDim.Height/2);
                ConsoleFramework.console.Write($"[{Text}]");
                ConsoleFramework.console.BackgroundColor = ConsoleColor.Black;
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
        /// Mouse enters the Control.
        /// </summary>
        /// <param name="M">The m.</param>
        public virtual void MouseEnter(Point M)
        {
            OnMouseEnter?.Invoke(this, EventArgs.Empty);
            foreach (var ctrl in Children)
            {
                if (ctrl.Over(M))
                    ctrl.MouseEnter(M);
            }
        }
        /// <summary>
        /// Mouse leaves the control.
        /// </summary>
        /// <param name="M">The m.</param>
        public virtual void MouseLeave(Point M)
        {
            OnMouseLeave?.Invoke(this, EventArgs.Empty);
            foreach (var ctrl in Children)
            {
                if (ctrl.Over(M))
                    ctrl.MouseLeave(M);
            }
        }

        /// <summary>
        /// Mouse move in the control.
        /// </summary>
        /// <param name="M">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="lastMousePos">The last mouse Position.</param>
        public virtual void MouseMove(MouseEventArgs M, Point lastMousePos)
        {
            OnMouseMove?.Invoke(this, M);
            foreach (var ctrl in Children)
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
        /// Mouse clicks the control.
        /// </summary>
        /// <param name="M">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        public virtual void MouseClick(MouseEventArgs M)
        {
            bool xFlag = false;
            foreach (var ctrl in Children)
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
            if (e.KeyChar == Accelerator)
            {
                Click();
                e.Handled = true;
            }
            else
            { 
                ActiveControl?.HandlePressKeyEvents(e);
                if (!e.Handled) foreach (var ctrl in Children){
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
            foreach (var ctrl in Children)
            {
                ctrl.DoUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public long Tag { get; set; }
        /// <summary>
        /// Gets or sets the accelerator.
        /// </summary>
        /// <value>The accelerator.</value>
        public Char Accelerator { get; set; }
    }
}
