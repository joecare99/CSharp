// ***********************************************************************
// Assembly         : WpfDemoUI2
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 06-17-2022
// ***********************************************************************
// <copyright file="ShellViewModel.cs" company="WpfDemoUI2">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using DemoLibrary.Models;
using DemoLibrary;
using MVVM.ViewModel;
using System.Linq;

namespace WpfDemoUI.ViewModels {
	/// <summary>
	/// Class ShellViewModel.
	/// Implements the <see cref="BaseViewModel" />
	/// </summary>
	/// <seealso cref="BaseViewModel" />
	public class ShellViewModel : BaseViewModel {

		/// <summary>
		/// Gets or sets the people.
		/// </summary>
		/// <value>The people.</value>
		public BindableCollection<PersonModel> People { get; set; }
		/// <summary>
		/// Gets or sets the addresses.
		/// </summary>
		/// <value>The addresses.</value>
		public BindableCollection<AddressModel> Addresses { get; set; }

		/// <summary>
		/// The selected person
		/// </summary>
		private PersonModel? _selectedPerson;
		/// <summary>
		/// Gets or sets the selected person.
		/// </summary>
		/// <value>The selected person.</value>
		public PersonModel? SelectedPerson {
			get => _selectedPerson;
			set {
				_selectedPerson = value;
				if (value != null) 
				Addresses = new BindableCollection<AddressModel>(value.Addresses);
				else
                    Addresses = new BindableCollection<AddressModel>();
                SelectedAddress = value?.PrimaryAddress;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(Addresses));
			}
		}

		/// <summary>
		/// The selected address
		/// </summary>
		private AddressModel? _selectedAddress;
		/// <summary>
		/// Gets or sets the selected address.
		/// </summary>
		/// <value>The selected address.</value>
		public AddressModel? SelectedAddress {
			get => _selectedAddress;
			set {
				_selectedAddress = value;
				if (SelectedPerson !=null)
					SelectedPerson.PrimaryAddress = value;
				RaisePropertyChanged();
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ShellViewModel"/> class.
		/// </summary>
		public ShellViewModel() {
			//DataAccess da = new DataAccess();
			People = new BindableCollection<PersonModel>(DataAccess.GetPeople());
			Addresses = new BindableCollection<AddressModel>();
		}

		/// <summary>
		/// Adds the person.
		/// </summary>
		public void AddPerson() {

			int maxId = People.Max(x => x.PersonId);
			People.Add(DataAccess.GetPerson(maxId+1));
		}	
	}
}
