using MVVM.ViewModel;

namespace MVVM_16_UserControl_2.ViewModel
{
    public class UserControlViewViewModel : BaseViewModel
    {
		private string _text="";

		public UserControlViewViewModel()
        {
            return;
        }

		public string Text { get => _text; 
            set => SetProperty(ref _text,value); }

	}
}
