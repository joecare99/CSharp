using MVVM_07a_CTDialogBoxes.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_07a_CTDialogBoxes.View
{
    public interface IDialogWindow
    {
        object DataContext { get; }

        bool? ShowDialog();
    }
}
