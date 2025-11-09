using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using IntegrationTestApp.Models;
using IntegrationTestApp.ViewModels;

namespace IntegrationTestApp
{
    public partial class MainWindow : Window
    {
        private bool _menuInitialized;

        public MainWindow()
        {
            // Set name in code behind, so source generator will ignore it.
            Name = "MainWindow";

            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
            AppOverlayPopups.Text = Program.OverlayPopups ? "Overlay Popups" : "Native Popups";
            PositionChanged += OnPositionChanged;
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            if (_menuInitialized)
                return;
            if (DataContext is MainWindowViewModel vm)
            {
                InitializeViewMenu(vm.Pages);
                _menuInitialized = true;
            }
        }

        private MainWindowViewModel? ViewModel => (MainWindowViewModel?)DataContext;

        private void InitializeViewMenu(IEnumerable<Page> pages)
        {
            var viewMenu = (NativeMenuItem?)NativeMenu.GetMenu(this)?.Items[1];

            foreach (var page in pages)
            {
                var menuItem = new NativeMenuItem
                {
                    Header = (string?)page.Name,
                    ToolTip = $"Tip:{(string?)page.Name}",
                    ToggleType = NativeMenuItemToggleType.Radio,
                };

                menuItem.Click += (_, _) =>
                {
                    if (ViewModel is { } viewModel)
                        viewModel.SelectedPage = page;
                };

                viewMenu?.Menu?.Items.Add(menuItem);
            }
        }

        private void Pager_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (Pager.SelectedItem is Page page)
                PagerContent.Child = page.CreateContent();
        }

        private void OnPositionChanged(object? sender, PixelPointEventArgs e)
        {
            // HACK: Toggling the window decorations can cause the window to be moved off screen, 
            // causing test failures. Until this bug is fixed, detect this and move the window
            // to the screen origin. See #11411.
            if (Screens.ScreenFromWindow(this) is { } screen)
            {
                var bounds = new PixelRect(
                    e.Point,
                    PixelSize.FromSize(ClientSize, DesktopScaling));

                if (!screen.WorkingArea.Contains(bounds))
                    Position = screen.WorkingArea.Position;
            }
        }
    }
}
