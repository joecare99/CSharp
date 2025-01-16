// ***********************************************************************
// Assembly         : MVVM_17_1_CSV_Laden
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 08-13-2022
// ***********************************************************************
// <copyright file="DataPointsViewModel.cs" company="MVVM_17_1_CSV_Laden">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.ObjectModel;
using System.Drawing;
using MVVM.ViewModel;
using MVVM_17_1_CSV_Laden.Model;

namespace MVVM_17_1_CSV_Laden.ViewModels;


/// <summary>
/// Class DataPointsViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public class DataPointsViewModel : BaseViewModel
{
    /// <summary>
    /// The view port
    /// </summary>
    private SWindowPort _viewPort;

    /// <summary>
    /// Gets or sets the window port.
    /// </summary>
    /// <value>The window port.</value>
    public SWindowPort WindowPort { get => _viewPort; set => SetProperty(ref _viewPort, value); }

    /// <summary>
    /// Gets or sets the vp window.
    /// </summary>
    /// <value>The vp window.</value>
    public RectangleF VPWindow { get => _viewPort.port; set => SetProperty(ref _viewPort.port, value, new string[] { nameof(WindowPort), nameof(DataPoints) }); }
    /// <summary>
    /// Gets or sets the size of the window.
    /// </summary>
    /// <value>The size of the window.</value>
    public System.Windows.Size WindowSize
    {
        get => _viewPort.WindowSize;
        set => SetProperty(ref _viewPort.WindowSize, value, new string[] { nameof(WindowPort), nameof(DataPoints) });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPointsViewModel"/> class.
    /// </summary>
    public DataPointsViewModel()
    {
        LoadCsV = new DelegateCommand((o) => ExecLoadCsV());
        DataPoints = new ObservableCollection<DataPoint>();
        IsLoading = false;

        VPWindow = new RectangleF(-3, -3, 9, 6);
        //           VPWindow = new RectangleF(-3, -3, 900, 600);
        //           VPWindow = new RectangleF(-0.03f, -0.03f, 0.09f, 0.06f);
        WindowSize = new System.Windows.Size(300, 400);
        _viewPort.Parent = this;

    }

    /// <summary>
    /// Gets or sets the data points.
    /// </summary>
    /// <value>The data points.</value>
    public ObservableCollection<DataPoint> DataPoints  { get; set; }

    /// <summary>
    /// Executes the load cs v.
    /// </summary>
    private async void  ExecLoadCsV()
    {
        IsLoading = true;
        using (var service = new CsvModel("Resources\\RBG_XIst_YIst.csv"))
        {
            var result = service.ReadCSV();
            await foreach (var item in result)
                DataPoints.Add(item);
        }
        var max = new PointF((float)DataPoints[0].X, (float)DataPoints[0].Y);
        var min = new PointF(max.X,max.Y);
        foreach (var item in DataPoints)
        {
            if (item.X > max.X) max.X = (float)item.X;
            if (item.Y > max.Y) max.Y = (float)item.Y;
            if (item.X < min.X) min.X = (float)item.X;
            if (item.Y < min.Y) min.Y = (float)item.Y;
        }
        VPWindow = new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
        IsLoading = false;
        RaisePropertyChanged(nameof(DataPoints));
    }

    /// <summary>
    /// The is loading
    /// </summary>
    private bool _isLoading;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is loading.
    /// </summary>
    /// <value><c>true</c> if this instance is loading; otherwise, <c>false</c>.</value>
    public bool IsLoading {
        get { return _isLoading; }
        set { if (_isLoading != value) return; _isLoading = value; RaisePropertyChanged(); }
    }

    /// <summary>
    /// Gets or sets the load cs v.
    /// </summary>
    /// <value>The load cs v.</value>
    public DelegateCommand LoadCsV { get; set; } = new DelegateCommand(
        (o) => { },
        (o)=>false
        );
}
