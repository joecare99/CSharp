using Gen_FreeWin.ViewModels.Interfaces;

namespace Gen_FreeWin.Views;

public sealed class NamenSuchViewAdapter : INamenSuchViewAdapter
{
    public Namensuch Form { get; }

    public NamenSuchViewAdapter(Namensuch form)
    {
        Form = form;
    }
}
