using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;

namespace MVVM_04_DelegateCommand.ViewModels
{
    public class DelegateCommandViewModel : BaseViewModel
    {
        #region Properties
        private string _firstname;
        private string _lastname;

        public RelayCommand<object> ClearCommand { get; set; }
        public string Firstname { get => _firstname; set => SetProperty(ref _firstname, value); }
        public string Lastname { get => _lastname; set => SetProperty(ref _lastname , value); }
        public string Fullname => $"{Lastname}, {Firstname}";
        #endregion

        #region Methods
        public DelegateCommandViewModel()
        {
            _firstname = "Dave";
            _lastname = "Dev";
            AddPropertyDependency(nameof(Fullname), nameof(Lastname));
            AddPropertyDependency(nameof(Fullname), nameof(Firstname));
            AddPropertyDependency(nameof(ClearCommand), nameof(Lastname));
            AddPropertyDependency(nameof(ClearCommand), nameof(Firstname));
            ClearCommand = new(
                (o)=> { Firstname = ""; Lastname = ""; }, 
                (o) => !string.IsNullOrEmpty(_firstname) || !string.IsNullOrEmpty(_lastname));
        }
        #endregion

    }
}
