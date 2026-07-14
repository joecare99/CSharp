using GenFreeBrowser.Model;
using System.Collections.ObjectModel;

namespace GenFreeBrowser.ViewModels.Interfaces;

public interface IKernFamilieViewModel
{
    DispPersones? AusgewaehltePerson { get; set; }
    DispPersones? EhePartner { get; }
    ReadOnlyObservableCollection<DispPersones> Kinder { get; }
    bool HatEhePartner { get; }
    bool HatKinder { get; }
    void LadeFamilie();
}
