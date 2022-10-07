using MVVM.ViewModel;

namespace MVVM_16_UserControl_1.ViewModel
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
