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

namespace MVVM_24b_UserControl.ViewModels
{
	/// <summary>
	/// Class MainWindowViewModel.
	/// Implements the <see cref="BaseViewModel" />
	/// </summary>
	/// <seealso cref="BaseViewModel" />
	public class UserControlViewModel : BaseViewModel
	{
		private string _text1 = "Hello World";
		public string Text1 { get => _text1; set { SetProperty(ref _text1, value); } }

		private string _text2 = "Hello World2";
		public string Text2 { get => _text2; set { SetProperty(ref _text2, value); } }

		private bool? _state1 = true;
		public bool? State1 { get => _state1; set { SetProperty(ref _state1 , value); } }

		private bool? _state2 = true;
		public bool? State2 { get => _state2; set { SetProperty(ref _state2 , value); } }

		/// <summary>
		/// Initializes a new instance of the <see cref="UserControlViewModel"/> class.
		/// </summary>
		public UserControlViewModel()
		{
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="UserControlViewModel"/> class.
		/// </summary>
		~UserControlViewModel()
		{
			return;
		}

	}
}
