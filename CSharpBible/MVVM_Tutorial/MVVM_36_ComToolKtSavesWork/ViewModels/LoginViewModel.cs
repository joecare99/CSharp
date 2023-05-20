﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM_36_ComToolKtSavesWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_36_ComToolKtSavesWork.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;

        public LoginViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DoLoginCommand))]
        private string _userName="";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DoLoginCommand))]
        private string _password="";

        private bool _canDoLogin => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);

        [RelayCommand(CanExecute = nameof(_canDoLogin))]
        private void DoLogin()
        {
            var user = _userRepository.Login(UserName, Password);
            if (user != null)
            {

            }
        }
    }
}
