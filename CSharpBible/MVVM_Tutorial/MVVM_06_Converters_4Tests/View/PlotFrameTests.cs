using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.View.Extension;
using MVVM_06_Converters_4.Model;
using NSubstitute;
using System;

namespace MVVM_06_Converters_4.View.Tests
{
    [TestClass()]
    public class PlotFrameTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        PlotFrame testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private Func<Type, object> _grsOld;
        private IAGVModel? _model;

        [TestInitialize]
        public void TestInitialize()
        {
            _grsOld = IoC.GetReqSrv;
            IoC.GetReqSrv = (t) => t switch
            {
                Type _t when _t == typeof(IAGVModel) => _model ??= Substitute.For<IAGVModel>(),
                _ => throw new System.NotImplementedException($"No Service for {t}")
            }; 
            testView = new PlotFrame();
            testView.Visibility = System.Windows.Visibility.Visible;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            IoC.GetReqSrv = _grsOld;
        }

        [TestMethod()]
        public void PlotFrameTest()
        {
            Assert.IsNotNull(testView);
            Assert.IsNotNull(_model);
        }

        [TestMethod()]
        public void OnSizeChangeTest()
        {
            testView.Height = testView.Height+2;
        }

        [TestMethod()]
        public void OnVInitializedTest()
        {
           // testView.OnInitialized(;
        }
    }
}