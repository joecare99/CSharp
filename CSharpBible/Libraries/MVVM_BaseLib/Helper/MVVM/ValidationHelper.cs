using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Helper.MVVM
{
    public class ValidationHelper :INotifyDataErrorInfo
    {
        private readonly IDictionary<string, List<ValidationResult>> _errorList = new Dictionary<string, List<ValidationResult>>();

        public string? this[string property]=> _errorList.TryGetValue(property, out var l) ? l!.First().ErrorMessage :null;
        public bool HasErrors => _errorList.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
            => _errorList.TryGetValue(propertyName ??"", out var l) ? l : new List<ValidationResult>();

        public void AddError(string property, string message)
        {
            if (_errorList.TryGetValue(property, out var lErr))
                lErr.Add(new ValidationResult(message, new[] {property }));
            else
                _errorList[property] = new List<ValidationResult>() { new ValidationResult(message,new[] { property }) };
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
        }

        public void ClearErrors(string property)
        {
            _errorList.Remove(property);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
        }
    }
}
