// ***********************************************************************
// Assembly         : WPF_Hello_World
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
using CommunityToolkit.Mvvm.DependencyInjection;
using MVVM.ViewModel;
using WPF_Hello_World.Models.Interfaces;

namespace WPF_Hello_World.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class MainWindowViewModel : BaseViewModelCT
    {
        private IHelloWorldModel model;
        #region Properties
        #endregion
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel():this(Ioc.Default.GetService<IHelloWorldModel>())
        {        }

        public MainWindowViewModel(IHelloWorldModel model)
        {
            this.model = model;
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

        internal void Closing()
        {
            model?.ClosingCommand.Execute(null);
        }
        #endregion
    }
}
