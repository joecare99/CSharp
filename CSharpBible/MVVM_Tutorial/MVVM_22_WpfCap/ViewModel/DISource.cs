using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MVVM_22_WpfCap.ViewModel
{
    public class DISource : MarkupExtension
    {
        public static Func<Type, object> Resolver { get; set; }
        public Type Type { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider) => Resolver?.Invoke(Type);
    }
}
