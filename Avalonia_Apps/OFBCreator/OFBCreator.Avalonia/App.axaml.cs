using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Document.Base.Factories;
using Document.Docx;
using Genealogy.Drivers;
using Genealogy.Gedcom;
using Microsoft.Extensions.DependencyInjection;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Avalonia.Services;
using OFBCreator.Avalonia.ViewModels;
using OFBCreator.Avalonia.Views;
using OFBCreator.Console.Services;
using OFBCreator.Console.Services.Templates;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;
using OFBCreator.Projects.Services;
using PortableProjectStore = OFBCreator.Projects.Services.OFBProjectStore;

namespace OFBCreator.Avalonia;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<OFBProjectWorkspaceViewModel>();
            desktop.MainWindow = mainWindow;
            desktop.Exit += (_, _) => _serviceProvider?.Dispose();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
        services.AddSingleton<IGenealogyInputDriver, GedcomInputDriver>();
        services.AddSingleton<CanonicalGenealogyAdapter>();
        services.AddSingleton<CanonicalGedcomFamilyDataSource>();
        services.AddSingleton<IFamilyDataSource>(provider =>
            provider.GetRequiredService<CanonicalGedcomFamilyDataSource>());
        services.AddSingleton<IUserDocumentFactory, UserDocumentFactoryImpl>();
        services.AddSingleton<PortableProjectStore>();
        services.AddSingleton<IOFBFileDialogService, AvaloniaFileDialogService>();
        services.AddSingleton<EntryTemplateStore>();
        services.AddSingleton<ConsoleExportService>();
        services.AddSingleton<IOFBWorkspaceService, OFBWorkspaceService>();
        services.AddSingleton<OFBProjectWorkspaceViewModel>();
        services.AddSingleton<MainWindow>();
    }
}
