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
    public class TemplateViewModelTests
    {
        private ITemplateModel? model;
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        TemplateViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private string sTestLog="";
        private CultureInfo? cc;

        [TestInitialize()]
        public void TestInitialize()
        {
            cc = CultureInfo.CurrentCulture ;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            model = Substitute.For<ITemplateModel>();
            testModel = new TemplateViewModel(model);
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
            Assert.IsInstanceOfType(testModel, typeof(TemplateViewModel));
            Assert.IsInstanceOfType(testModel, typeof(ITemplateViewModel));
        }

        [TestMethod()]
        public void SetupTest2()
        {
            var testModel2 = new TemplateViewModel();
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel2, typeof(TemplateViewModel));
            Assert.IsInstanceOfType(testModel2, typeof(ITemplateViewModel));
        }


        [TestMethod()]
        [DataRow("Hello World !", @"PropChg(TemplateViewModel, Greeting) = $Hello World !\r\n")]
        [DataRow("Dada", @"PropChg(TemplateViewModel, Greeting) = $Dada\r\n")]

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
        [DataRow("Hello World !", @"PropChg(TemplateViewModel, Title) = $Hello World !\r\n")]
        [DataRow("menu grande", @"PropChg(TemplateViewModel, Title) = $menu grande\r\n")]
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
        [DataRow("now", @"PropChg(TemplateViewModel, Now) = $01/14/2025 00:00:00\r\n")]
        [DataRow("title", @"PropChg(TemplateViewModel, Title) = $Main Menu\r\n")]
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
        [DataRow(nameof(TemplateViewModel.ActionsCommand), "Action:", @"PropChg(TemplateViewModel, Greeting) = $Action:\r\n")]
        [DataRow(nameof(TemplateViewModel.ConfigCommand), "Config:", @"PropChg(TemplateViewModel, Greeting) = $Config:\r\n")]
        [DataRow(nameof(TemplateViewModel.HomeCommand), "Hello, Avalonia!", @"PropChg(TemplateViewModel, Greeting) = $Hello, Avalonia!\r\n")]
        [DataRow(nameof(TemplateViewModel.MacrosCommand), "Macros:", @"PropChg(TemplateViewModel, Greeting) = $Macros:\r\n")]
        [DataRow(nameof(TemplateViewModel.HistoryCommand), "History:", @"PropChg(TemplateViewModel, Greeting) = $History:\r\n")]
        [DataRow(nameof(TemplateViewModel.ProcessCommand), "Process:", @"PropChg(TemplateViewModel, Greeting) = $Process:\r\n")]
        [DataRow(nameof(TemplateViewModel.ReportsCommand), "Reports:", @"PropChg(TemplateViewModel, Greeting) = $Reports:\r\n")]
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