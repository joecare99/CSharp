using MVVM_26_CTBindingGroupExp.Properties;
using System.Globalization;
using System.Windows.Controls;

namespace MVVM_26_CTBindingGroupExp.View.Validation;

public class PriceIsAPositiveNumber : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is string s && decimal.TryParse(s,NumberStyles.Float,cultureInfo,out decimal d))
            return new ValidationResult(d > 0, Resources.Err_MustbePositive);
        return new ValidationResult(false, Resources.Err_HasWrongType);
    }
}
