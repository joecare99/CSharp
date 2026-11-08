using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AxamlDesigner.Host.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace AA98_AxamlDesigner.Host.Commands;

public static class AxamlDesignerMcpExports
{
    public static IServiceCollection AddAxamlDesignerMcpExports(this IServiceCollection services)
    {
        return AddAxamlDesignerMcpExports(services, OnUiThreadAsync);
    }

    internal static IServiceCollection AddAxamlDesignerMcpExports(
        this IServiceCollection services,
        Func<Func<string>, CancellationToken, Task<string>> invokeOnUiThread)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(invokeOnUiThread);
        services.AddSingleton<IComponentScreenshotSource, AxamlDesignerScreenshotSource>();
        services.AddSingleton<IComponentMcpCommand>(provider =>
        {
            AxamlDesignerHostViewModel viewModel = provider.GetRequiredService<AxamlDesignerHostViewModel>();
            return Create("designer.get_state", "Get designer state",
                "Returns the open file, current selection, preview tree, and editable properties.",
                """{"type":"object","properties":{},"additionalProperties":false}""",
                isReadOnly: true,
                (_, token) => invokeOnUiThread(viewModel.GetDesignerStateJson, token));
        });
        services.AddSingleton<IComponentMcpCommand>(provider =>
        {
            AxamlDesignerHostViewModel viewModel = provider.GetRequiredService<AxamlDesignerHostViewModel>();
            return Create("designer.open", "Open AXAML file",
                "Opens a local .axaml document in the designer preview.",
                """{"type":"object","properties":{"path":{"type":"string","description":"Path to a local .axaml file."}},"required":["path"],"additionalProperties":false}""",
                false,
                (args, token) =>
                {
                    string path = ReadString(args, "path");
                    if (!string.Equals(Path.GetExtension(path), ".axaml", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException("Only .axaml files can be opened through the designer command.");
                    }

                    return invokeOnUiThread(() =>
                    {
                        if (!viewModel.OpenDocument(path))
                        {
                            throw new IOException(viewModel.StatusText);
                        }

                        return viewModel.StatusText;
                    }, token);
                });
        });
        services.AddSingleton<IComponentMcpCommand>(provider =>
        {
            AxamlDesignerHostViewModel viewModel = provider.GetRequiredService<AxamlDesignerHostViewModel>();
            return Create("designer.save", "Save AXAML file",
                "Saves the currently open AXAML file to its existing path.",
                """{"type":"object","properties":{},"additionalProperties":false}""",
                false,
                (_, token) => invokeOnUiThread(() => viewModel.SaveDocument()
                    ? viewModel.StatusText
                    : throw new IOException(viewModel.StatusText), token));
        });
        services.AddSingleton<IComponentMcpCommand>(provider =>
        {
            AxamlDesignerHostViewModel viewModel = provider.GetRequiredService<AxamlDesignerHostViewModel>();
            return Create("designer.select_element", "Select designer element",
                "Selects an element from the preview tree using its child-index path from the root (for example 0/1).",
                """{"type":"object","properties":{"nodePath":{"type":"string","description":"Slash-separated child indexes; empty string selects the root."}},"required":["nodePath"],"additionalProperties":false}""",
                true,
                (args, token) =>
                {
                    string path = ReadString(args, "nodePath", allowEmpty: true);
                    return invokeOnUiThread(() =>
                    {
                        if (!viewModel.SelectElement(path))
                        {
                            throw new ArgumentException($"No preview element exists at path '{path}'.");
                        }

                        return viewModel.GetDesignerStateJson();
                    }, token);
                });
        });
        services.AddSingleton<IComponentMcpCommand>(provider =>
        {
            AxamlDesignerHostViewModel viewModel = provider.GetRequiredService<AxamlDesignerHostViewModel>();
            return Create("designer.set_property", "Set designer property",
                "Updates one supported property on a selected preview element and writes the change to the open AXAML document.",
                """{"type":"object","properties":{"nodePath":{"type":"string"},"propertyName":{"type":"string"},"value":{"type":"string"}},"required":["nodePath","propertyName","value"],"additionalProperties":false}""",
                false,
                (args, token) =>
                {
                    string nodePath = ReadString(args, "nodePath", allowEmpty: true);
                    string propertyName = ReadString(args, "propertyName");
                    string value = ReadString(args, "value", allowEmpty: true);
                    return invokeOnUiThread(() =>
                    {
                        bool changed = viewModel.SetElementProperty(nodePath, propertyName, value);
                        if (!changed)
                        {
                            throw new ArgumentException("The selected AXAML preview path does not exist.");
                        }

                        return viewModel.GetDesignerStateJson();
                    }, token);
                });
        });
        services.AddSingleton<IComponentMcpCommand>(provider =>
        {
            AxamlDesignerHostViewModel viewModel = provider.GetRequiredService<AxamlDesignerHostViewModel>();
            return Create("designer.undo", "Undo designer edit",
                "Undoes the most recent designer property edit.",
                """{"type":"object","properties":{},"additionalProperties":false}""",
                false,
                (_, token) => invokeOnUiThread(() =>
                {
                    if (!viewModel.Designer.UndoCommand.CanExecute(null))
                    {
                        throw new InvalidOperationException("There is no designer edit to undo.");
                    }

                    viewModel.Designer.UndoCommand.Execute(null);
                    return viewModel.GetDesignerStateJson();
                }, token));
        });
        services.AddSingleton<IComponentMcpCommand>(provider =>
        {
            AxamlDesignerHostViewModel viewModel = provider.GetRequiredService<AxamlDesignerHostViewModel>();
            return Create("designer.redo", "Redo designer edit",
                "Reapplies the most recently undone designer property edit.",
                """{"type":"object","properties":{},"additionalProperties":false}""",
                false,
                (_, token) => invokeOnUiThread(() =>
                {
                    if (!viewModel.Designer.RedoCommand.CanExecute(null))
                    {
                        throw new InvalidOperationException("There is no designer edit to redo.");
                    }

                    viewModel.Designer.RedoCommand.Execute(null);
                    return viewModel.GetDesignerStateJson();
                }, token));
        });
        return services;
    }

    private static DelegateComponentMcpCommand Create(
        string id,
        string title,
        string description,
        string schema,
        bool isReadOnly,
        Func<JsonElement, CancellationToken, Task<string>> execute)
    {
        return new DelegateComponentMcpCommand(
            new ComponentMcpCommandDescriptor(id, title, description, schema, isReadOnly),
            execute);
    }

    private static string ReadString(JsonElement arguments, string name, bool allowEmpty = false)
    {
        if (!arguments.TryGetProperty(name, out JsonElement value)
            || value.ValueKind != JsonValueKind.String
            || (!allowEmpty && string.IsNullOrWhiteSpace(value.GetString())))
        {
            throw new ArgumentException($"A non-empty string argument '{name}' is required.");
        }

        return value.GetString()!;
    }

    private static async Task<string> OnUiThreadAsync(Func<string> action, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (Dispatcher.UIThread.CheckAccess())
        {
            return action();
        }

        return await Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Normal, cancellationToken);
    }
}

public sealed class AxamlDesignerScreenshotSource(AxamlDesignerHostViewModel viewModel) : IComponentScreenshotSource
{
    public string ComponentId => "axaml-designer";

    public Task<byte[]> CapturePngAsync(CancellationToken cancellationToken)
    {
        return viewModel.CaptureScreenshotPngAsync(cancellationToken);
    }
}
