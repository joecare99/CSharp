using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFreeBrowser.ViewModels;

public partial class FraPersonenListViewModel:BaseViewModelCT
{
    [ObservableProperty]
    public partial ObservableCollection<DispPersones> Personen { get; set; }
}
