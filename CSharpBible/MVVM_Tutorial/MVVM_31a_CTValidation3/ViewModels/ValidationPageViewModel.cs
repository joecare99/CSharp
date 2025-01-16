using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using BaseLib.Helper.MVVM;
using MVVM.ViewModel;
using MVVM_31a_CTValidation3.Properties;
using MVVM_31a_CTValidation3.Validator;
using CommunityToolkit.Mvvm.Input;

namespace MVVM_31a_CTValidation3.ViewModels;

[NotifyDataErrorInfo]
public partial class ValidationPageViewModel : BaseViewModelCT
{
    private static ValidationPageViewModel? This { get; set; }
    
    [ObservableProperty]
    [MinLength(6, ErrorMessageResourceName = "Err_MustHave_Chars", ErrorMessageResourceType = typeof(Resources))]
    [Required(ErrorMessageResourceName = "Err_MayNotBeEmpty", ErrorMessageResourceType = typeof(Resources))]
    [NotTheSpecData("BlaBla", ErrorMessageResourceName = "Err_MayNotBeKnown", ErrorMessageResourceType = typeof(Resources))]
    [CustomValidation(typeof(ValidationPageViewModel),nameof(TestUsername), ErrorMessageResourceName = "Err_Something", ErrorMessageResourceType = typeof(Resources))]
    [NotifyPropertyChangedFor(nameof(TTUserName))]
    [NotifyCanExecuteChangedFor(nameof(UserLoginCommand))]
    private string _userName = ".";

    public static ValidationResult? TestUsername(object value) => This?._testUsername(value);

    private ValidationResult? _testUsername(object value)
    {
        return value is not string s 
            || s is not ("Dududu" or "aaaaaa" or "bbbbbb") ? null : new ValidationResult(null);
    }

    partial void OnUserNameChanging(string value) => This = this;

    public string? TTUserName => this.ValidationText();

    bool xCanLogin => !HasErrors && UserName!=".";

    [RelayCommand(CanExecute =nameof(xCanLogin))]
    private void UserLogin()
    {
        // Todo:
    }

    public ValidationPageViewModel()
    {            
    }
}
