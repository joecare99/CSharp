// ***********************************************************************
// Assembly         : MVVM_27_DataGrid
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;

namespace MVVM_27_DataGrid.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class MainWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {

        }

#if !NET5_0_OR_GREATER
        // Finalizer wird bei .net-Core nicht (mehr) ausgeführt
        /// <summary>
        /// Finalizes an instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        ~MainWindowViewModel()
        {
            return;
        }
#endif
    }
}
