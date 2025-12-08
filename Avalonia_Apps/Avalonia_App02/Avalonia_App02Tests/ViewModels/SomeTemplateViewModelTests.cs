using NSubstitute;
using Avalonia_App02.Models.Interfaces;
using System.Diagnostics;
using Avalonia_App02.ViewModels.Interfaces;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace Avalonia_App02.ViewModels.Tests
{
    [TestClass()]
    public class SomeTemplateViewModelTests
    {
        private ISomeTemplateModel? model;
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        SomeTemplateViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private string sTestLog="";
        private CultureInfo? cc;

        [TestInitialize()]
        public void TestInitialize()
        {
            cc = CultureInfo.CurrentCulture ;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            model = Substitute.For<ISomeTemplateModel>();
            testModel = new SomeTemplateViewModel(model);
            testModel.PropertyChanged += (s, e) => DoLog($"PropChg({s?.GetType().Name}, {e.PropertyName}) = ${s?.GetType().GetProperty(e.PropertyName??"")?.GetValue(s)}");            
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Debug.WriteLine(sTestLog);
            CultureInfo.CurrentCulture = cc!;
        }

        private void DoLog(string v)
        {
            Debug.WriteLine(v);
            sTestLog += $"{v}\r\n";
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(SomeTemplateViewModel));
            Assert.IsInstanceOfType(testModel, typeof(ISomeTemplateViewModel));
        }

        [TestMethod()]
        public void SetupTest2()
        {
            var testModel2 = new SomeTemplateViewModel();
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel2, typeof(SomeTemplateViewModel));
            Assert.IsInstanceOfType(testModel2, typeof(ISomeTemplateViewModel));
        }


        [TestMethod()]
        [DataRow("Hello World !", @"PropChg(SomeTemplateViewModel, Greeting) = $Hello World !\r\n")]
        [DataRow("Dada", @"PropChg(SomeTemplateViewModel, Greeting) = $Dada\r\n")]

        public void GreetingsTest(string sAct,string sExp)
        {
            // Arrange & basetest
            Assert.AreEqual("Welcome to Avalonia! The current time is 01/01/0001 00:00:00", testModel.Greeting);

            // Act
            testModel.Greeting = sAct;

            // Assert
            Assert.AreEqual(sAct, testModel.Greeting);
            Assert.AreEqual(sExp.Replace("\\r\\n","\r\n"), sTestLog);
        }

        [TestMethod()]
        [DataRow("Hello World !", @"PropChg(SomeTemplateViewModel, Title) = $Hello World !\r\n")]
        [DataRow("menu grande", @"PropChg(SomeTemplateViewModel, Title) = $menu grande\r\n")]
        [DataRow("Main Menu", @"")]
        public void TitleTest1(string sAct, string sExp)
        {
            // Arrange & basetest
            Assert.AreEqual("Main Menu", testModel.Title);

            // Act
            testModel.Title = sAct;

            // Assert
            Assert.AreEqual(sAct, testModel.Title);
            Assert.AreEqual(sExp.Replace("\\r\\n", "\r\n"), sTestLog);
        }

        [TestMethod()]
        [DataRow("", "")]
        [DataRow("now", @"PropChg(SomeTemplateViewModel, Now) = $01/14/2025 00:00:00\r\n")]
        [DataRow("title", @"PropChg(SomeTemplateViewModel, Title) = $Main Menu\r\n")]
        [DataRow("bumlux", "")]
        public void PropertyChangedTest2(string sAct, string sExp)
        {
            // Arrange & basetest
            DateTime _dt;
            model!.Now.Returns(_dt=new DateTime(2025,1,14));

            // Act
            model.PropertyChanged += Raise.Event<PropertyChangedEventHandler>([model, new PropertyChangedEventArgs(sAct)]);
            
            // Assert
            Assert.AreEqual(_dt, testModel.Now);
            Assert.AreEqual(sExp.Replace("\\r\\n", "\r\n"), sTestLog);
        }

        [TestMethod()]
        [DataRow("", "Welcome to Avalonia! The current time is 01/01/0001 00:00:00", "")]
        [DataRow(nameof(SomeTemplateViewModel.ActionsCommand), "Action:", @"PropChg(SomeTemplateViewModel, Greeting) = $Action:\r\n")]
        [DataRow(nameof(SomeTemplateViewModel.ConfigCommand), "Config:", @"PropChg(SomeTemplateViewModel, Greeting) = $Config:\r\n")]
        [DataRow(nameof(SomeTemplateViewModel.HomeCommand), "Hello, Avalonia!", @"PropChg(SomeTemplateViewModel, Greeting) = $Hello, Avalonia!\r\n")]
        [DataRow(nameof(SomeTemplateViewModel.MacrosCommand), "Macros:", @"PropChg(SomeTemplateViewModel, Greeting) = $Macros:\r\n")]
        [DataRow(nameof(SomeTemplateViewModel.HistoryCommand), "History:", @"PropChg(SomeTemplateViewModel, Greeting) = $History:\r\n")]
        [DataRow(nameof(SomeTemplateViewModel.ProcessCommand), "Process:", @"PropChg(SomeTemplateViewModel, Greeting) = $Process:\r\n")]
        [DataRow(nameof(SomeTemplateViewModel.ReportsCommand), "Reports:", @"PropChg(SomeTemplateViewModel, Greeting) = $Reports:\r\n")]
        public void RelayCommandTest(string sAct,string sExp1,string sExp2)
        {
            // Arrange & basetest
            Assert.AreEqual("Welcome to Avalonia! The current time is 01/01/0001 00:00:00", testModel.Greeting);

            // Act
            if (testModel.GetType().GetProperty(sAct)?.GetValue(testModel) is IRelayCommand iRc)
                iRc.Execute(null);

            // Assert
            Assert.AreEqual(sExp1, testModel.Greeting);
            Assert.AreEqual(sExp2.Replace("\\r\\n", "\r\n"), sTestLog);
        }

    }
}