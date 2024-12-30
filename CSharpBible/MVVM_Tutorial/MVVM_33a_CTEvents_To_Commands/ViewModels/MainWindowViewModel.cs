// ***********************************************************************
// Assembly         : MVVM_33a_CTEvents_To_Commands
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

namespace MVVM_33a_CTEvents_To_Commands.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModelCT" />
    /// </summary>
    /// <seealso cref="BaseViewModelCT" />
    public partial class MainWindowViewModel : BaseViewModelCT
    {
        #region Properties
        #endregion
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {

        }

#if !NET5_0_OR_GREATER
        /// <summary>
        /// Finalizes an instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        ~MainWindowViewModel()
        {
            return;
        }
#endif
        #endregion
    }
}
