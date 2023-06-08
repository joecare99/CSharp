using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using BaseLib.Helper.MVVM;
using MVVM.ViewModel;
using MVVM_31a_CTValidation3.Properties;

namespace MVVM_31a_CTValidation3.ViewModels
{
    [NotifyDataErrorInfo]
    public partial class ValidationPageViewModel : BaseViewModelCT
    {
        [ObservableProperty]
        [MinLength(6, ErrorMessageResourceName = "Err_MustHave_Chars", ErrorMessageResourceType = typeof(Resources))]
        [Required(ErrorMessageResourceName = "Err_MayNotBeEmpty", ErrorMessageResourceType = typeof(Resources))]
        [NotifyPropertyChangedFor(nameof(TTUserName))]
        private string _userName = ".";

        public string? TTUserName => this.ValidationText();
        public ValidationPageViewModel()
        {
        }
    }
}
