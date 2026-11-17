using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Decompilation;
using AA98_AvlnCodeStudio.UI.Resources;
using Code.Navigation;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>Owns one explicit assembly inspection and its selected-type operation.</summary>
public sealed class DecompilationWorkbench : IDisposable
{
    private readonly IAssemblyDecompilationService _service;
    private CancellationTokenSource? _operation;
    private long _revision;
    private bool _disposed;

    public DecompilationWorkbench(IAssemblyDecompilationService service) =>
        _service = service ?? throw new ArgumentNullException(nameof(service));

    public event EventHandler? Changed;
    public AssemblyInspection? Assembly { get; private set; }
    public IReadOnlyList<AssemblyTypeInfo> Types => Assembly?.Types ?? Array.Empty<AssemblyTypeInfo>();
    public AssemblyTypeInfo? SelectedType { get; private set; }
    public string Status { get; private set; } = UiStrings.DecompileChoosePrompt;
    public bool IsBusy { get; private set; }

    public void Reset()
    {
        if (_disposed) return;
        Invalidate();
        Assembly = null;
        SelectedType = null;
        IsBusy = false;
        Status = UiStrings.DecompileChoosePrompt;
        Notify();
    }

    public void SelectType(AssemblyTypeInfo? type)
    {
        if (_disposed || ReferenceEquals(type, SelectedType)) return;
        Invalidate();
        IsBusy = false;
        SelectedType = type is not null && Types.ContainsReference(type) ? type : null;
        Status = SelectedType is null ? UiStrings.DecompileSelectTypeText : UiStrings.DecompileSelectedTypeText;
        Notify();
    }

    public async Task InspectAsync(string path)
    {
        if (_disposed) return;
        Reset();
        var (revision, operation) = Start(UiStrings.DecompileInspectingText);
        try
        {
            AssemblyInspection assembly = await _service.InspectAsync(path, operation.Token);
            if (!IsCurrent(revision, operation)) return;
            Assembly = assembly;
            Status = assembly.Types.Count == 0 ? UiStrings.DecompileNoTypesText : UiStrings.DecompileSelectTypeText;
        }
        catch (OperationCanceledException) when (operation.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            if (IsCurrent(revision, operation)) Status = string.Format(UiStrings.DecompileInspectErrorFormat, exception.Message);
        }
        finally
        {
            Finish(revision, operation);
        }
    }

    public async Task<CodeLocation?> DecompileSelectedAsync()
    {
        if (_disposed || IsBusy || Assembly is null || SelectedType is null) return null;
        string path = Assembly.AssemblyPath;
        string typeName = SelectedType.FullName;
        var (revision, operation) = Start(UiStrings.DecompileInProgressText);
        try
        {
            DecompiledTypeArtifact artifact = await _service.DecompileTypeAsync(path, typeName, operation.Token);
            if (!IsCurrent(revision, operation)) return null;
            Status = UiStrings.DecompileSourceReadyText;
            return artifact.NavigationLocation;
        }
        catch (OperationCanceledException) when (operation.IsCancellationRequested)
        {
            return null;
        }
        catch (Exception exception)
        {
            if (IsCurrent(revision, operation)) Status = string.Format(UiStrings.DecompileErrorFormat, exception.Message);
            return null;
        }
        finally
        {
            Finish(revision, operation);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Invalidate();
        Changed = null;
    }

    private (long, CancellationTokenSource) Start(string status)
    {
        Invalidate();
        var operation = new CancellationTokenSource();
        _operation = operation;
        IsBusy = true;
        Status = status;
        Notify();
        return (_revision, operation);
    }

    private void Invalidate()
    {
        _revision++;
        _operation?.Cancel();
        _operation = null;
    }

    private bool IsCurrent(long revision, CancellationTokenSource operation) =>
        !_disposed && revision == _revision && ReferenceEquals(operation, _operation) && !operation.IsCancellationRequested;

    private void Finish(long revision, CancellationTokenSource operation)
    {
        if (IsCurrent(revision, operation))
        {
            _operation = null;
            IsBusy = false;
            Notify();
        }

        operation.Dispose();
    }

    private void Notify() => Changed?.Invoke(this, EventArgs.Empty);
}

internal static class DecompilationTypeListExtensions
{
    public static bool ContainsReference(this IReadOnlyList<AssemblyTypeInfo> types, AssemblyTypeInfo value)
    {
        foreach (AssemblyTypeInfo type in types)
        {
            if (ReferenceEquals(type, value)) return true;
        }

        return false;
    }
}
