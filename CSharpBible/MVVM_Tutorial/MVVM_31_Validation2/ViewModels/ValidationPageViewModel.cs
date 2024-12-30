using MVVM.ViewModel;
using BaseLib.Helper.MVVM;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM_31_Validation2.ViewModels
{
    public class ValidationPageViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private string _userName = "";

        public ValidationHelper VHelper { get; } =new ValidationHelper();
        public string UserName { get => _userName; set => SetProperty(ref _userName, value,validate:s=> TestUsername(s),null); }

        public bool HasErrors => VHelper.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public ValidationPageViewModel()
        {
            VHelper.ErrorsChanged += (_,e)=>
            {
                RaisePropertyChanged(nameof(VHelper));
                ErrorsChanged?.Invoke(this, e);
            };
        }
        public IEnumerable GetErrors(string? propertyName) 
            => VHelper.GetErrors(propertyName);

        public bool TestUsername(string arg1,[CallerMemberName] string property="")
        {
            VHelper.ClearErrors(property);
            if (string.IsNullOrEmpty(arg1))
                VHelper.AddError(property, Properties.Resources.Err_MayNotBeEmpty);
            else if (arg1.Length < 6)
                VHelper.AddError(property, Properties.Resources.Err_MustHave6Chars);
//else 
               return true;
 //           return false;
        }

    }
}
