using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Gen_FreeWin;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;
using System;

namespace GenFreeAvln.Views;

public partial class Lizenz : Window
{
    private readonly ILizenzViewModel _viewModel;
    private readonly IApplUserTexts _strings;

    public ILizenzViewModel ViewModel => _viewModel;

    public Lizenz(ILizenzViewModel viewModel, IApplUserTexts strings)
    {
        _viewModel = viewModel;
        _strings = strings;
        DataContext = _viewModel;

        _viewModel.DoClose = Close;

        InitializeComponent();
        Opened += Lizenz_Opened;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Lizenz_Opened(object? sender, EventArgs e)
    {
        if (this.FindControl<TextBlock>("lblEnterLicence") is TextBlock lblEnterLicence)
        {
            lblEnterLicence.Text = _strings[EUserText.t112];
        }

        if (this.FindControl<Button>("btnVerify") is Button btnVerify)
        {
            btnVerify.Content = _strings[EUserText.t113];
        }

        if (this.FindControl<Button>("btnCancel") is Button btnCancel)
        {
            btnCancel.Content = _strings[EUserText.tNMCancel];
        }
    }

    private void TextBox_OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (!int.TryParse(textBox.Tag?.ToString(), out int tag))
        {
            return;
        }

        if (string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        char ch = e.Text[0];
        bool isDigit = char.IsDigit(ch);

        if (tag == 0 && ch == '-')
        {
            e.Handled = true;
            return;
        }

        if (!isDigit)
        {
            e.Handled = true;
        }
    }

    private void TextBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (!int.TryParse(textBox.Tag?.ToString(), out int tag))
        {
            return;
        }

        switch (tag)
        {
            case 0:
                if ((textBox.Text?.Length ?? 0) >= 10 && this.FindControl<TextBox>("txtLicPart2") is TextBox txtLicPart2)
                {
                    txtLicPart2.Focus();
                }
                break;
            case 1:
                if ((textBox.Text?.Length ?? 0) >= 10 && this.FindControl<TextBox>("txtLicPart3") is TextBox txtLicPart3)
                {
                    txtLicPart3.Focus();
                }
                break;
            case 2:
                if ((textBox.Text?.Length ?? 0) >= 5 && this.FindControl<Button>("btnVerify") is Button btnVerify)
                {
                    btnVerify.Focus();
                }
                break;
        }
    }
}
