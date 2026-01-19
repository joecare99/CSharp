using Avln_CommonDialogs.Avalonia.Dialogs;
using Avln_CommonDialogs.Avalonia.Printing;
using Avln_CommonDialogs.Base.Interfaces;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Avln_CommonDialogs.Avalonia;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Avalonia-based dialog implementations.
    /// 
    /// The <paramref name="topLevelProvider"/> is used for parameterless ShowAsync() calls.
    /// If you always call the owner overloads, you can pass () => null.
    /// </summary>
    public static IServiceCollection AddAvaloniaCommonDialogs(
        this IServiceCollection services,
        Func<TopLevel?> topLevelProvider)
    {
        services.AddTransient<IOpenFileDialog>(_ => new AvaloniaOpenFileDialog(topLevelProvider));
        services.AddTransient<ISaveFileDialog>(_ => new AvaloniaSaveFileDialog(topLevelProvider));
        services.AddTransient<IColorDialog>(_ => new AvaloniaColorDialog(topLevelProvider));
        services.AddTransient<IFontDialog>(_ => new AvaloniaFontDialog(topLevelProvider));

        // Printing is not provided cross-platform by Avalonia out of the box.
        // Register a placeholder so consumers can still resolve IPrintDialog.
        services.AddTransient<IPrintDialog, NotSupportedPrintDialog>();

        return services;
    }
}
