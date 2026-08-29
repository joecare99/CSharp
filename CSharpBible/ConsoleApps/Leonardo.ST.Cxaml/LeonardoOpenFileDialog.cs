using System.Threading;
using Leonardo.ViewModels.Interfaces;

namespace Leonardo.ST.Cxaml;

/// <summary>Provides Leonardo's model with an STA-safe native open-file dialog.</summary>
internal sealed class LeonardoOpenFileDialog : IOpenFileDialog
{
    private readonly OpenFileDialog _dialog = new();

    public string Filter
    {
        get => _dialog.Filter;
        set => _dialog.Filter = value;
    }

    public string? FileName
    {
        get => _dialog.FileName;
        set => _dialog.FileName = value;
    }

    public void Dispose() => _dialog.Dispose();

    public bool ShowDialog()
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
        {
            return _dialog.ShowDialog() == DialogResult.OK;
        }

        bool accepted = false;
        Thread thread = new(() => accepted = _dialog.ShowDialog() == DialogResult.OK);
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        return accepted;
    }
}
