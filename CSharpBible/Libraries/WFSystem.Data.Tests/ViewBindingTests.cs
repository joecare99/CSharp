using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Views;

namespace WFSystem.Windows.Data.Tests;

[TestClass]
public sealed class ViewBindingTests
{
    [TestMethod]
    public void Commit_InitializesAnnotatedControlsFromViewModel()
    {
        var viewModel = new BindingViewModel();
        var view = new BindingView();

        ViewBinding.Commit(view, viewModel);

        Assert.AreEqual("Initial status", view.StatusLabel.Text);
        Assert.IsTrue(view.EnabledButton.Enabled);
        Assert.IsTrue(view.VisibleLabel.Visible);
        Assert.IsTrue(view.SelectedCheckBox.Checked);
        CollectionAssert.AreEqual(new object[] { "A", "B" }, view.ValuesListBox.Items);
    }

    [TestMethod]
    public void Commit_ReflectsViewModelPropertyChangesInControls()
    {
        var viewModel = new BindingViewModel();
        var view = new BindingView();
        ViewBinding.Commit(view, viewModel);

        viewModel.Status = "Updated status";
        viewModel.IsEnabled = false;
        viewModel.IsVisible = false;
        viewModel.IsSelected = false;
        viewModel.Values.Add("C");

        Assert.AreEqual("Updated status", view.StatusLabel.Text);
        Assert.IsFalse(view.EnabledButton.Enabled);
        Assert.IsFalse(view.VisibleLabel.Visible);
        Assert.IsFalse(view.SelectedCheckBox.Checked);
        CollectionAssert.AreEqual(new object[] { "A", "B", "C" }, view.ValuesListBox.Items);
    }

    [TestMethod]
    public void Commit_WritesEditableControlValuesBackToViewModel()
    {
        var viewModel = new BindingViewModel();
        var view = new BindingView();
        ViewBinding.Commit(view, viewModel);

        view.StatusLabel.Text = "Edited status";
        view.SelectedCheckBox.Checked = false;
        view.ValuesListBox.SelectedItem = "B";

        Assert.AreEqual("Edited status", viewModel.Status);
        Assert.IsFalse(viewModel.IsSelected);
        Assert.AreEqual("B", viewModel.SelectedValue);
    }

    [TestMethod]
    public void Commit_ConnectsCommandBinding()
    {
        var viewModel = new BindingViewModel();
        var view = new BindingView();
        ViewBinding.Commit(view, viewModel);

        view.CommandButton.PerformClick();

        Assert.AreEqual(1, viewModel.CommandExecutionCount);
    }

    [TestMethod]
    public void Commit_ConnectsDoubleClickAndKeyBindings()
    {
        var viewModel = new BindingViewModel();
        var view = new BindingView();
        ViewBinding.Commit(view, viewModel);

        view.DoubleClickButton.RaiseDoubleClick();
        view.KeyTextBox.RaiseKeyPress('K');

        Assert.AreEqual(1, viewModel.DoubleClickExecutionCount);
        Assert.AreEqual(1, viewModel.KeyExecutionCount);
    }

    [TestMethod]
    public void Commit_ReflectsBackColorChangesInControl()
    {
        var viewModel = new BindingViewModel();
        var view = new BindingView();
        ViewBinding.Commit(view, viewModel);

        Assert.AreEqual(Color.DarkRed, view.BackColorLabel.BackColor);

        viewModel.BackgroundColor = Color.DarkBlue;

        Assert.AreEqual(Color.DarkBlue, view.BackColorLabel.BackColor);
    }

    [TestMethod]
    public void Commit_RejectsNullArguments()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ViewBinding.Commit(null!, new object()));
        Assert.ThrowsExactly<ArgumentNullException>(() => ViewBinding.Commit(new object(), null!));
    }

    private sealed class BindingView
    {
        [TextBinding(nameof(BindingViewModel.Status))]
        private readonly TextBox _statusLabel = new();

        [CheckedBinding(nameof(BindingViewModel.IsSelected))]
        private readonly CheckBox _selectedCheckBox = new();

        [ListBinding(nameof(BindingViewModel.Values), nameof(BindingViewModel.SelectedValue))]
        private readonly ListBox _valuesListBox = new();

        [EnabledBinding(nameof(BindingViewModel.IsEnabled))]
        private readonly Button _enabledButton = new();

        [VisibilityBinding(nameof(BindingViewModel.IsVisible))]
        private readonly Label _visibleLabel = new();

        [CommandBinding(nameof(BindingViewModel.ExecuteCommand))]
        private readonly Button _commandButton = new();

        [DblClickBinding(nameof(BindingViewModel.ExecuteDoubleClickCommand))]
        private readonly TestButton _doubleClickButton = new();

        [KeyBinding('K', nameof(BindingViewModel.ExecuteKeyCommand))]
        private readonly TestTextBox _keyTextBox = new();

        [BackColorBinding(nameof(BindingViewModel.BackgroundColor))]
        private readonly Label _backColorLabel = new();

        public TextBox StatusLabel => _statusLabel;

        public CheckBox SelectedCheckBox => _selectedCheckBox;

        public ListBox ValuesListBox => _valuesListBox;

        public Button EnabledButton => _enabledButton;

        public Label VisibleLabel => _visibleLabel;

        public Button CommandButton => _commandButton;

        public TestButton DoubleClickButton => _doubleClickButton;

        public TestTextBox KeyTextBox => _keyTextBox;

        public Label BackColorLabel => _backColorLabel;
    }

    private sealed class BindingViewModel : INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        private bool _isSelected = true;
        private bool _isVisible = true;
        private Color _backgroundColor = Color.DarkRed;
        private string? _selectedValue;
        private string _status = "Initial status";

        public BindingViewModel()
        {
            Values = new ObservableCollection<string> { "A", "B" };
            ExecuteCommand = new DelegateCommand(() => CommandExecutionCount++);
            ExecuteDoubleClickCommand = new DelegateCommand(() => DoubleClickExecutionCount++);
            ExecuteKeyCommand = new DelegateCommand(() => KeyExecutionCount++);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public int CommandExecutionCount { get; private set; }

        public int DoubleClickExecutionCount { get; private set; }

        public int KeyExecutionCount { get; private set; }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value, nameof(BackgroundColor));
        }

        public ICommand ExecuteCommand { get; }

        public ICommand ExecuteDoubleClickCommand { get; }

        public ICommand ExecuteKeyCommand { get; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value, nameof(IsEnabled));
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value, nameof(IsSelected));
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value, nameof(IsVisible));
        }

        public string? SelectedValue
        {
            get => _selectedValue;
            set => SetProperty(ref _selectedValue, value, nameof(SelectedValue));
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }

        public ObservableCollection<string> Values { get; }

        private void SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private sealed class DelegateCommand : ICommand
    {
        private readonly Action _execute;

        public DelegateCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }

    public sealed class TestButton : Button
    {
        public void RaiseDoubleClick() => OnDoubleClick(EventArgs.Empty);
    }

    public sealed class TestTextBox : TextBox
    {
        public void RaiseKeyPress(char key) => OnKeyPress(new KeyPressEventArgs(key));
    }
}
