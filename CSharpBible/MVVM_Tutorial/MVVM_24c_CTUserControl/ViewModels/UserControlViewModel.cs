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

namespace MVVM_24c_CTUserControl.ViewModels;

	/// <summary>
	/// Class MainWindowViewModel.
	/// Implements the <see cref="BaseViewModel" />
	/// </summary>
	/// <seealso cref="BaseViewModel" />
	public partial class UserControlViewModel : BaseViewModelCT
	{
		[ObservableProperty]
		private string _text1 = "Hello World";

		[ObservableProperty]
		private string _text2 = "Hello World2";

		[ObservableProperty]
		private bool? _state1 = true;

		[ObservableProperty]
		private bool? _state2 = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserControlViewModel"/> class.
		/// </summary>
		public UserControlViewModel() { }

		/// <summary>
		/// Finalizes an instance of the <see cref="UserControlViewModel"/> class.
		/// </summary>
		~UserControlViewModel()
		{
			return;
		}

	}
