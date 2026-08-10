using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Db.Core.Abstractions.Sql.Interfaaces;
using Db.Provider.MySql;
using Microsoft.Extensions.DependencyInjection;
using RnzTrauer.Avalonia.ViewModels;
using RnzTrauer.Avalonia.Views;
using RnzTrauer.Core.Export;
using RnzTrauer.Persistence.MySql;
using RnzTrauer.Places;
using RnzTrauer.Core.Services;
using RnzTrauer.Import.Services;

namespace RnzTrauer.Avalonia;

/// <summary>Composition root; views stay DI-created while Core has no Avalonia dependency.</summary>
public sealed partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        var factory = new MySqlDbConnectionFactory();
        var settings = factory.CreateSettingsStub();
        settings["Server"] = Environment.GetEnvironmentVariable("RNZ_DB_SERVER") ?? "localhost";
        settings["Port"] = uint.TryParse(Environment.GetEnvironmentVariable("RNZ_DB_PORT"), out var port) ? port : 3306u;
        settings["UserID"] = Environment.GetEnvironmentVariable("RNZ_DB_USER") ?? "root";
        settings["Password"] = Environment.GetEnvironmentVariable("RNZ_DB_PASSWORD") ?? string.Empty;
        settings["Database"] = Environment.GetEnvironmentVariable("RNZ_DB_NAME") ?? "RNZ";
        services.AddSingleton<IDbConnectionFactory>(factory)
            .AddSingleton<IDBSettings>(settings)
            .AddSingleton<INoticeRepository, MySqlNoticeRepository>()
            .AddSingleton<MySqlPlaceCoordinateStore>()
            .AddSingleton<IPlaceCoordinateStore>(services => services.GetRequiredService<MySqlPlaceCoordinateStore>())
            .AddSingleton<ICoordinateSchemaProbe>(services => services.GetRequiredService<MySqlPlaceCoordinateStore>())
            .AddSingleton<INoticeTextParser, NoticeTextParser>()
            .AddSingleton<IHtmlTextNormalizer, HtmlTextNormalizer>()
            .AddSingleton<IHtmlEncodingDecoder, HtmlEncodingDecoder>()
            .AddTransient<IHtmlCallbackTokenizer, HtmlCallbackTokenizer>()
            .AddTransient<ISchemaFilter, SchemaFilter>()
            .AddTransient<ISchemaImportAccumulator, SchemaImportAccumulator>()
            .AddTransient<IHtmlSchemaImporter, HtmlSchemaImporter>()
            .AddSingleton<IExportService, NoticeExportService>()
            .AddSingleton<MainWindowViewModel>();
        var provider = services.BuildServiceProvider();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow { DataContext = provider.GetRequiredService<MainWindowViewModel>() };
        base.OnFrameworkInitializationCompleted();
    }
}
