using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_06_Converters_4.Model;
using NSubstitute;
using System;
using System.Collections.Generic;
using BaseLib.Helper;
using MVVM.ViewModel;
using System.ComponentModel;
using MVVM.View.Extension;

namespace MVVM_06_Converters_4.ViewModel.Tests
{
    [TestClass()]
    public class VehicleViewModelTests : BaseTestViewModel<VehicleViewModel>
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private IAGVModel _testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public override void Init()
        {
            _testModel = Substitute.For<IAGVModel>();
            _testModel.SwivelKoor.Returns(new MathLibrary.TwoDim.Math2d.Vector());
            _testModel.AGVVelocity.Returns(new MathLibrary.TwoDim.Math2d.Vector());
            _testModel.VehicleDim.Returns(new MathLibrary.TwoDim.Math2d.Vector());
            _testModel.Dependencies.Returns(returnThis: new (string Dest, string Src)[] { ("1","2") });
            IoC.GetReqSrv=(t)=>t switch
            {
                _ when t == typeof(IAGVModel) => _testModel,
                _ => throw new NotImplementedException()
            };
            base.Init();
        }
        [TestMethod()]
        public void TestSetUp()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(VehicleViewModel));
            Assert.IsInstanceOfType(testModel, typeof(BaseViewModel));
        }

        static IEnumerable<object[]> VehicleViewModelPropertyTestData
        {
            get
            {
                foreach (var p in typeof(VehicleViewModel).GetProperties())
                    if (p.CanWrite)
                        switch (p.PropertyType.TC())
                        {
                            case TypeCode.String:
                                yield return new object[] { p.Name, "Null", null!, null! };
                                yield return new object[] { p.Name, "Empty", "", "" };
                                yield return new object[] { p.Name, "Peter", "Peter", "Peter" };
                                yield return new object[] { p.Name, "Müller", "Müller", "Müller" };
                                break;
                            case TypeCode.Int32:
                                yield return new object[] { p.Name, "0", 0, 0 };
                                yield return new object[] { p.Name, "1", 1, 1 };
                                yield return new object[] { p.Name, "-1", -1, -1 };
                                yield return new object[] { p.Name, "MaxInt", int.MaxValue, int.MaxValue };
                                yield return new object[] { p.Name, "MinInt", int.MinValue, int.MinValue };
                                break;
                            case TypeCode.Double:
                                yield return new object[] { p.Name, "0", 0.0d, 0.0d };
                                yield return new object[] { p.Name, "1", 1d, 1d };
                                yield return new object[] { p.Name, "-1", -1d, -1d };
                                yield return new object[] { p.Name, "MaxDouble", double.MaxValue, double.MaxValue };
                                yield return new object[] { p.Name, "MinDouble", double.MinValue, double.MinValue };
                                yield return new object[] { p.Name, "Epsilon", double.Epsilon, double.Epsilon };
                                break;
                            case TypeCode.Object when p.PropertyType == typeof(DateTime?):
                                yield return new object[] { p.Name, "Null", null!, null! };
                                yield return new object[] { p.Name, "0", (DateTime?)new DateTime(1980, 1, 1), (DateTime?)new DateTime(1980, 1, 1) };
                                yield return new object[] { p.Name, "1", (DateTime?)new DateTime(2001, 1, 1), new DateTime(2001, 1, 1) };
                                yield return new object[] { p.Name, "Today", (DateTime?)DateTime.Today, DateTime.Today };
                                yield return new object[] { p.Name, "MaxDate", (DateTime?)DateTime.MaxValue, DateTime.MaxValue };
                                yield return new object[] { p.Name, "MinDate", (DateTime?)DateTime.MinValue, DateTime.MinValue };
                                break;
                            default:
                                yield return new object[] { p.Name, "Null", null!, null! };
                                break;
                        }
                    else if (p.PropertyType == typeof(Double))
                        yield return new object[] { p.Name, "ro", 0d, 0d };
                    else if (p.PropertyType == typeof(Int32))
                        yield return new object[] { p.Name, "ro", 0, 0 };
                    else if (p.PropertyType == typeof(Boolean))
                        yield return new object[] { p.Name, "ro", false, false };
                //else
                //  yield return new object[] { p.Name, "ro", null!, null! };
            }
        }
        [DataTestMethod]
        [DynamicData(nameof(VehicleViewModelPropertyTestData))]
        public void TestProperties(string sProp, string sName, object oVal, object oExp)
        {
            if (oVal is DateTime?)
                testModel.SetProp<VehicleViewModel, DateTime?>(sProp, oVal as DateTime?);
            else if (sName != "ro")
                testModel.SetProp(sProp, oVal);
            Assert.AreEqual(oExp, testModel.GetProp(sProp));
        }
        [DataTestMethod()]
        [DataRow(nameof(IAGVModel.VehicleDim), new[] { @"PropChg(MVVM_06_Converters_4.ViewModel.VehicleViewModel,VehicleLength)=0
PropChg(MVVM_06_Converters_4.ViewModel.VehicleViewModel,VehicleWidth)=0
" })]
        [DataRow(nameof(IAGVModel.SwivelKoor), new[] { @"PropChg(MVVM_06_Converters_4.ViewModel.VehicleViewModel,SwivelKoorX)=0
PropChg(MVVM_06_Converters_4.ViewModel.VehicleViewModel,SwivelKoorY)=0
" })]
        [DataRow(nameof(IAGVModel.AxisOffset), new[] { @"PropChg(MVVM_06_Converters_4.ViewModel.VehicleViewModel,AxisOffset)=0
" })]
        [DataRow(nameof(IAGVModel.AGVVelocity), new[] { @"PropChg(MVVM_06_Converters_4.ViewModel.VehicleViewModel,VehicleVelocityX)=0
PropChg(MVVM_06_Converters_4.ViewModel.VehicleViewModel,VehicleVelocityY)=0
PropChg(MVVM_06_Converters_4.ViewModel.VehicleViewModel,AGVVelocity)=( 0, 0)
" })]
        [DataRow(nameof(IAGVModel.VehicleRotation), new[] { @"PropChg(MVVM_06_Converters_4.ViewModel.VehicleViewModel,VehicleRotation)=0
" })]
        [DataRow(null, new[] { @"" })]
        public void OnPropertyChangedTest(string sProp, string[] asExp)
        {
            _testModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangedEventArgs(sProp));
            Assert.AreEqual(asExp[0], DebugLog);
        }

        protected override Dictionary<string, object?> GetDefaultData() => new();
    }
}