// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : AI Assistant (extended)
// Created          : 09-27-2025
// Last Modified On : 09-27-2025 (hover feedback + binding + disabled + triangles)
// ***********************************************************************
using System;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// A simple vertical or horizontal scrollbar control for ConsoleLib.
    /// Supports keyboard (+,-,P,O) and mouse interaction (arrows, track paging, thumb drag).
    /// Hover feedback, disabled visualization and optional two-way value binding.
    /// </summary>
    public class ScrollBar : Control
    {
        private enum Part { None, DecArrow, IncArrow, Thumb, Track }

        public event EventHandler? OnValueChanged;

        /// <summary>True = vertical, False = horizontal.</summary>
        public bool Vertical { get; set; } = true;

        #region Value / Range
        private int _minimum;
        public int Minimum
        {
            get => _minimum;
            set
            {
                if (value == _minimum) return;
                _minimum = value;
                if (_maximum < _minimum) _maximum = _minimum;
                Value = Math.Max(Value, _minimum);
                Invalidate();
            }
        }

        private int _maximum = 100;
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (value == _maximum) return;
                _maximum = Math.Max(value, _minimum);
                Value = Math.Min(Value, _maximum);
                Invalidate();
            }
        }

        private int _largeChange = 10;
        public int LargeChange
        {
            get => _largeChange;
            set
            {
                if (value <= 0) value = 1;
                if (value == _largeChange) return;
                _largeChange = value;
                Invalidate();
            }
        }

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                var v = Math.Max(_minimum, Math.Min(_maximum, value));
                if (v == _value) return;
                _value = v;
                // update model if bound
                if (_valueBindingModel != null && _valueBindingPropInfo != null && !_internalBindingUpdate)
                {
                    try { _valueBindingPropInfo.SetValue(_valueBindingModel, _value); } catch { }
                }
                OnValueChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }
        #endregion

        #region Appearance / Colors
        public ConsoleColor TrackColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor ThumbColor { get; set; } = ConsoleColor.Gray;
        public ConsoleColor ThumbHotColor { get; set; } = ConsoleColor.White;
        public ConsoleColor ThumbHotBackColor { get; set; } = ConsoleColor.DarkBlue;
        public ConsoleColor DisabledColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor ArrowColor { get; set; } = ConsoleColor.Gray;
        public ConsoleColor ArrowHotColor { get; set; } = ConsoleColor.White;
        public ConsoleColor ArrowHotBackColor { get; set; } = ConsoleColor.DarkBlue;
        public ConsoleColor DisabledBackColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor DisabledThumbBackColor { get; set; } = ConsoleColor.DarkGray;
        #endregion

        public bool Enabled { get; set; } = true;

        // Drag state / hover state
        private bool _dragging;
        private int _dragThumbOffset; // offset inside thumb where click started
        private Part _hoverPart = Part.None;

        // Binding state
        private INotifyPropertyChanged? _valueBindingModel;
        private PropertyInfo? _valueBindingPropInfo;
        private bool _internalBindingUpdate;
        private string? _valueBindingPropertyName;

        public ScrollBar()
        {
            BackColor = ConsoleColor.Black;
            ForeColor = ConsoleColor.Gray;
            size = Vertical ? new Size(1, 10) : new Size(10, 1);
        }

        private int Range => Math.Max(0, _maximum - _minimum);
        private int TrackLengthRaw => (Vertical ? size.Height : size.Width);
        private int TrackLength => Math.Max(0, TrackLengthRaw - 2); // exclude arrow cells

        private (int start, int length) GetThumb()
        {
            if (TrackLength <= 0)
            {
                int full = TrackLengthRaw <= 2 ? Math.Max(0, TrackLengthRaw - 2) : 0;
                return (1, Math.Max(0, full));
            }
            if (Range == 0) return (1, TrackLength);
            int thumbLen = Math.Max(1, (int)Math.Round((double)_largeChange / (Range + _largeChange) * TrackLength));
            int maxPos = TrackLength - thumbLen;
            int pos = maxPos == 0 ? 0 : (int)Math.Round((double)(Value - _minimum) / Range * maxPos);
            return (1 + pos, thumbLen);
        }

        private Part HitTest(Point p)
        {
            var dim = RealDim;
            if (!dim.Contains(p)) return Part.None;
            int coord = Vertical ? p.Y - dim.Y : p.X - dim.X;
            if (coord == 0) return Part.DecArrow;
            if (coord == TrackLengthRaw - 1) return Part.IncArrow;
            var (thumbStart, thumbLen) = GetThumb();
            if (coord >= thumbStart && coord < thumbStart + thumbLen) return Part.Thumb;
            return Part.Track;
        }

        public override void Draw()
        {
            var dim = RealDim;
            var bg = BackColor;
            var thumbBase = TrackColor;
            // Disabled override colors
            if (!Enabled)
            {
                bg = DisabledBackColor;
                thumbBase = DisabledThumbBackColor;
            }
            ConsoleFramework.Canvas.FillRect(dim, ForeColor, bg, ' ');

            var (thumbPos, thumbLen) = GetThumb();

            // Arrow glyphs (filled triangles)
            char decGlyph = Vertical ? '\x18' : '\x11';
            char incGlyph = Vertical ? '\x19' : '\x10';

            if (Vertical)
            {
                var decFg = (!Enabled) ? DisabledColor : (_hoverPart == Part.DecArrow ? ArrowHotColor : ArrowColor);
                var decBg = (!Enabled) ? bg : (_hoverPart == Part.DecArrow ? ArrowHotBackColor : bg);
                var incFg = (!Enabled) ? DisabledColor : (_hoverPart == Part.IncArrow ? ArrowHotColor : ArrowColor);
                var incBg = (!Enabled) ? bg : (_hoverPart == Part.IncArrow ? ArrowHotBackColor : bg);
                ConsoleFramework.Canvas.OutTextXY(dim.X, dim.Y, decGlyph, decFg, decBg);
                ConsoleFramework.Canvas.OutTextXY(dim.X, dim.Bottom - 1, incGlyph, incFg, incBg);
            }
            else
            {
                var decFg = (!Enabled) ? DisabledColor : (_hoverPart == Part.DecArrow ? ArrowHotColor : ArrowColor);
                var decBg = (!Enabled) ? bg : (_hoverPart == Part.DecArrow ? ArrowHotBackColor : bg);
                var incFg = (!Enabled) ? DisabledColor : (_hoverPart == Part.IncArrow ? ArrowHotColor : ArrowColor);
                var incBg = (!Enabled) ? bg : (_hoverPart == Part.IncArrow ? ArrowHotBackColor : bg);
                ConsoleFramework.Canvas.OutTextXY(dim.X, dim.Y, decGlyph, decFg, decBg);
                ConsoleFramework.Canvas.OutTextXY(dim.Right - 1, dim.Y, incGlyph, incFg, incBg);
            }

            // Thumb
            if (thumbLen > 0)
            {
                bool hot = _hoverPart == Part.Thumb || _dragging;
                var fg = (!Enabled) ? DisabledColor : (hot ? ThumbHotColor : ThumbColor);
                var tbg = (!Enabled) ? thumbBase : (hot ? ThumbHotBackColor : thumbBase);
                if (Vertical)
                {
                    for (int i = 0; i < thumbLen; i++)
                        ConsoleFramework.Canvas.OutTextXY(dim.X, dim.Y + thumbPos + i, ' ', fg, tbg);
                }
                else
                {
                    for (int i = 0; i < thumbLen; i++)
                        ConsoleFramework.Canvas.OutTextXY(dim.X + thumbPos + i, dim.Y, ' ', fg, tbg);
                }
            }
            Valid = true;
        }

        #region Commands
        public void SmallIncrement() => Value++;
        public void SmallDecrement() => Value--;
        public void LargeIncrement() => Value += LargeChange;
        public void LargeDecrement() => Value -= LargeChange;
        #endregion

        #region Keyboard / Mouse
        public override void HandlePressKeyEvents(Interfaces.IKeyEvent e)
        {
            if (!Enabled || !e.bKeyDown) return;
            switch (char.ToUpperInvariant(e.KeyChar))
            {
                case '+': SmallIncrement(); e.Handled = true; break;
                case '-': SmallDecrement(); e.Handled = true; break;
                case 'P': LargeIncrement(); e.Handled = true; break;
                case 'O': LargeDecrement(); e.Handled = true; break;
            }
            if (e.Handled) Invalidate(); else base.HandlePressKeyEvents(e);
        }

        public override void MouseClick(IMouseEvent M)
        {
            if (!Enabled) { base.MouseClick(M); return; }
            var dim = RealDim;
            if (!dim.Contains(M.MousePos)) { base.MouseClick(M); return; }
            int coord = Vertical ? M.MousePos.Y - dim.Y : M.MousePos.X - dim.X;
            int lastIndex = TrackLengthRaw - 1;
            if (coord == 0) SmallDecrement();
            else if (coord == lastIndex) SmallIncrement();
            else
            {
                var (thumbStart, thumbLen) = GetThumb();
                int thumbEnd = thumbStart + thumbLen - 1;
                if (coord < thumbStart) LargeDecrement();
                else if (coord > thumbEnd) LargeIncrement();
                else { _dragging = true; _dragThumbOffset = coord - thumbStart; }
            }
            Invalidate();
            base.MouseClick(M);
        }

        public override void MouseMove(IMouseEvent M, Point lastMousePos)
        {
            if (!Enabled) { base.MouseMove(M, lastMousePos); return; }
            if (_dragging && M.MouseMoved)
            {
                var dim = RealDim;
                int coord = Vertical ? M.MousePos.Y - dim.Y : M.MousePos.X - dim.X;
                coord = Math.Max(1, Math.Min(TrackLengthRaw - 2, coord));
                var (thumbStart, thumbLen) = GetThumb();
                int newThumbStart = coord - _dragThumbOffset;
                int minThumbStart = 1;
                int maxThumbStart = TrackLengthRaw - 1 - thumbLen;
                newThumbStart = Math.Max(minThumbStart, Math.Min(maxThumbStart, newThumbStart));
                int maxPos = TrackLength - thumbLen;
                int posWithinTrack = newThumbStart - 1;
                int newValue = Range == 0 || maxPos == 0 ? _minimum : _minimum + (int)Math.Round((double)posWithinTrack / maxPos * Range);
                Value = newValue;
            }
            else if (M.MouseMoved)
            {
                var newPart = HitTest(M.MousePos);
                if (newPart != _hoverPart)
                {
                    _hoverPart = newPart;
                    Invalidate();
                }
            }
            base.MouseMove(M, lastMousePos);
        }

        public override void MouseLeave(Point M)
        {
            _dragging = false;
            if (_hoverPart != Part.None)
            {
                _hoverPart = Part.None;
                Invalidate();
            }
            base.MouseLeave(M);
        }
        #endregion

        #region Binding
        /// <summary>
        /// Binds this scrollbar's Value to a property of an INotifyPropertyChanged model (two-way binding).
        /// The model property must be read/write and of type int (or convertible).
        /// </summary>
        public void BindValue(INotifyPropertyChanged model, string propertyName)
        {
            _valueBindingModel = model;
            _valueBindingPropertyName = propertyName;
            _valueBindingPropInfo = model.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (_valueBindingPropInfo != null && _valueBindingPropInfo.CanRead)
            {
                try
                {
                    var raw = _valueBindingPropInfo.GetValue(model);
                    if (raw != null)
                    {
                        int nv = Convert.ToInt32(raw);
                        _internalBindingUpdate = true;
                        Value = nv; // sets internal without model re-entry
                        _internalBindingUpdate = false;
                    }
                }
                catch { }
            }
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == _valueBindingModel && e.PropertyName == _valueBindingPropertyName && _valueBindingPropInfo != null)
            {
                try
                {
                    var raw = _valueBindingPropInfo.GetValue(_valueBindingModel);
                    if (raw != null)
                    {
                        int nv = Convert.ToInt32(raw);
                        if (nv != _value)
                        {
                            _internalBindingUpdate = true;
                            Value = nv; // uses setter
                            _internalBindingUpdate = false;
                        }
                    }
                }
                catch { }
            }
        }
        #endregion
    }
}
