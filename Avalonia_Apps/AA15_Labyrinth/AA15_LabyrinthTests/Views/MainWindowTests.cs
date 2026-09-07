using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AA15_Labyrinth.Views;
using AA15_Labyrinth.ViewModels;
using NSubstitute;

namespace AA15_Labyrinth.Views.Tests;

[TestClass]
public class MainWindowTests
{
    [AvaloniaTestMethod]
    public void NewButton_Click_Should_Randomize_And_Invalidate()
    {
        // Arrange
        ILabyrinthViewModel viewModel = Substitute.For<ILabyrinthViewModel>();
        var window = new MainWindow(viewModel)
        {
            Height = 600,
            Width = 800
        };

        window.Show();

        Button? button = window.GetVisualDescendants().OfType<Button>().FirstOrDefault(b => (b.Content as string) == "Neu");
        Assert.IsNotNull(button);

        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        viewModel.Received(1).Randomize();
    }

    [AvaloniaTestMethod]
    public void OnNewClick_NoViewModel_Should_Not_Throw()
    {
        var window = new MainWindow()
        {
            Height = 400,
            Width = 600
        };
        window.Show();

        Button? button = window.GetVisualDescendants().OfType<Button>().FirstOrDefault(b => (b.Content as string) == "Neu");
        Assert.IsNotNull(button);
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        Assert.IsNull(window.DataContext);
    }
}
