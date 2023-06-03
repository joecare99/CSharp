using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;

namespace MVVM_03a_CTNotifyChange.ViewModels
{
    public partial class NotifyChangeViewModel : BaseViewModelCT
    {
        #region Properties
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Fullname))]
        private string _firstname;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Fullname))]
        private string _lastname;

         public string Fullname => $"{Lastname}, {Firstname}";
        #endregion

        #region Methods
        public NotifyChangeViewModel()
        {
            _firstname = "Dave";
            _lastname = "Dev";
        }
        #endregion

    }
}
