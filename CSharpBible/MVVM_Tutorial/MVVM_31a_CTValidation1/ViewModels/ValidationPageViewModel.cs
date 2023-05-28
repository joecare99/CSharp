using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_31a_CTValidation1.ViewModels
{
    public class ValidationPageViewModel : BaseViewModel
    {
        private string _userName="";

        public string UserName { get => _userName; set => SetProperty(ref _userName, value, TestUsername); }

        public bool TestUsername( string arg1)
        {
            if (string.IsNullOrEmpty(arg1))
               throw new ArgumentNullException("Username may not be empty");
            if (arg1.Length<6)
                throw new ArgumentException("Username must have min. 6 Chars");
            return true;
        }
    }
}
