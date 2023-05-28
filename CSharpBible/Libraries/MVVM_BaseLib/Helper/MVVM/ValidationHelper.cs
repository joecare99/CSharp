using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_BaseLib.Helper.MVVM
{
    public class ValidationHelper :INotifyDataErrorInfo
    {
        private readonly IDictionary<string, List<string>> _errorList = new Dictionary<string, List<string>>();

        public string this[string property]=> _errorList.TryGetValue(property, out var l) ? l.First() :string.Empty;
        public bool HasErrors => _errorList.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
            => _errorList.TryGetValue(propertyName ??"", out var l) ? l : new List<string>();

        public void AddError(string property, string message)
        {
            if (_errorList.TryGetValue(property, out var lErr))
                lErr.Add(message);
            else
                _errorList[property] = new List<string>() { message };
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
        }

        public void ClearErrors(string property)
        {
            _errorList.Remove(property);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
        }
    }
}
