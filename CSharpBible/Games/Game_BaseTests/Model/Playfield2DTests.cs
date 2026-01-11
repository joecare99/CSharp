using BaseLib.Helper;
using BaseLib.Interfaces;
using Game.Model;
using Game.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Game.Model.Tests;

public class TestItem2D : IPlacedObject, IEquatable<TestItem2D>, IParentedObject
{
    internal object? _parent;
    public Point _place;
    private Point _oldPlace;

    public string Name = "";

    public static event Action<string, TestItem2D, object?, object?, string>? Log;

    public event EventHandler<(Point oP, Point nP)>? OnPlaceChange;

    public bool Equals(TestItem2D? other) => other != null && other.Name == Name;

#if NET6_0_OR_GREATER
    public object? Parent
    {
        get => _parent;
        set => SetParent(value);
    }

    public Point Place
    {
        get => _place;
        set => SetPlace(value);
    }
#endif

    public object? GetParent() => _parent;

    public void SetParent(object? value, [CallerMemberName] string callerMember = "")
    {
        if (_parent == value) return;
        var old = _parent;
        _parent = value;
        Log?.Invoke("New Parent", this, old, value, callerMember);
    }

    public Point GetPlace() => _place;

    public void SetPlace(Point value, [CallerMemberName] string name = "")
        => value.SetProperty(ref _place,
            (s, o, n) =>
            {
                _oldPlace = (Point)o;
                OnPlaceChange?.Invoke(this, ((Point)o, (Point)n));
                Log?.Invoke("Place changed", this, o, n, s);
            },
            name);

    public Point GetOldPlace() => _oldPlace;

    public override string ToString() => $"TestItem2D({Name},{_place})";

    public bool AddItem(object value)
    {
        throw new NotImplementedException();
    }

    public bool RemoveItem(object value)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<object> GetItems()
    {
        throw new NotImplementedException();
    }

    public void NotifyChildChange(object childObject, object oldVal, object newVal, [CallerMemberName] string prop = "")
    {
        throw new NotImplementedException();
    }
}

[TestClass]
public class Playfield2DTests
{
#pragma warning disable CS8618
    private Playfield2D<TestItem2D> _pf;
#pragma warning restore CS8618

    private string _result = "";

    [TestInitialize]
    public void Init()
    {
        _pf = new Playfield2D<TestItem2D>(new Size(5, 5));
        _pf.OnDataChanged += PfOnDataChanged;
        TestItem2D.Log += ItemLog;
        _result = "";
    }

    [TestCleanup]
    public void Cleanup()
    {
        _pf.OnDataChanged -= PfOnDataChanged;
        TestItem2D.Log -= ItemLog;
    }

    [TestMethod]
    public void Ctor_ShouldSetSizeAndRect()
    {
        Assert.AreEqual(new Size(5, 5), _pf.PfSize);
        Assert.AreEqual(new Rectangle(Point.Empty, new Size(5, 5)), _pf.Rect);
    }

    [TestMethod]
    public void IsInside_Boundaries()
    {
        Assert.IsTrue(_pf.IsInside(new Point(0, 0)));
        Assert.IsTrue(_pf.IsInside(new Point(4, 4)));
        Assert.IsFalse(_pf.IsInside(new Point(-1, 0)));
        Assert.IsFalse(_pf.IsInside(new Point(0, -1)));
        Assert.IsFalse(_pf.IsInside(new Point(5, 0)));
        Assert.IsFalse(_pf.IsInside(new Point(0, 5)));
    }

    [TestMethod]
    public void Indexer_SetInside_ShouldPlaceSetParentAndRaiseItemsChanged()
    {
        var item = new TestItem2D { Name = "A" };

        _pf[new Point(2, 3)] = item;

#if NET6_0_OR_GREATER
        Assert.AreEqual(new Point(2, 3), item.Place);
        Assert.AreEqual(_pf, item.Parent);
#else
        Assert.AreEqual(new Point(2, 3), item.GetPlace());
        Assert.AreEqual(_pf, item.GetParent());
#endif
        Assert.AreSame(item, _pf[new Point(2, 3)]);
        Assert.AreEqual(1, new List<TestItem2D>(_pf.Items).Count);
        StringAssert.Contains(_result, "DataChange: Items\to:\tn:TestItem2D(A,{X=2,Y=3})");
    }

    [TestMethod]
    public void Indexer_SetOutside_ShouldIgnore()
    {
        var item = new TestItem2D { Name = "A" };
        _pf[new Point(7, 7)] = item;

        Assert.IsNull(_pf[new Point(7, 7)]);
        Assert.AreEqual(0, new List<TestItem2D>(_pf.Items).Count);
        Assert.AreEqual("", _result);
    }

    [TestMethod]
    public void Indexer_SetNull_ShouldRemoveUnsubscribeAndRaiseItemsChanged()
    {
        var item = new TestItem2D { Name = "A" };
        _pf[new Point(1, 1)] = item;
        _result = "";

        _pf[new Point(1, 1)] = null;

        Assert.IsNull(_pf[new Point(1, 1)]);
        Assert.AreEqual(0, new List<TestItem2D>(_pf.Items).Count);
#if NET6_0_OR_GREATER
        Assert.IsNull(item.Parent);
#else
        Assert.IsNull(item.GetParent());
#endif
        StringAssert.Contains(_result, "DataChange: Items\to:TestItem2D(A,{X=1,Y=1})\tn:");

        // ensure handler was unsubscribed: changing place should not move anything in playfield
#if NET6_0_OR_GREATER
        item.Place = new Point(2, 2);
#else
        item.SetPlace(new Point(2, 2));
#endif
        Assert.IsNull(_pf[new Point(2, 2)]);
    }

    [TestMethod]
    public void AddItem_WhenTargetCellEmpty_ShouldAddAndReturnTrue()
    {
        var item = new TestItem2D { Name = "A" };
#if NET6_0_OR_GREATER
        item.Place = new Point(2, 2);
#else
        item.SetPlace(new Point(2, 2));
#endif

        Assert.IsTrue(_pf.AddItem(item));
        Assert.AreSame(item, _pf[new Point(2, 2)]);
    }

    [TestMethod]
    public void AddItem_WhenCellAlreadyContainsSameRef_ShouldReturnFalse()
    {
        var item = new TestItem2D { Name = "A" };
#if NET6_0_OR_GREATER
        item.Place = new Point(2, 2);
#else
        item.SetPlace(new Point(2, 2));
#endif

        Assert.IsTrue(_pf.AddItem(item));
        Assert.IsFalse(_pf.AddItem(item));
    }

    [TestMethod]
    public void RemoveItem_WhenPlaced_ShouldRemoveAndReturnTrue()
    {
        var item = new TestItem2D { Name = "A" };
#if NET6_0_OR_GREATER
        item.Place = new Point(2, 2);
#else
        item.SetPlace(new Point(2, 2));
#endif
        _pf.AddItem(item);
        _result = "";

        Assert.IsTrue(_pf.RemoveItem(item));
        Assert.IsNull(_pf[new Point(2, 2)]);
        StringAssert.Contains(_result, "DataChange: Items\to:TestItem2D(A,{X=2,Y=2})\tn:");
    }

    [TestMethod]
    public void ChildPlaceChanged_WhenItemMoves_ShouldMoveDictionaryEntryAndRaisePlaceChange()
    {
        var item = new TestItem2D { Name = "A" };
        _pf[new Point(1, 1)] = item;
        _result = "";

#if NET6_0_OR_GREATER
        item.Place = new Point(3, 4);
#else
        item.SetPlace(new Point(3, 4));
#endif

        Assert.IsNull(_pf[new Point(1, 1)]);
        Assert.AreSame(item, _pf[new Point(3, 4)]);
        StringAssert.Contains(_result, "DataChange: Place\to:{X=1,Y=1}\tn:{X=3,Y=4}");
    }

    [TestMethod]
    public void PfSize_Set_ShouldRaiseEventAndUpdateRect()
    {
        _result = "";
        _pf.PfSize = new Size(7, 8);

        Assert.AreEqual(new Size(7, 8), _pf.PfSize);
        Assert.AreEqual(new Rectangle(Point.Empty, new Size(7, 8)), _pf.Rect);
        StringAssert.Contains(_result, "DataChange: PfSize\to:{Width=5, Height=5}\tn:{Width=7, Height=8}");
    }

    private void PfOnDataChanged(object? sender, (string prop, object? oldVal, object? newVal) e)
    {
        _result += $"DataChange: {e.prop}\to:{e.oldVal}\tn:{e.newVal}\r\n";
    }

    private void ItemLog(string op, TestItem2D item, object? oldVal, object? newVal, string prop)
    {
        _result += $"{op}: {item}\to:{oldVal}\tn:{newVal}\tc:{prop}\r\n";
    }
}
