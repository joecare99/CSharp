using System;
using System.Reflection;
using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using DataAnalysis.Core.Export;

namespace DataAnalysis.Core.Tests;

// Pseudocode (Plan):
// 1. Per Reflection die private Methode ToXLCellValue ermitteln: BindingFlags.Instance | BindingFlags.NonPublic.
// 2. Instanz von TableExcelExporter erzeugen.
// 3. DataTestMethod mit mehreren DataRow Fällen:
//    - null -> string.Empty
//    - DBNull.Value -> string.Empty
//    - bool true/false -> 1/0 (int)
//    - int -> int
//    - long im int-Bereich -> int
//    - long außerhalb int-Bereich -> double
//    - float/double Ganzzahl-Wert -> int
//    - float/double mit Nachkommastellen -> double
//    - double.NaN / double.PositiveInfinity -> kulturinvariant string ("NaN","Infinity")
//    - string leer/Whitespace -> string.Empty
//    - string Integer -> int
//    - string Double -> double
// 4. Methode via MethodInfo.Invoke aufrufen, Ergebnis (XLCellValue) typ und Wert prüfen.
// 5. Separater Test: Interface ITableExporter via NSubstitute erzeugen und verifizieren,
//    dass Reflection auf Interface die Methode nicht findet, aber auf konkretem Typ schon.
// 6. Assertions kurz und präzise.
// 7. Nutzung NSubstitute sicherstellen (Substitute.For<ITableExporter>()).

[TestClass]
public class TableExcelExporterTests
{
    private static MethodInfo GetPrivateMethod(string name)
        => typeof(TableExcelExporter).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)
           ?? throw new InvalidOperationException($"Methode {name} nicht gefunden.");

    [DataTestMethod]
    [DataRow(TypeCode.Empty, null, "", TypeCode.String)]                     // null -> ""
    [DataRow(TypeCode.DBNull, null, "", TypeCode.String)]                    // DBNull -> ""
    [DataRow(TypeCode.Boolean, true, true, TypeCode.Boolean)]                     // true -> 1
    [DataRow(TypeCode.Boolean, false, false, TypeCode.Boolean)]                    // false -> 0
    [DataRow(TypeCode.Int32, 5, 5, TypeCode.Int32)]                          // int bleibt int
    [DataRow(TypeCode.Int64, 5L, 5, TypeCode.Int32)]                         // long im int-Bereich -> int
    [DataRow(TypeCode.Int64, 5000000000L, 5000000000d, TypeCode.Double)]     // long außerhalb int-Bereich -> double
    [DataRow(TypeCode.Single, 5.0f, 5, TypeCode.Int32)]                      // float Ganzzahl -> int
    [DataRow(TypeCode.Single, 5.25f, 5.25d, TypeCode.Double)]                // float mit Nachkommastellen -> double
    [DataRow(TypeCode.Double, 5.0d, 5, TypeCode.Int32)]                      // double Ganzzahl -> int
    [DataRow(TypeCode.Double, 5.125d, 5.125d, TypeCode.Double)]              // double mit Nachkommastellen -> double
    [DataRow(TypeCode.Double, double.NaN, "NaN", TypeCode.String)]           // NaN -> string "NaN"
    [DataRow(TypeCode.Double, double.PositiveInfinity, "Infinity", TypeCode.String)] // Infinity -> string
    [DataRow(TypeCode.String, "   ", "", TypeCode.String)]                   // Whitespace -> ""
    [DataRow(TypeCode.String, "True", true, TypeCode.Boolean)]                     // String int -> int
    [DataRow(TypeCode.String, "false", false, TypeCode.Boolean)]                     // String int -> int
    [DataRow(TypeCode.String, "42", 42, TypeCode.Int32)]                     // String int -> int
    [DataRow(TypeCode.String, "42.75", 42.75d, TypeCode.Double)]             // String double (.) -> double
    [DataRow(TypeCode.String, "1e-6", 1e-6d, TypeCode.Double)]             // String double (.) -> double
    [DataRow(TypeCode.String, "-4.2e3", -4.2e3d, TypeCode.Double)]             // String double (.) -> double
    [DataRow(TypeCode.String, "42,75", 42.75d, TypeCode.Double)]             // String double (,) -> double
    [DataRow(TypeCode.String, "42.7.5", "42.7.5", TypeCode.String)]          // Ungültig -> string unverändert
    [DataRow(TypeCode.String, "0010", "0010", TypeCode.String)]              // Führende Nullen -> string
    [DataRow(TypeCode.String, "0", 0, TypeCode.Int32)]                       // nur Null -> int
    public void ToXLCellValue_Test(TypeCode inputTypeCode, object? inputRaw, object expectedValue, TypeCode expectedTypeCode)
    {
        // Eingabeobjekt aus TypeCode ableiten
        object? input = inputTypeCode switch
        {
            TypeCode.Empty => null,
            TypeCode.DBNull => DBNull.Value,
            _ => inputRaw
        };

        // Erwarteten Typ aus TypeCode bestimmen
        Type expectedType = expectedTypeCode switch
        {
            TypeCode.String => typeof(string),
            TypeCode.Int32 => typeof(int),
            TypeCode.Boolean => typeof(bool),
            TypeCode.Double => typeof(double),
            _ => throw new AssertFailedException("Nicht unterstützter erwarteter TypeCode.")
        };

        var exporter = new TableExcelExporter();
        var mi = GetPrivateMethod("ToXLCellValue");

        var xl = (XLCellValue)mi.Invoke(exporter, new[] { input })!;

        object actualValue;
        TypeCode actualTC;
        if (xl.IsBlank)
        {
            actualValue = "";
            actualTC = TypeCode.String;
        }
        else if (xl.IsText)
        {
            actualValue = xl.GetText();
            actualTC = TypeCode.String;
        }
        else if (xl.IsNumber)
        { 
            actualValue = xl.GetNumber();
            actualTC = Math.Abs((double)actualValue % 1d) < 1e-10 ? TypeCode.Int32 : TypeCode.Double;
        }
        else if (xl.IsBoolean)
        {
            actualValue = xl.GetBoolean();
            actualTC = TypeCode.Boolean;
        }
        else
        {
            actualValue = xl.ToString();
            actualTC = TypeCode.String;
        }

        if (expectedType == typeof(string))
        {
            Assert.AreEqual(expectedValue.ToString(), actualValue?.ToString(), "Stringwert stimmt nicht.");
            return;
        }

        if (expectedType == typeof(int))
        {
            Assert.AreEqual(expectedTypeCode, actualTC , "Erwartet int.");
            Assert.AreEqual((int)expectedValue, (int)Math.Round((double)actualValue), "int-Wert stimmt nicht.");
        }
        else if (expectedType == typeof(double))
        {
            Assert.IsTrue(actualValue is double or int, "Erwartet double oder int Repräsentation.");
            double actualDouble = actualValue is int ai ? ai : (double)actualValue;
            Assert.AreEqual(Convert.ToDouble(expectedValue), actualDouble, 1e-12, "double-Wert stimmt nicht.");
        }
        else if (expectedType == typeof(bool))
        {
            Assert.IsInstanceOfType(actualValue, typeof(bool), "Erwartet bool.");
            Assert.AreEqual((bool)expectedValue, (bool)actualValue, "bool-Wert stimmt nicht.");
        }
        else
        {
            Assert.Fail("Nicht unterstützter erwarteter Typ.");
        }
    }

    [TestMethod]
    public void Reflection_Finden_Der_Privaten_Methode_Ueber_Konkreten_Typ_Nicht_Ueber_Interface()
    {
        // Arrange
        var ifaceSub = Substitute.For<DataAnalysis.Core.Export.Interfaces.ITableExporter>();
        var viaInterface = ifaceSub.GetType().GetMethod("ToXLCellValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        var viaConcrete = typeof(TableExcelExporter).GetMethod("ToXLCellValue", BindingFlags.Instance | BindingFlags.NonPublic);

        // Assert
        Assert.IsNull(viaInterface, "Private Methode sollte über Interface-Proxy nicht gefunden werden.");
        Assert.IsNotNull(viaConcrete, "Private Methode sollte über konkreten Typ gefunden werden.");
    }
}