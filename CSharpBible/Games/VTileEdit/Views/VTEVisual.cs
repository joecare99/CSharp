using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTileEdit.ViewModels;

namespace VTileEdit.Views;

public class VTEVisual : IVisual
{
    private IVTEViewModel _viewModel;

    public VTEVisual(IVTEViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.PropertyChanged += OnPropertyChanged;
    }

    public void HandleUserInput()
    {
        throw new NotImplementedException();
    }

    public bool ShowFileDialog(IFileDialogData fileDialog)
    {
        throw new NotImplementedException();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Find bound properties
    }
}
