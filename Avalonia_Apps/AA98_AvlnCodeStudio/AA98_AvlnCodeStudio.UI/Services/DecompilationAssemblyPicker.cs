using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace AA98_AvlnCodeStudio.UI.Services;

public interface IDecompilationAssemblyPicker
{
    Task<string?> PickAsync(Window owner);
}

public sealed class DecompilationAssemblyPicker : IDecompilationAssemblyPicker
{
    public async Task<string?> PickAsync(Window owner)
    {
        var files = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose a managed assembly",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Managed assemblies") { Patterns = ["*.dll", "*.exe"] }],
        });
        if (files.FirstOrDefault() is not { } file) return null;
        return file.TryGetLocalPath() ?? throw new InvalidOperationException("Choose a local assembly file.");
    }
}
