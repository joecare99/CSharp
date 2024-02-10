using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MVVM_24c_CTUserControl.Views.Converters.Tests;

[TestClass]
public class BoolToBrushConverterTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
	BoolToBrushConverter testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

	[TestInitialize]
	public void Init()
	{
		testClass = new();
	}

	[TestMethod]
	public void BoolToBrushConverterTest()
	{
		Assert.IsNotNull(testClass);
		Assert.IsInstanceOfType(testClass, typeof(BoolToBrushConverter));
		Assert.IsInstanceOfType(testClass, typeof(IValueConverter));
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	[DataRow(null)]
	public void PropertyTest(bool? xAct)
	{
		var tstBr = new SolidColorBrush(Color.FromArgb(0, 0, 0, 1));
		_ = xAct switch
		{
			true => testClass.TrueBrush = tstBr,
			false => testClass.FalseBrush = tstBr,
			_ => testClass.DefaultBrush = tstBr,
		};
		bool? xAct2 = xAct switch
		{
			true => false,
			false => null,
			_ => true
		};
		Assert.AreEqual(tstBr, testClass.Convert(xAct!, null!, null!, null!));
		Assert.AreNotEqual(tstBr, testClass.Convert(xAct2!, null!, null!, null!));
		if (xAct == null)
		Assert.AreEqual(tstBr, testClass.Convert(2, null!, null!, null!));
		else
		Assert.AreNotEqual(tstBr, testClass.Convert(2, null!, null!, null!));
	}

	[TestMethod]
	public void ConvertTest()
	{
		var tstBr = new SolidColorBrush(Color.FromArgb(0, 0, 0, 2));

		Assert.AreEqual(tstBr, testClass.Convert(true, null!, tstBr, null!));
		Assert.AreNotEqual(tstBr, testClass.Convert(false, null!, tstBr, null!));
		Assert.AreNotEqual(tstBr, testClass.Convert(null!, null!, tstBr, null!));
		Assert.AreNotEqual(tstBr, testClass.Convert(2, null!, tstBr, null!));
	}

	[TestMethod]
	public void ConvertBackTest()
	{
		Assert.ThrowsException<NotImplementedException>(() => testClass.ConvertBack(null!, null!, null!, null!));
	}
}
