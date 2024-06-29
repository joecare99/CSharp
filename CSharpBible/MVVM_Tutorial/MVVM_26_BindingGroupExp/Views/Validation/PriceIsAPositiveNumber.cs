using System.Globalization;
using System.Windows.Controls;

namespace MVVM_26_BindingGroupExp.Views.Validation
{
    public class PriceIsAPositiveNumber : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string s && decimal.TryParse(s,NumberStyles.Float,cultureInfo,out decimal d))
                return new ValidationResult(d > 0, $"Value has to be positive or zero");
            return new ValidationResult(false,"Argument has wrong type");
        }
    }
}
