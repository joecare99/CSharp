using MVVM_26_CTBindingGroupExp.Properties;
using MVVM_26_CTBindingGroupExp.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MVVM_26_CTBindingGroupExp.View.Validation;

public class ValidateDateAndPrice : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
            // Get the source object.
        if (value is BindingGroup bg && bg.Items[0] is MainWindowViewModel item)
        {
            const decimal prLimit = 100m;
            const int iDAvail = 7;
            // Get the proposed values for Price and OfferExpires.
            bool priceResult = bg.TryGetValue(item, nameof(MainWindowViewModel.Price), out object doubleValue);
            bool dateResult = bg.TryGetValue(item, nameof(MainWindowViewModel.Date), out object dateTimeValue);
            if (!priceResult || !dateResult)
            {
                return new ValidationResult(false, Resources.Err_PropNotFound );
            }

            decimal price = (decimal)doubleValue;
            DateTime offerExpires = (DateTime)dateTimeValue;

            // Check that an item over $100 is available for at least 7 days.
            if (price > prLimit)
            {
                if (offerExpires < DateTime.Today + new TimeSpan(iDAvail, 0, 0, 0))
                {
                    return new ValidationResult(false, string.Format(Resources.Err_Itemsover1mustbe2a, prLimit, iDAvail));
                }
            }

        }
        return ValidationResult.ValidResult;
    }
}
