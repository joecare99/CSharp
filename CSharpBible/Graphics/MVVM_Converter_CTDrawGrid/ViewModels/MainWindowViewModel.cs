﻿using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System;

namespace MVVM_Converter_CTDrawGrid.ViewModel
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class MainWindowViewModel:BaseViewModelCT
    {
        /// <summary>
        /// Gets or sets the show client.
        /// </summary>
        /// <value>The show client.</value>
        public Func<string, BaseViewModel?>? ShowClient { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {

        }

        /// <summary>
        /// Gets or sets the load level command.
        /// </summary>
        /// <value>The load level command.</value>
        [RelayCommand]
        private void LoadLevel() 
            => _drawGridModel.LoadLevel();
        /// <summary>
        /// Gets or sets the next level command.
        /// </summary>
        /// <value>The next level command.</value>
        [RelayCommand]
        private void NextLevel() 
            => _drawGridModel.NextLevel();
        /// <summary>
        /// Gets or sets the previous level command.
        /// </summary>
        /// <value>The previous level command.</value>
        [RelayCommand]
        private void PrevLevel() 
            => _drawGridModel.PrevLevel();

    }
}
