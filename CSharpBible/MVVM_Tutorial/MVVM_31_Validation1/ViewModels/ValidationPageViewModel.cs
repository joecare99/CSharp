using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_31_Validation1.ViewModels
{
    public class ValidationPageViewModel : BaseViewModel
    {
        private string _userName="";

        public string UserName { get => _userName; set => SetProperty(ref _userName, value,TestUsername); }

        private bool TestUsername( string arg1)
        {
            if (string.IsNullOrEmpty(arg1))
               throw new ArgumentNullException("Username may not be empty");
            if (arg1.Length<6)
                throw new ArgumentException("Username must have 6 Chars");
            return true;
        }
    }
}
