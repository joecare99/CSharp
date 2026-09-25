using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class MainForm : Form
{
    private ModernHostViewModel? _viewModel;

    public MainForm()
    {
        InitializeComponent();
    }

    public MainForm(ModernHostViewModel viewModel)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        InitializeComponent();

        _tenantStatusLabel.ForeColor = _viewModel.IsReady ? SystemColors.ControlText : Color.DarkRed;

        global::Views.ViewBinding.Commit(this, _viewModel);
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        ShowActivePage();
    }


    private void ShowActivePage()
    {
        if (_viewModel is null)
        {
            return;
        }

        _pageHost.Controls.Clear();

        switch (_viewModel.ActivePageViewModel)
        {
            case OFBMenuViewModel menuViewModel:
                _pageHost.Controls.Add(new OFBMenuPageView(menuViewModel));
                break;
            case PlaceSelectionViewModel placeSelectionViewModel:
                _pageHost.Controls.Add(new PlaceSelectionPageView(placeSelectionViewModel));
                break;
            case PublicationSettingsViewModel publicationSettingsViewModel:
                _pageHost.Controls.Add(new PublicationSettingsPageView(publicationSettingsViewModel));
                break;
            case FamilyPublicationViewModel familyPublicationViewModel:
                _pageHost.Controls.Add(new FamilyPublicationPageView(familyPublicationViewModel));
                break;
            case StatisticsViewModel statisticsViewModel:
                _pageHost.Controls.Add(new StatisticsPageView(statisticsViewModel));
                break;
            default:
                if (_viewModel.ActivePageViewModel != null)
                {
                    _pageHost.Controls.Add(new HostPageView(_viewModel.ActivePageViewModel));
                }

                break;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(ModernHostViewModel.ActivePageViewModel))
        {
            ShowActivePage();
        }
    }
}
