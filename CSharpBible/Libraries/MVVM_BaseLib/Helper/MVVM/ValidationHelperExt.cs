using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BaseLib.Helper.MVVM
{
    public static class ValidationHelperExt
    {
        public static string? ValidationText(this INotifyDataErrorInfo vm,[CallerMemberName] string? property = null)
        {
            if (vm == null) throw new ArgumentNullException(nameof(vm));
            if (string.IsNullOrEmpty(property)) throw new ArgumentNullException(nameof(property));
            var l = (vm.GetErrors(property!.TrimStart('T')) as List<ValidationResult>)?.ConvertAll(o => o.ErrorMessage);
            if (l?.Count>0) 
                return string.Join(", ",l);
            else
                return null;
        }

    }
}
