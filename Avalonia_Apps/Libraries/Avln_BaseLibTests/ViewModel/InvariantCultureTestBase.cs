using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace Avalonia.ViewModels;

public abstract class InvariantCultureTestBase : BaseTestViewModel
{
    private static readonly SemaphoreSlim _cultureSemaphore = new(1, 1);

    private CultureInfo _currentUICulture = null!;
    private CultureInfo _currentCulture = null!;
    private CultureInfo? _defaultThreadCurrentUICulture;
    private CultureInfo? _defaultThreadCurrentCulture;

    [TestInitialize]
    public virtual void InitInvariantCulture()
    {
        _cultureSemaphore.Wait();

        _currentUICulture = CultureInfo.CurrentUICulture;
        _currentCulture = CultureInfo.CurrentCulture;
        _defaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;
        _defaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentCulture;

        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        OnInvariantCultureInitialized();
    }

    [TestCleanup]
    public virtual void CleanupInvariantCulture()
    {
        try
        {
            OnInvariantCultureCleanup();
            CultureInfo.CurrentUICulture = _currentUICulture;
            CultureInfo.CurrentCulture = _currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = _defaultThreadCurrentUICulture;
            CultureInfo.DefaultThreadCurrentCulture = _defaultThreadCurrentCulture;
        }
        finally
        {
            _cultureSemaphore.Release();
        }
    }

    protected virtual void OnInvariantCultureInitialized() { }

    protected virtual void OnInvariantCultureCleanup() { }
}

public abstract class InvariantCultureTestBase<T> : BaseTestViewModel<T> where T : class, INotifyPropertyChanged, new()
{
    private static readonly SemaphoreSlim _cultureSemaphore = new(1, 1);

    private CultureInfo _currentUICulture = null!;
    private CultureInfo _currentCulture = null!;
    private CultureInfo? _defaultThreadCurrentUICulture;
    private CultureInfo? _defaultThreadCurrentCulture;

    [TestInitialize]
    public override void Init()
    {
        _cultureSemaphore.Wait();
        try
        {
            _currentUICulture = CultureInfo.CurrentUICulture;
            _currentCulture = CultureInfo.CurrentCulture;
            _defaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;
            _defaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentCulture;

            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            OnInvariantCultureInitialized();
            base.Init();
        }
        catch
        {
            _cultureSemaphore.Release();
            throw;
        }
    }

    [TestCleanup]
    public virtual void CleanupInvariantCulture()
    {
        try
        {
            OnInvariantCultureCleanup();
            CultureInfo.CurrentUICulture = _currentUICulture;
            CultureInfo.CurrentCulture = _currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = _defaultThreadCurrentUICulture;
            CultureInfo.DefaultThreadCurrentCulture = _defaultThreadCurrentCulture;
        }
        finally
        {
            _cultureSemaphore.Release();
        }
    }

    protected virtual void OnInvariantCultureInitialized() { }

    protected virtual void OnInvariantCultureCleanup() { }
}
