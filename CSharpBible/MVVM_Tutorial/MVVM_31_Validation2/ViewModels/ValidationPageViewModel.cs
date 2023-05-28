using MVVM.ViewModel;
using MVVM_BaseLib.Helper.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
                VHelper.AddError(property, "Username may not be empty");
            else if (arg1.Length < 6)
                VHelper.AddError(property, "Username must have min. 6 Chars");
//else 
               return true;
 //           return false;
        }

    }
}
