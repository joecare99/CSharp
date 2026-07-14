using GenFreeWin.Views;
using GenFree.Interfaces;
using System;
using System.Windows.Forms;

namespace GenFreeWin.ViewModels;

public partial class NamenSuchViewModel
{
    IContainerControl GenFree.ViewModels.Interfaces.INamenSuchViewModel.View { get; set; }

    private GenFreeWin.ViewModels.Interfaces.INamenSuchViewAdapter _viewAdapter;

    public void AttachViewAdapter(GenFreeWin.ViewModels.Interfaces.INamenSuchViewAdapter viewAdapter)
    {
        _viewAdapter = viewAdapter;
    }

    private Namensuch UiForm => _viewAdapter?.Form ?? Namensuch.Default;

    private FraNameSrchSelection fraNameSrchSelection1 => UiForm.fraNameSrchSelection1;

    private IDocument Document => UiForm.fraPreview1;

    [Obsolete]
    private PictureBox PictureBox1 => UiForm.PictureBox1;

    [Obsolete]
    private ComboBox ComboBox1 => UiForm.ComboBox1;

    [Obsolete]
    private ComboBox ComboBox2 => UiForm.ComboBox2;

    [Obsolete]
    private GroupBox Frame3 => UiForm.Frame3;

    [Obsolete]
    private ListBox ListBox1 => UiForm.ListBox1;

    [Obsolete]
    private ListBox List1 => UiForm.List1;

    [Obsolete]
    private Cursor Cursor { get; set; }
}
