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
using MVVM.ViewModel;

namespace MVVM_24_UserControl.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public class UserControlViewModel : BaseViewModel
{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserControlViewModel"/> class.
		/// </summary>
		public UserControlViewModel(){}

    private string _text1 = "Hello World";
    
    public string Text1
    {
        get => _text1;
        set => SetProperty(ref _text1, value);
    }


    public string Text2 { get; set; } = "Hello World 2";

		/// <summary>
		/// Finalizes an instance of the <see cref="UserControlViewModel"/> class.
		/// </summary>
		~UserControlViewModel()
    {
        return;
       
    }

}
