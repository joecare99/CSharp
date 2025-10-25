using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLib.Helper.MVVM.Tests;

[TestClass()]
public class ValidationHelperExtTests : INotifyDataErrorInfo
{
    private bool _xHasErrors = default;
    private string _sPropName = string.Empty;
    private List<ValidationResult> _ErrList = new();
    private string? csExpEmpty = default;

    public bool HasErrors => _xHasErrors;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName == _sPropName)
            return _ErrList;
        else
            return new List<string>();
    }

    [TestMethod()]
    [DataRow("Test", false, new string[] { }, null)]
    [DataRow("Test2", true, new[] { "Error1" }, "Error1")]
    [DataRow("TTest2", true, new[] { "Error1", "Error2" }, "Error1, Error2")]
    [DataRow("TTest3", true, new[] { "Error1", "Error2", "Error3" }, "Error1, Error2, Error3")]
    public void ValidationTextTest(string sProp, bool xHasErrors, string[] Errs, string? sExp)
    {
        _xHasErrors = xHasErrors;
        _sPropName = sProp.Remove(0, 1);
        _ErrList.Clear();
        foreach (var s in Errs)
            _ErrList.Add(new ValidationResult(s));
        Assert.AreEqual(sExp, this.ValidationText(sProp));
        Assert.AreEqual(csExpEmpty, this.ValidationText("Test"));
    }

    [TestMethod]
    public void ValidationTextTestNull()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            ((INotifyDataErrorInfo)null!).ValidationText("123"));
    }

    [TestMethod]
    public void ValidationTextTestNull2()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            this.ValidationText(null));
    }
}