using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;

namespace GenFreeAvln.Views;

public partial class FraPersImpQuerry : UserControl
{
    private readonly IApplUserTexts _strings;
    private readonly IFraPersImpQueryViewModel _viewModel;

    public FraPersImpQuerry(IFraPersImpQueryViewModel viewModel, IApplUserTexts strings)
    {
        _strings = strings;
        _viewModel = viewModel;
        DataContext = _viewModel;

        InitializeComponent();
        ApplyTexts();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ApplyTexts()
    {
        if (this.FindControl<GroupBox>("frmImport") is GroupBox frmImport)
        {
            frmImport.Header = _strings[_viewModel.IText];
        }

        if (this.FindControl<Button>("btnDeleteQuiet") is Button btnDeleteQuiet)
        {
            btnDeleteQuiet.Content = _strings[_viewModel.IDelete];
        }

        if (this.FindControl<Button>("btnCancel3") is Button btnCancel3)
        {
            btnCancel3.Content = _strings[_viewModel.ICancel];
        }

        if (this.FindControl<Button>("btnReenter") is Button btnReenter)
        {
            btnReenter.Content = _strings[_viewModel.IReenter];
        }

        if (this.FindControl<Button>("btnLoadFromFile") is Button btnLoadFromFile)
        {
            btnLoadFromFile.Content = _strings[_viewModel.ILoadFromFile];
        }
    }
}
