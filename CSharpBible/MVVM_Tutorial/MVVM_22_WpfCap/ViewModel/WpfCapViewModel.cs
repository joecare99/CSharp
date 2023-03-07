// ***********************************************************************
// Assembly         : MVVM_22_WpfCap
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="WpfCapViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using MVVM_22_WpfCap.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MVVM_22_WpfCap.ViewModel
{
    /// <summary>
    /// Class RowData.
    /// Implements the <see cref="INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class RowData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the row identifier.
        /// </summary>
        /// <value>The row identifier.</value>
        public int RowId { get; set; }
        /// <summary>
        /// Gets or sets the move left.
        /// </summary>
        /// <value>The move left.</value>
        public DelegateCommand MoveLeft { get; set; }
        /// <summary>
        /// Gets or sets the move right.
        /// </summary>
        /// <value>The move right.</value>
        public DelegateCommand MoveRight { get; set; }
        /// <summary>
        /// Gets or sets the color of the tile.
        /// </summary>
        /// <value>The color of the tile.</value>
        public int[] TileColor
        {
            get
            {
                var _result = new int[4];
                for (var i = 0;i<4;i++)
                    _result[i]= Parent.Model.TileColor(i,RowId - 1);
                return _result;
            }
        }

        /// <summary>
        /// Gets the this.
        /// </summary>
        /// <value>The this.</value>
        public object This => this;
        /// <summary>
        /// The parent
        /// </summary>
        public WpfCapViewModel Parent;
#if NET5_0_OR_GREATER
        public event PropertyChangedEventHandler? PropertyChanged;
#else
        /// <summary>
        /// Tritt ein, wenn sich ein Eigenschaftswert ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
#endif
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="name">The name.</param>
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Class ColData.
    /// Implements the <see cref="INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class ColData : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the col identifier.
        /// </summary>
        /// <value>The col identifier.</value>
        public int ColId { get; set; }
        /// <summary>
        /// Gets or sets the move up.
        /// </summary>
        /// <value>The move up.</value>
        public DelegateCommand MoveUp { get; set; }
        /// <summary>
        /// Gets or sets the move down.
        /// </summary>
        /// <value>The move down.</value>
        public DelegateCommand MoveDown { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="System.Int32"/> with the specified ix.
        /// </summary>
        /// <param name="ix">The ix.</param>
        /// <returns>System.Int32.</returns>
        public int this[int ix] => Parent.Model.TileColor(ix, ColId - 1);
        /// <summary>
        /// Gets the this.
        /// </summary>
        /// <value>The this.</value>
        public object This => this;
        /// <summary>
        /// The parent
        /// </summary>
        public WpfCapViewModel Parent;
#if NET5_0_OR_GREATER
        public event PropertyChangedEventHandler? PropertyChanged;
#else
        /// <summary>
        /// Tritt ein, wenn sich ein Eigenschaftswert ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
#endif
        /// <summary>
        /// The length
        /// </summary>
        public readonly int Length = 4;
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="name">The name.</param>
        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Class WpfCapViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class WpfCapViewModel : BaseViewModel
    {
        public WpfCapViewModel():this((IWpfCapModel)DISource.Resolver.Invoke(typeof(IWpfCapModel)))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfCapViewModel"/> class.
        /// </summary>
        public WpfCapViewModel(IWpfCapModel model)
        { 
            _model = model;
            var _moveLeft = new DelegateCommand(DoMoveLeft);
            var _moveRight = new DelegateCommand(DoMoveRight);
            Rows = new ObservableCollection<RowData>();
            Rows.Add(new RowData() { RowId = 1, MoveLeft = _moveLeft, MoveRight = _moveRight, Parent = this });
            Rows.Add(new RowData() { RowId = 2, MoveLeft = _moveLeft, MoveRight = _moveRight, Parent = this });
            Rows.Add(new RowData() { RowId = 3, MoveLeft = _moveLeft, MoveRight = _moveRight, Parent = this });
            Rows.Add(new RowData() { RowId = 4, MoveLeft = _moveLeft, MoveRight = _moveRight, Parent = this });

            var _moveUp = new DelegateCommand(DoMoveUp);
            var _moveDown = new DelegateCommand(DoMoveDown);
            Cols = new ObservableCollection<ColData>();
            Cols.Add(new ColData() { ColId = 1, MoveUp = _moveUp, MoveDown = _moveDown, Parent = this });
            Cols.Add(new ColData() { ColId = 2, MoveUp = _moveUp, MoveDown = _moveDown, Parent = this });
            Cols.Add(new ColData() { ColId = 3, MoveUp = _moveUp, MoveDown = _moveDown, Parent = this });
            Cols.Add(new ColData() { ColId = 4, MoveUp = _moveUp, MoveDown = _moveDown, Parent = this });

            ShuffleCommand = new DelegateCommand(Shuffle);

            model.Init();
            model.Shuffle();
            model.TileColorChanged += Model_TileColorChanged;
        }

        private void Model_TileColorChanged(
#if NET5_0_OR_GREATER
            object?
#else
            object
# endif
            sender, EventArgs e)
        {
            for (int i = 0; i < _model.Height; i++)
            {
                var rd = Rows[i];
                rd.NotifyPropertyChanged(nameof(rd.TileColor));
            }
        }

        /// <summary>
        /// Gets or sets the shuffle command.
        /// </summary>
        /// <value>The shuffle command.</value>
        public DelegateCommand ShuffleCommand { get; set; }

        private IWpfCapModel _model;
        public IWpfCapModel Model => _model;

        /// <summary>
        /// Shuffles the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void Shuffle(
#if NET5_0_OR_GREATER
            object?
#else
            object
#endif
            obj=null)
        {
            _model?.Shuffle();
        }

        /// <summary>
        /// Does the move left.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DoMoveLeft(
#if NET5_0_OR_GREATER
            object?
#else
            object
#endif
            obj)
        {
            if (obj is RowData rd)
            {
                _model.MoveLeft(rd.RowId-1);
            }
            else
              throw new NotImplementedException();
        }

        /// <summary>
        /// Does the move right.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DoMoveRight(
#if NET5_0_OR_GREATER
            object?
#else
            object
#endif
            obj)
        {
            if (obj is RowData rd)
            {
                _model.MoveRight(rd.RowId-1);
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// Does the move up.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DoMoveUp(
#if NET5_0_OR_GREATER
            object?
#else
            object
#endif
            obj)
        {
            if (obj is ColData cd)
            {
                _model.MoveUp(cd.ColId-1);
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// Does the move down.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DoMoveDown(
#if NET5_0_OR_GREATER
            object?
#else
            object
#endif
            obj)
        {
            if (obj is ColData cd)
            {
                _model.MoveDown(cd.ColId-1);
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public ObservableCollection<RowData> Rows { get; set; }
        /// <summary>
        /// Gets or sets the cols.
        /// </summary>
        /// <value>The cols.</value>
        public ObservableCollection<ColData> Cols { get; set; }
    }
}
