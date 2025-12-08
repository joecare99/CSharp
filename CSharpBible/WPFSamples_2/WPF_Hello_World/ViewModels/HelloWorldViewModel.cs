// ***********************************************************************
// Assembly         : WPF_Hello_World
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="HelloWorldViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System.ComponentModel;
using WPF_Hello_World.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF_Hello_World.Properties;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace WPF_Hello_World.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class HelloWorldViewModel : BaseViewModelCT
    {
        #region Properties
        private IHelloWorldModel model;

        [ObservableProperty]
        private string _greeting;
        #endregion

        #region Methods
        public HelloWorldViewModel() : this(Ioc.Default.GetRequiredService<IHelloWorldModel>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public HelloWorldViewModel(IHelloWorldModel model)
        {
            this.model = model;
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Greeting))
                Greeting = Resources.ResourceManager.GetString($"{model.Greeting}Text");
        }

        #endregion
    }
}
