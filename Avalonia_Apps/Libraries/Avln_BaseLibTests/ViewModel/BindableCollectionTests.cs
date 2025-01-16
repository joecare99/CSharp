using Avalonia.ViewModels.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Avalonia.ViewModels.Tests;

[TestClass()]
public class BindableCollectionTests : BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private BindableCollection<string> _testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        _testClass = new BindableCollection<string>();
        _testClass.CollectionChanged += OnCollectionChanged;
        ClearLog();

    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
       DoLog($"OnCollectionChanged({sender.GetType().Name},{e.Action})");
    }

    [TestMethod()]
    public void BindableCollectionTest()
    {
        Assert.IsNotNull(_testClass);
        Assert.IsInstanceOfType(_testClass, typeof(BindableCollection<string>));
        Assert.IsInstanceOfType(_testClass, typeof(ObservableCollection<string>));
        Assert.IsInstanceOfType(_testClass, typeof(IObservableCollection<string>));
        Assert.AreSame(_testClass, _testClass as IObservableCollection<string>);
    }

    [TestMethod()]
    public void BindableCollectionTest1()
    {
        var testClass = new BindableCollection<string>(new[] { "1", "2", "3" });
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(BindableCollection<string>));
        Assert.IsInstanceOfType(testClass, typeof(ObservableCollection<string>));
        Assert.IsInstanceOfType(testClass, typeof(IObservableCollection<string>));
    }

    [TestMethod()]
    public void NotifyOnCollectionChangedTest()
    {
        _testClass.Add("5");
        Assert.AreEqual(1, _testClass.Count);
        Assert.AreEqual("5", _testClass[0]);
        Assert.AreEqual("OnCollectionChanged(BindableCollection`1,Add)\r\n", DebugLog);
    }

    [TestMethod()]
    public void NotifyOfPropertyChangeTest()
    {
        _testClass.Add("5");
        _testClass.NotifyOfPropertyChange("Test");
        Assert.AreEqual(1, _testClass.Count);
        Assert.AreEqual("5", _testClass[0]);
        Assert.AreEqual("OnCollectionChanged(BindableCollection`1,Add)\r\n", DebugLog);
    }

    [TestMethod()]
    public void SetItemTest()
    {
        _testClass.Add("5");
        _testClass[0] = "7";
        Assert.AreEqual(1, _testClass.Count);
        Assert.AreEqual("7", _testClass[0]);
        Assert.AreEqual("OnCollectionChanged(BindableCollection`1,Add)\r\nOnCollectionChanged(BindableCollection`1,Replace)\r\n", DebugLog);
    }

    [TestMethod()]
    public void RemoveItemTest()
    {
        _testClass.Add("5");
        _testClass.RemoveAt(0);
        Assert.AreEqual(0, _testClass.Count);
        Assert.AreEqual("OnCollectionChanged(BindableCollection`1,Add)\r\nOnCollectionChanged(BindableCollection`1,Remove)\r\n", DebugLog);
    }

    [TestMethod()]
    public void AddRangeTest()
    {
        _testClass.AddRange(new[] { "5", "7" });
        Assert.AreEqual(2, _testClass.Count);
        Assert.AreEqual("5", _testClass[0]);
        Assert.AreEqual("7", _testClass[1]);
    }

    [TestMethod()]
    public void ClearTest()
    {
        _testClass.AddRange(new[] { "5", "7" });
        _testClass.Clear();
        Assert.AreEqual(0, _testClass.Count);
    }

    [TestMethod()]
    public void RefreshTest()
    {
        _testClass.AddRange(new[] { "5", "7" });
        _testClass.Refresh();
        Assert.AreEqual(2, _testClass.Count);
    }

    [TestMethod()]
    public void RemoveRangeTest()
    {
        _testClass.AddRange(new[] { "5", "7", "1" });
        _testClass.RemoveRange(new[] { "5", "1", "0" });
        Assert.AreEqual(1, _testClass.Count);
    }
}