using MVVM.ViewModel;

namespace MVVM_16_UserControl_1.ViewModel
{
    public class UserControlViewViewModel : BaseViewModel
    {
		private string _text="";

        private string _data = "";
		public string Text { get => _text; set => SetProperty(ref _text, value); }
        public string Daten { get => _data; set => SetProperty(ref _data, value); }

        public UserControlViewViewModel()
        {
            return;
        }


    }
}
