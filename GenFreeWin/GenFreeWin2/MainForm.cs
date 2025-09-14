using BaseLib.Helper;
using GenFreeWin.Views;
using GenFreeWin2.ViewModels.Interfaces;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFreeWin2;

public partial class MainForm : Form
{
    private IMainFormViewModel _viewModel;

    Control? _View;

    public MainForm(IMainFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.ShowDialog = viewModel_ShowDialog;
        this.IsMdiContainer = true;
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
        viewModel.View = IoC.GetRequiredService<Menue>();       
    }

    private void viewModel_ShowDialog(Control control)
    {
        control.Visible = true;
        control.BringToFront();
        if (control is Form f)
        {
            f.MdiParent = this;
            f.FormClosed += Dialog_OnFormClosed;
        }
        components.Add(control);
    }

    private void Dialog_OnFormClosed(object? sender, FormClosedEventArgs e)
    {
        components.Remove(sender as Control);
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IMainFormViewModel.View))
        {
            if (_View != null)
            {
                components.Remove(_View);
                _View.Hide();
            }
            _View = (sender as IMainFormViewModel).View;
            if (_View != null)
            {
                _View.Dock = DockStyle.Fill;
                _View.Visible = true;
                _View.BringToFront();
                if (_View is Form f)
                {
                    f.MdiParent = this;
                    this.Icon = f.Icon;
                }
                this.Text = _View.Text;
                components.Add(_View);
            }
        }
    }
}
