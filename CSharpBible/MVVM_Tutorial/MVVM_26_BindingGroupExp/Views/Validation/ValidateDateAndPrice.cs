using MVVM_26_BindingGroupExp.ViewModel;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MVVM_26_BindingGroupExp.Views.Validation
{
    public class ValidateDateAndPrice : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
                // Get the source object.
            if (value is BindingGroup bg && bg.Items[0] is BindingGroupViewModel item)
            {

                // Get the proposed values for Price and OfferExpires.
                bool priceResult = bg.TryGetValue(item, "Price", out object doubleValue);
                bool dateResult = bg.TryGetValue(item, "OfferExpires", out object dateTimeValue);
                if (!priceResult || !dateResult)
                {
                    return new ValidationResult(false, "Properties not found");
                }

                decimal price = (decimal)doubleValue;
                DateTime offerExpires = (DateTime)dateTimeValue;

                // Check that an item over $100 is available for at least 7 days.
                if (price > 100)
                {
                    if (offerExpires < DateTime.Today + new TimeSpan(7, 0, 0, 0))
                    {
                        return new ValidationResult(false, "Items over $100 must be available for at least 7 days.");
                    }
                }

            }
            return ValidationResult.ValidResult;
        }
    }
}
