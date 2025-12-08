using AA09_DialogBoxes.ViewModels;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AA09_DialogBoxes.Views;

public partial class DialogView : UserControl
{
    public DialogView()
    {
        InitializeComponent();
        DataContext ??= new DialogViewModel();
    }
}
