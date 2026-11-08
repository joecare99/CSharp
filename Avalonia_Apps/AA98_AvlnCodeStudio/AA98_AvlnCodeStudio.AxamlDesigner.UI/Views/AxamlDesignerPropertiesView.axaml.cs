using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;

public partial class AxamlDesignerPropertiesView : UserControl
{
    private readonly DispatcherTimer _propertyApplyTimer;
    private AxamlPropertyRowViewModel? _pendingPropertyRow;
    private AxamlDesignerViewModel? _observedViewModel;
    private bool _isAttached;

    public AxamlDesignerPropertiesView()
    {
        InitializeComponent();
        _propertyApplyTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(350) };
        _propertyApplyTimer.Tick += OnPropertyApplyTimerTick;
        DataContextChanged += OnDataContextChanged;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        ObserveDataContext();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        ObserveDataContext();
        base.OnDetachedFromVisualTree(e);
    }

    private void OnDiscardPropertyDraft(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: AxamlPropertyRowViewModel row }
            && DataContext is AxamlDesignerViewModel viewModel)
        {
            if (ReferenceEquals(_pendingPropertyRow, row))
            {
                _propertyApplyTimer.Stop();
                _pendingPropertyRow = null;
            }

            viewModel.DiscardPropertyDraft(row);
        }
    }

    private void OnApplyPropertyDraft(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: AxamlPropertyRowViewModel row }
            && DataContext is AxamlDesignerViewModel viewModel)
        {
            _propertyApplyTimer.Stop();
            _pendingPropertyRow = null;
            viewModel.ApplyPropertyRow(row, finalize: true);
        }
    }

    private void OnRestoreStoredDraft(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: AxamlPropertyDraft draft }
            && DataContext is AxamlDesignerViewModel viewModel)
        {
            viewModel.RestoreDraft(draft);
        }
    }

    private void OnDiscardStoredDraft(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: AxamlPropertyDraft draft }
            && DataContext is AxamlDesignerViewModel viewModel)
        {
            viewModel.DiscardStoredDraft(draft);
        }
    }

    private async void OnApplyBinding(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: AxamlBindingRowViewModel row }
            && DataContext is AxamlDesignerViewModel viewModel)
        {
            await viewModel.ApplyBindingRowAsync(row);
        }
    }

    private void OnPropertyApplyTimerTick(object? sender, EventArgs e)
    {
        _propertyApplyTimer.Stop();
        AxamlPropertyRowViewModel? row = _pendingPropertyRow;
        _pendingPropertyRow = null;
        if (row is not null && DataContext is AxamlDesignerViewModel viewModel)
        {
            viewModel.ApplyPropertyRow(row, finalize: false);
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e) => ObserveDataContext();

    private void ObserveDataContext()
    {
        if (_observedViewModel is not null)
        {
            _observedViewModel.PropertyDraftChanged -= OnPropertyDraftChanged;
        }

        _propertyApplyTimer.Stop();
        _pendingPropertyRow = null;
        _observedViewModel = _isAttached ? DataContext as AxamlDesignerViewModel : null;
        if (_observedViewModel is not null)
        {
            _observedViewModel.PropertyDraftChanged += OnPropertyDraftChanged;
        }
    }

    private void OnPropertyDraftChanged(AxamlPropertyRowViewModel row)
    {
        if (DataContext is not AxamlDesignerViewModel viewModel)
        {
            return;
        }

        viewModel.BeginPropertyDraft(row, hasChanged: true);
        _pendingPropertyRow = row;
        _propertyApplyTimer.Stop();
        _propertyApplyTimer.Start();
    }
}
