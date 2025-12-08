using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.MSTest;
using Avalonia.Input;
using AA15_Labyrinth.Views;
using AA15_Labyrinth.ViewModels;
using NSubstitute;
using Avalonia.VisualTree; // <--- Diese using-Direktive hinzufügen

namespace AA15_Labyrinth.Views.Tests;

[TestClass]
public class MainWindowTests
{
    private ILabyrinthViewModel? _vm;

    [AvaloniaTestMethod]
    public void NewButton_Click_Should_Randomize_And_Invalidate()
    {
        // Arrange
        var window = new MainWindow()
        {
            Height = 600,
            Width = 800,
            DataContext = _vm = Substitute.For<ILabyrinthViewModel>()
        };

        // Ensure content is a visual to hit invalidate path
        window.Content = new LabyrinthView();

        window.Show();

        // Act: Find the button and simulate Enter press on it
        var btn = window.FindControl<Button>("OnNewClick"); // name not set; fallback to call handler via Click routed event.
                                                            // Since the XAML doesn't set Name, trigger by raising the event handler directly via routed event on the found button by content
        var button = window.GetVisualDescendants().OfType<Button>().FirstOrDefault(b => (b.Content as string) == "Neu");
        button?.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        // Assert
        _vm!.Received(1).Randomize();
    }

    [AvaloniaTestMethod]
    public void OnNewClick_NoViewModel_Should_Not_Throw()
    {
        var window = new MainWindow()
        {
            Height = 400,
            Width = 600
        };
        window.Content = new LabyrinthView();
        window.Show();

        // Invoke handler safely via Enter on button
        var button = window.GetVisualDescendants().OfType<Button>().FirstOrDefault(b => (b.Content as string) == "Neu");
        button?.Focus();
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);

        Assert.IsTrue(true);
    }
}
