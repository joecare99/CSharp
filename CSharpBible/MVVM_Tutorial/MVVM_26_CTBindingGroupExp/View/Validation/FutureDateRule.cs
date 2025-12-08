using System;
using System.Globalization;
using System.Windows.Controls;
using MVVM_26_CTBindingGroupExp.Properties;

namespace MVVM_26_CTBindingGroupExp.View.Validation;

public class FutureDateRule : ValidationRule
{

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is string s && DateTime.TryParse(s, cultureInfo,DateTimeStyles.None , out var d))
            return new ValidationResult(DateTime.Compare(DateTime.Now, d)<0, Resources.Err_DateMustBeInFuture);
        return new ValidationResult(false, Resources.Err_HasWrongType);
    }

}
