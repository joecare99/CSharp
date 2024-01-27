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
using MVVM.ViewModel;
using MVVM_17_1_CSV_Laden.Model;

namespace MVVM_17_1_CSV_Laden.ViewModel
{
    /// <summary>
    /// Class DataPointsViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class DataPointsViewModel : BaseViewModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointsViewModel"/> class.
        /// </summary>
        public DataPointsViewModel()
        {
            LoadCsV = new DelegateCommand((o) => ExecLoadCsV());
            DataPoints = new ObservableCollection<DataPoint>();
            IsLoading = false;
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
            using (var service = new CsvService("RBG_XIst_YIst.csv"))
            {
                var result = service.ReadCSV();
                await foreach (var item in result)
                    DataPoints.Add(item);
            }
            IsLoading = false;
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
}
