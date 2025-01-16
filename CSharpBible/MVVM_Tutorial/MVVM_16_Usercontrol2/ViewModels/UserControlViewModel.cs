using MVVM.ViewModel;

namespace MVVM_16_UserControl2.ViewModels;

public class UserControlViewModel : BaseViewModel
{
		private string _text="";

		public UserControlViewModel()
    {
        return;
    }

		public string Text { get => _text; 
        set => SetProperty(ref _text,value); }

	}
