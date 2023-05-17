using MVVM.ViewModel;

namespace MVVM_16_UserControl1.ViewModels
{
    public class CurrencyViewViewModel : BaseViewModel
    {
        private decimal _value;
        public decimal Value { get => _value;
            set
            { if (_value == value) return; _value = value; RaisePropertyChanged(); } 
        }

        public CurrencyViewViewModel()
        {
            Value = 10;
        }

    }
}
