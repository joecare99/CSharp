using System;
using System.Globalization;
using System.Windows.Controls;

namespace MVVM_26_BindingGroupExp.View.Validation
{
    public class FutureDateRule : ValidationRule
    {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string s && DateTime.TryParse(s, cultureInfo,DateTimeStyles.None , out var d))
                return new ValidationResult(DateTime.Compare(DateTime.Now, d)<0, $"Date has to be in the future");
            return new ValidationResult(false, "Argument has wrong type");
        }

    }
}
