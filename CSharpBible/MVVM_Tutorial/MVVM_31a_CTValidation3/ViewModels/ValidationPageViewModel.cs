using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using MVVM_BaseLib.Helper.MVVM;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM_31a_CTValidation3.ViewModels
{
    public partial class ValidationPageViewModel : BaseViewModelCT, INotifyDataErrorInfo
    {
        [ObservableProperty]
        private string _userName = ".";

        public ValidationHelper VHelper { get; } =new ValidationHelper();

        public bool HasErrors => VHelper.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public ValidationPageViewModel()
        {
            VHelper.ErrorsChanged += (_,e)=>
            {
                OnPropertyChanged(nameof(VHelper));
                ErrorsChanged?.Invoke(this, e);
            };
        }
        public IEnumerable GetErrors(string? propertyName) 
            => VHelper.GetErrors(propertyName);

        partial void OnUserNameChanging(string value)
        {
            var property = nameof(UserName);
            VHelper.ClearErrors(property);
            if (string.IsNullOrEmpty(value))
                VHelper.AddError(property, "Username may not be empty");
            else if (value.Length < 6)
                VHelper.AddError(property, "Username must have min. 6 Chars");
        }
    }
}
