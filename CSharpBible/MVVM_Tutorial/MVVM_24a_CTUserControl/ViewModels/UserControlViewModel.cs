// ***********************************************************************
// Assembly         : WpfApp
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="UserControlViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace MVVM_24a_CTUserControl.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    [NotifyDataErrorInfo]
    public partial class UserControlViewModel : BaseViewModelCT
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="UserControlViewModel"/> class.
		/// </summary>
		public UserControlViewModel(){ }

        [ObservableProperty]
        [Required(AllowEmptyStrings =false,ErrorMessageResourceName ="Err_PropRequired", ErrorMessageResourceType =typeof(Properties.Resources))]
        private string _text1 = "Hello World";

		[ObservableProperty]
		[Required()]
		private string _text2 = "Hello World 2";

		/// <summary>
		/// Finalizes an instance of the <see cref="UserControlViewModel"/> class.
		/// </summary>
		~UserControlViewModel()
        {
            return;
        }

    }
}
