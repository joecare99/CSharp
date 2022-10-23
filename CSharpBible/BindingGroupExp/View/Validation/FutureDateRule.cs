using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace BindingGroupExp.View.Validation
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
