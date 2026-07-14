using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;

namespace GenFreeAvln.Views;

public partial class FraEventShowEdit : UserControl
{
    private readonly IEventShowEditViewModel _viewModel;
    private readonly IApplUserTexts _strings;

    public FraEventShowEdit(IEventShowEditViewModel viewModel, IApplUserTexts strings)
    {
        _viewModel = viewModel;
        _strings = strings;
        DataContext = _viewModel;

        InitializeComponent();

        _viewModel.DoClick = RaiseClick;
        UpdateHdr();

        if (this.FindControl<TextBlock>("lblEvent") is TextBlock hdr)
        {
            hdr.PointerPressed += Label_PointerPressed;
        }

        if (this.FindControl<TextBlock>("lblBirthDisp") is TextBlock value)
        {
            value.PointerPressed += Label_PointerPressed;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Label_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        RaiseClick();
    }

    private void RaiseClick()
    {
        _viewModel.ClickCommand.Execute(null);
    }

    private void UpdateHdr()
    {
        if (this.FindControl<TextBlock>("lblEvent") is TextBlock hdr)
        {
            hdr.Text = _strings[_viewModel.Display_Hdr];
        }
    }
}
