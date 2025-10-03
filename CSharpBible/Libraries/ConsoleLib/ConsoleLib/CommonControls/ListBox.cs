// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : AI Assistant
// Created          : 2025-09-27
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// Simple list box for textual items. Supports
    ///  - Binding to an ObservableCollection / any IList
    ///  - Optional vertical ScrollBar (auto shown if items exceed viewport height)
    ///  - Two way binding for SelectedItem
    ///  - Mouse wheel scrolling & hover highlighting
    /// </summary>
    public class ListBox : Control
    {
        private IList? _itemsSource;
        private INotifyCollectionChanged? _notifyCol;
        private ScrollBar? _vScroll;
        private int _topIndex;
        private int _selectedIndex = -1;
        private object? _selectedItem;
        private int _hoverIndex = -1; // new: row currently hovered (absolute index)

        // Selection binding
        private INotifyPropertyChanged? _selBindingModel;
        private PropertyInfo? _selBindingPropInfo;
        private bool _internalSelUpdate;
        private string? _selBindingPropertyName;

        public ConsoleColor SelectedBackColor { get; set; } = ConsoleColor.DarkBlue;
        public ConsoleColor SelectedForeColor { get; set; } = ConsoleColor.White;
        public ConsoleColor HoverBackColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor HoverForeColor { get; set; } = ConsoleColor.White;
        public ConsoleColor ScrollBarTrackColor { get; set; } = ConsoleColor.DarkGray;

        /// <summary>Items source (IList or ObservableCollection). Re-subscribes to change notifications.</summary>
        public IList? ItemsSource
        {
            get => _itemsSource; set => SetItemsSource(value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex) return;
                if (value < -1 || value >= ItemCount) return;
                _selectedIndex = value;
                _selectedItem = _selectedIndex >= 0 ? _itemsSource?[_selectedIndex] : null;
                UpdateSelectedBinding();
                EnsureSelectedVisible();
                Invalidate();
            }
        }

        public object? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (ReferenceEquals(value, _selectedItem)) return;
                int idx = IndexOfItem(value);
                if (idx >= 0 || value == null)
                {
                    _selectedItem = value;
                    _selectedIndex = idx;
                    UpdateSelectedBinding();
                    EnsureSelectedVisible();
                    Invalidate();
                }
            }
        }

        private int VisibleRows => Math.Max(0, size.Height);
        private int ItemCount => _itemsSource?.Count ?? 0;
        private bool NeedScrollBar => ItemCount > VisibleRows && VisibleRows > 0;

        public (INotifyPropertyChanged, string) SelectedBinding { set=> BindSelected(value.Item1,value.Item2); }

        public ListBox()
        {
            BackColor = ConsoleColor.Black;
            ForeColor = ConsoleColor.Gray;
            size = new Size(15, 5);
        }

        private void SetItemsSource(IList? list)
        {
            if (ReferenceEquals(list, _itemsSource)) return;
            if (_notifyCol != null)
                _notifyCol.CollectionChanged -= Items_CollectionChanged;
            _itemsSource = list;
            _notifyCol = list as INotifyCollectionChanged;
            if (_notifyCol != null)
                _notifyCol.CollectionChanged += Items_CollectionChanged;
            _topIndex = 0;
            if (_itemsSource == null || _itemsSource.Count == 0)
            {
                _selectedIndex = -1; _selectedItem = null;
            }
            else if (_selectedIndex >= _itemsSource.Count)
            {
                _selectedIndex = _itemsSource.Count - 1;
                _selectedItem = _itemsSource[_selectedIndex];
            }
            RefreshScrollBar();
            Invalidate();
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshScrollBar();
            if (_selectedIndex >= ItemCount)
            {
                _selectedIndex = ItemCount - 1;
                _selectedItem = _selectedIndex >= 0 ? _itemsSource?[_selectedIndex] : null;
                UpdateSelectedBinding();
            }
            EnsureSelectedVisible();
            Invalidate();
        }

        private void RefreshScrollBar()
        {
            if (NeedScrollBar)
            {
                if (_vScroll == null)
                {
                    _vScroll = new ScrollBar
                    {
                        Parent = this,
                        Vertical = true,
                        Position = new Point(Dimension.Width - 1, 0),
                        size = new Size(1, size.Height),
                        LargeChange = Math.Max(1, VisibleRows - 1)
                    };
                    _vScroll.OnValueChanged += (_, _) => { _topIndex = _vScroll.Value; Invalidate(); };
                }
                _vScroll.Maximum = Math.Max(0, ItemCount - VisibleRows);
                _vScroll.Value = Math.Max(0, Math.Min(_vScroll.Maximum, _topIndex));
                _vScroll.Visible = true;
            }
            else if (_vScroll != null)
            {
                _vScroll.Visible = false;
                _topIndex = 0;
            }
        }

        private void EnsureSelectedVisible()
        {
            if (_selectedIndex < 0) return;
            if (_selectedIndex < _topIndex) _topIndex = _selectedIndex;
            if (_selectedIndex >= _topIndex + VisibleRows) _topIndex = Math.Max(0, _selectedIndex - VisibleRows + 1);
            SyncScrollBar();
        }

        private void SyncScrollBar()
        {
            if (_vScroll != null)
            {
                _vScroll.Maximum = Math.Max(0, ItemCount - VisibleRows);
                _vScroll.Value = Math.Max(0, Math.Min(_vScroll.Maximum, _topIndex));
            }
        }

        private int IndexOfItem(object? value)
        {
            if (value == null || _itemsSource == null) return -1;
            for (int i = 0; i < _itemsSource.Count; i++)
                if (Equals(_itemsSource[i], value)) return i;
            return -1;
        }

        public override void SetText(string value)
        {
            base.SetText(value);
        }

        public override void Draw()
        {
            var dim = RealDim;
            ConsoleFramework.Canvas.FillRect(dim, ForeColor, BackColor, ' ');
            int rows = VisibleRows;
            int colWidth = size.Width - (NeedScrollBar ? 1 : 0);
            for (int line = 0; line < rows; line++)
            {
                int itemIndex = _topIndex + line;
                var y = dim.Y + line;
                var x = dim.X;
                string text = "";
                bool selectedLine = itemIndex == _selectedIndex;
                bool hoverLine = itemIndex == _hoverIndex && !selectedLine; // do not override selected appearance
                if (itemIndex < ItemCount && itemIndex >= 0)
                {
                    object? item = _itemsSource![itemIndex];
                    text = item?.ToString() ?? "";
                }
                if (text.Length > colWidth) text = text.Substring(0, colWidth);
                if (colWidth > 0)
                {
                    var fg = selectedLine ? SelectedForeColor : (hoverLine ? HoverForeColor : ForeColor);
                    var bg = selectedLine ? SelectedBackColor : (hoverLine ? HoverBackColor : BackColor);
                    string padded = text.PadRight(colWidth, ' ');
                    for (int i = 0; i < colWidth; i++)
                        ConsoleFramework.Canvas.OutTextXY(x + i, y, padded[i], fg, bg);
                }
            }
            _vScroll?.Draw();
            Valid = true;
        }

        public override void MouseClick(Interfaces.IMouseEvent M)
        {
            if (!Visible || !Enabled) { base.MouseClick(M); return; }
            var dim = RealDim;
            if (!dim.Contains(M.MousePos)) { base.MouseClick(M); return; }
            int relY = M.MousePos.Y - dim.Y;
            if (relY >= 0 && relY < VisibleRows)
            {
                int idx = _topIndex + relY;
                if (idx < ItemCount)
                {
                    SelectedIndex = idx;
                }
            }
            base.MouseClick(M);
        }

        public override void MouseMove(Interfaces.IMouseEvent M, Point lastMousePos)
        {
            if (!Visible || !Enabled) { return; }
            bool invalidate = false;
            if (M.MouseWheel != 0 && ItemCount > 0)
            {
                // Negative wheel => scroll down (increase top), positive => scroll up
                int delta = Math.Sign(-M.MouseWheel); // typical expectation: wheel up (positive) scrolls up (topIndex--)
                _topIndex = Math.Max(0, Math.Min(Math.Max(0, ItemCount - VisibleRows), _topIndex + delta));
                SyncScrollBar();
                invalidate = true;
            }
            var dim = RealDim;
            if (NeedScrollBar)
                dim.Inflate(-1,0);
            if (dim.Contains(M.MousePos))
            {
                int relY = M.MousePos.Y - dim.Y;
                int newHover = (relY >= 0 && relY < VisibleRows) ? _topIndex + relY : -1;
                if (newHover != _hoverIndex)
                {
                    _hoverIndex = newHover;
                    invalidate = true;
                }
            }
            else if (_hoverIndex != -1)
            {
                _hoverIndex = -1;
                invalidate = true;
            }
            if (invalidate) Invalidate();
            base.MouseMove(M, lastMousePos);
        }

        public override void MouseLeave(Point M)
        {
            if (_hoverIndex != -1)
            {
                _hoverIndex = -1;
                Invalidate();
            }
            base.MouseLeave(M);
        }

        public override void HandlePressKeyEvents(Interfaces.IKeyEvent e)
        {
            if (!Visible || !Enabled) { return; }
            if (!e.bKeyDown) return;
            switch (char.ToUpperInvariant(e.KeyChar))
            {
                case 'J':
                case '+':
                    if (SelectedIndex < ItemCount - 1) SelectedIndex++;
                    e.Handled = true; break;
                case 'K':
                case '-':
                    if (SelectedIndex > 0) SelectedIndex--;
                    e.Handled = true; break;
                case 'P':
                    SelectedIndex = Math.Min(ItemCount - 1, SelectedIndex + Math.Max(1, VisibleRows - 1)); e.Handled = true; break;
                case 'O':
                    SelectedIndex = Math.Max(0, SelectedIndex - Math.Max(1, VisibleRows - 1)); e.Handled = true; break;
            }
            if (e.Handled) EnsureSelectedVisible(); else base.HandlePressKeyEvents(e);
        }

        #region Binding SelectedItem
        public void BindSelected(INotifyPropertyChanged model, string propertyName)
        {
            _selBindingModel = model;
            _selBindingPropertyName = propertyName;
            _selBindingPropInfo = model.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (_selBindingPropInfo != null && _selBindingPropInfo.CanRead)
            {
                try
                {
                    var val = _selBindingPropInfo.GetValue(model);
                    if (val != null) SelectedItem = val;
                }
                catch { }
            }
            model.PropertyChanged += SelModel_PropertyChanged;
        }

        private void SelModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == _selBindingModel && e.PropertyName == _selBindingPropertyName && _selBindingPropInfo != null)
            {
                try
                {
                    var val = _selBindingPropInfo.GetValue(_selBindingModel);
                    if (!ReferenceEquals(val, _selectedItem))
                    {
                        _internalSelUpdate = true;
                        SelectedItem = val;
                        _internalSelUpdate = false;
                    }
                }
                catch { }
            }
        }

        private void UpdateSelectedBinding()
        {
            if (_internalSelUpdate) return;
            if (_selBindingModel != null && _selBindingPropInfo != null && _selBindingPropInfo.CanWrite)
            {
                try { _selBindingPropInfo.SetValue(_selBindingModel, _selectedItem); } catch { }
            }
        }
        #endregion
    }
}
