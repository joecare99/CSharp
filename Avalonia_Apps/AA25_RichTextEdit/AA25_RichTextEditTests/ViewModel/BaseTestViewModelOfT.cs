using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Avalonia.ViewModels;

public abstract class BaseTestViewModel<T> : BaseTestViewModel where T : class, INotifyPropertyChanged, new()
{
    protected System.Func<T> GetModel = static () => new T();

#pragma warning disable CS8618
    protected T testModel;
    protected T testModel2;
#pragma warning restore CS8618

    protected virtual Dictionary<string, object?> GetDefaultData() => new();

    [TestInitialize]
    public virtual void Init()
    {
        testModel = GetModel.Invoke();
        testModel2 = GetModel.Invoke();
        if (testModel is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged += OnVMPropertyChanged;
        }

        if (testModel is INotifyPropertyChanging inpcg)
        {
            inpcg.PropertyChanging += OnVMPropertyChanging;
        }

        if (testModel is INotifyDataErrorInfo indei)
        {
            indei.ErrorsChanged += OnVMErrorsChanged;
        }

        foreach (var propertyInfo in typeof(T).GetProperties())
        {
            if (propertyInfo.CanRead && propertyInfo.GetValue(testModel) is CommunityToolkit.Mvvm.Input.IRelayCommand relayCommand)
            {
                relayCommand.CanExecuteChanged += OnCanExChanged;
            }
        }
    }

    protected static IEnumerable<object[]> TestModelProperies => typeof(T).GetProperties().Select(o => new object[] { o.Name, o.PropertyType.TC(), o.CanRead, o.CanWrite });

    [TestMethod]
    [DynamicData(nameof(TestModelProperies))]
    public virtual void TestModelProperiesTest(string propertyName, System.TypeCode propertyType, bool canRead, bool canWrite)
    {
        var propertyInfo = typeof(T).GetProperty(propertyName);
        Assert.IsNotNull(propertyInfo);
        Assert.AreEqual(canRead, propertyInfo!.CanRead);
        Assert.AreEqual(canWrite, propertyInfo.CanWrite);
        if (canRead && GetDefaultData().TryGetValue(propertyName, out var defaultValue))
        {
            if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
            {
                var command = testModel.GetProp(propertyName) as ICommand;
                Assert.IsNotNull(command);
                Assert.AreEqual(defaultValue, command.CanExecute(null));
            }
            else
            {
                Assert.AreEqual(defaultValue, testModel.GetProp(propertyName));
            }
        }

        _ = propertyType;
    }
}