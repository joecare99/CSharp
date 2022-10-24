using MVVM.ViewModel;
using System;

namespace MVVM_16_UserControl_1.ViewModel
{
    public class UserControlViewViewModel : BaseViewModel
    {
		private string _text="<Ein Motto>";

        private string _data = "<Daten>";
		public string Text { get => _text; set => SetProperty(ref _text, value); }
        public string Daten { get => _data; set => SetProperty(ref _data, value); }

        public DelegateCommand Command1 { get; set; }
        public DelegateCommand Command2 { get; set; }

        public UserControlViewViewModel()
        {
            Command1 = new DelegateCommand(DoCommand1, (o) => string.IsNullOrEmpty(Text));
            CommandCanExecuteBinding.Add((nameof(Text), nameof(Command1)));

            Command2 = new DelegateCommand(DoCommand2, (o) => string.IsNullOrEmpty(Daten));
            CommandCanExecuteBinding.Add((nameof(Daten), nameof(Command2)));
        }

        private void DoCommand1(object? obj)
        {
            Text = "<Motto>";
            Daten = "";
        }
        private void DoCommand2(object? obj)
        {
            Daten = "<Daten>";
            Text = "";
        }
    }
}
