// ***********************************************************************
// Assembly         : MVVM_22_WpfCap
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="WpfCapViewModel.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int[] TileColor { get; set; }
        /// <summary>
        /// Gets the this.
        /// </summary>
        /// <value>The this.</value>
        public object This => this;
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
        public int this[int ix] { 
            get => Parent.Rows[ix].TileColor[ColId-1]; 
            set => Parent.Rows[ix].TileColor[ColId-1] = value ; }
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
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfCapViewModel"/> class.
        /// </summary>
        public WpfCapViewModel()
        {
            var _moveLeft = new DelegateCommand(DoMoveLeft);
            var _moveRight = new DelegateCommand(DoMoveRight);
            Rows = new ObservableCollection<RowData>();
            Rows.Add(new RowData() { RowId = 1, MoveLeft = _moveLeft, MoveRight = _moveRight, TileColor = new int[4] });
            Rows.Add(new RowData() { RowId = 2, MoveLeft = _moveLeft, MoveRight = _moveRight, TileColor = new int[4] });
            Rows.Add(new RowData() { RowId = 3, MoveLeft = _moveLeft, MoveRight = _moveRight, TileColor = new int[4] });
            Rows.Add(new RowData() { RowId = 4, MoveLeft = _moveLeft, MoveRight = _moveRight, TileColor = new int[4] });

            var _moveUp = new DelegateCommand(DoMoveUp);
            var _moveDown = new DelegateCommand(DoMoveDown);
            Cols = new ObservableCollection<ColData>();
            Cols.Add(new ColData() { ColId = 1, MoveUp = _moveUp, MoveDown = _moveDown, Parent = this });
            Cols.Add(new ColData() { ColId = 2, MoveUp = _moveUp, MoveDown = _moveDown, Parent = this });
            Cols.Add(new ColData() { ColId = 3, MoveUp = _moveUp, MoveDown = _moveDown, Parent = this });
            Cols.Add(new ColData() { ColId = 4, MoveUp = _moveUp, MoveDown = _moveDown, Parent = this });

            ShuffleCommand = new DelegateCommand(Shuffle);

            Init();
            Shuffle();
        }

        /// <summary>
        /// Gets or sets the shuffle command.
        /// </summary>
        /// <value>The shuffle command.</value>
        public DelegateCommand ShuffleCommand { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Init()
        {
            foreach (var row in Rows )
                for (int i = 0; i < row.TileColor.Length; i++)
                    row.TileColor[i] = i+1;
        }

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
            var rnd = new Random();
            for (int i=0;i< 100;i++)
            {
                var move = rnd.Next(16);
                switch(move % 4){
                    case 0:
                        DoMoveRight(Rows[move / 4]);
                        break;
                    case 1:
                        DoMoveLeft(Rows[move / 4]);
                        break;
                    case 2:
                        DoMoveUp(Cols[move / 4]);
                        break;
                    case 3:
                        DoMoveDown(Cols[move / 4]);
                        break;

                }
            }
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
                var s = rd.TileColor[0];
                for (int i = 1; i < rd.TileColor.Length; i++)
                    rd.TileColor[i - 1] = rd.TileColor[i];
                rd.TileColor[rd.TileColor.Length - 1] = s;
                rd.NotifyPropertyChanged(nameof(rd.TileColor));
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
                var s=rd.TileColor[rd.TileColor.Length - 1];
                for (int i = rd.TileColor.Length-1; i > 0; i--)
                    rd.TileColor[i] = rd.TileColor[i-1];
                rd.TileColor[0]=s;

                rd.NotifyPropertyChanged(nameof(rd.TileColor));
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
                var s = cd[0];
                for (int i = 1; i < cd.Length; i++)
                    cd[i - 1] = cd[i];
                cd[cd.Length - 1] = s;

                for (int i = 0; i < cd.Length; i++)
                    Rows[i].NotifyPropertyChanged("TileColor");
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
                var s = cd[cd.Length - 1];
                for (int i = cd.Length - 1; i > 0; i--)
                    cd[i] = cd[i - 1];
                cd[0] = s;

                for (int i = 0; i < cd.Length; i++)
                    Rows[i].NotifyPropertyChanged("TileColor");
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
