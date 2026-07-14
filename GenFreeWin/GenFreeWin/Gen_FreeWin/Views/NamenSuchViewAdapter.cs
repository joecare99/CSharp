using GenFreeWin.ViewModels.Interfaces;

namespace GenFreeWin.Views;

public sealed class NamenSuchViewAdapter : INamenSuchViewAdapter
{
    public Namensuch Form { get; }

    public NamenSuchViewAdapter(Namensuch form)
    {
        Form = form;
    }
}
