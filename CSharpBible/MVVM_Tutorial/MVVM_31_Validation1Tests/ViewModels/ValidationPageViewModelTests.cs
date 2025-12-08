using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MVVM_31_Validation1.ViewModels.Tests;

[TestClass()]
public class ValidationPageViewModelTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    ValidationPageViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testModel = new();
    }

    [TestMethod()]
    [DataRow("",1,false)]
    [DataRow("DS", 2, false)]
    [DataRow("DS1234", 0, true)]
    public void TestUsernameTest(string sVal, int iErg, bool xExp)
    {
        bool f(string s) => testModel.TestUsername(s);
        switch (iErg)
        {
            case 1: Assert.ThrowsExactly<ArgumentNullException>(() => f(sVal)); break;
            case 2: Assert.ThrowsExactly<ArgumentException>(() => f(sVal)); break;
            default: Assert.AreEqual(xExp, f(sVal)); break;
        }
    }
}