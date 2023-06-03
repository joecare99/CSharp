using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_31a_CTValidation1.ViewModels
{
    
    public partial class ValidationPageViewModel : BaseViewModelCT
    {
        [ObservableProperty]
        private string _userName=".";

        partial void OnUserNameChanging(string? oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
                throw new ArgumentNullException("Username may not be empty");
            if (newValue.Length < 6)
                throw new ArgumentException("Username must have min. 6 Chars");
        }
    }
}
