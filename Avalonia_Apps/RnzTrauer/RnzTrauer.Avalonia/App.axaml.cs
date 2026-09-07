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
using RnzTrauer.Media;

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
            .AddSingleton<INoticeSearchService, NoticeSearchService>()
            .AddSingleton<INoticeDetailService, NoticeDetailService>()
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
            .AddSingleton<IExternalProcessRunner, SystemExternalProcessRunner>()
            .AddSingleton<IPdfImageRenderer>(services =>
                new PopplerPdfImageRenderer(
                    new PopplerPdfImageRendererOptions(
                        Environment.GetEnvironmentVariable("RNZ_PDFTOPPM_PATH") ?? "pdftoppm"),
                    services.GetRequiredService<IExternalProcessRunner>()))
            .AddSingleton<IOcrEngine>(services =>
                new TesseractOcrEngine(
                    new TesseractOcrOptions(
                        Environment.GetEnvironmentVariable("RNZ_TESSERACT_PATH") ?? "tesseract"),
                    services.GetRequiredService<IExternalProcessRunner>()))
            .AddSingleton<IPdfOcrService>(services =>
                new PdfOcrService(
                    services.GetRequiredService<IPdfImageRenderer>(),
                    services.GetRequiredService<IOcrEngine>()))
            .AddSingleton<IExportService, NoticeExportService>()
            .AddSingleton<MainWindowViewModel>(services =>
                new MainWindowViewModel(
                    services.GetRequiredService<INoticeRepository>(),
                    services.GetRequiredService<INoticeSearchService>(),
                    services.GetRequiredService<INoticeTextParser>(),
                    services.GetRequiredService<IExportService>(),
                    services.GetRequiredService<ICoordinateSchemaProbe>(),
                    services.GetRequiredService<IPlaceCoordinateStore>(),
                    services.GetRequiredService<INoticeDetailService>(),
                    services.GetRequiredService<IPdfOcrService>(),
                    readOnly: true));
        var provider = services.BuildServiceProvider();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = provider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow { DataContext = viewModel };
            _ = viewModel.InitializeAsync();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
