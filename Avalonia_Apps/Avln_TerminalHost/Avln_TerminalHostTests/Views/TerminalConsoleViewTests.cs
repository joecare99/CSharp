using System;
using System.Threading.Tasks;
using Avln_TerminalHost.ViewModels;
using Avln_TerminalHost.Views;
using Avalonia.Controls;
using Avalonia.Headless.MSTest;
using Avalonia.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Avln_TerminalHost.Services;

namespace Avln_TerminalHostTests.Views;

[TestClass]
public class TerminalConsoleViewTests
{
    [AvaloniaTestMethod]
    public async Task TerminalConsoleView_ShouldResizeConsoleBufferAfterDebouncedSizeChange()
    {
        var processRunner = Substitute.For<IProcessRunner>();
        var viewModel = new MainWindowViewModel(processRunner);
        var view = new TerminalConsoleView
        {
            Width = 800,
            Height = 400,
            DataContext = viewModel,
        };
        var window = new Window
        {
            Width = 1800,
            Height = 900,
            Content = view,
        };

        window.Show();
        await Task.Delay(150);
        await Dispatcher.UIThread.InvokeAsync(() => { });

        viewModel.Console.WindowWidth.Should().BeGreaterThan(1);
        viewModel.Console.WindowHeight.Should().BeGreaterThan(1);

        view.Width = 1200;
        view.Height = 600;
        await Task.Delay(150);
        await Dispatcher.UIThread.InvokeAsync(() => { });

        var resizedWidth = viewModel.Console.WindowWidth;
        var resizedHeight = viewModel.Console.WindowHeight;

        view.Width = 1600;
        view.Height = 800;
        await Task.Delay(150);
        await Dispatcher.UIThread.InvokeAsync(() => { });

        resizedWidth.Should().BeGreaterThan(1);
        resizedHeight.Should().BeGreaterThan(1);
        viewModel.Console.WindowWidth.Should().BeGreaterThan(resizedWidth);
        viewModel.Console.WindowHeight.Should().BeGreaterThan(resizedHeight);
    }

    [AvaloniaTestMethod]
    public async Task TerminalConsoleView_ShouldDisplayProcessOutputAfterResizeWithoutAdditionalInteraction()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var viewModel = new MainWindowViewModel(processRunner);
        var view = new TerminalConsoleView
        {
            Width = 800,
            Height = 400,
            DataContext = viewModel,
        };
        var window = new Window
        {
            Width = 1800,
            Height = 900,
            Content = view,
        };

        window.Show();
        await viewModel.StartCommand.ExecuteAsync(null);
        view.Width = 1200;
        view.Height = 600;
        await Task.Delay(150);

        hostedProcess.EmitStandardOutput("hello after resize");
        await Dispatcher.UIThread.InvokeAsync(() => { });

        viewModel.Console.Content.Should().Contain("hello after resize");
    }
}
