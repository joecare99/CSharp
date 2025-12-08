using NSubstitute;
using AA05_CommandParCalc.Models.Interfaces;
using System.Diagnostics;
using AA05_CommandParCalc.ViewModels.Interfaces;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace AA05_CommandParCalc.ViewModels.Tests
{
    [TestClass()]
    public class CommandParCalcViewModelTests
    {
        private ICommandParCalcModel? model;
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        CommandParCalcViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private string sTestLog="";
        private CultureInfo? cc;

        [TestInitialize()]
        public void TestInitialize()
        {
            cc = CultureInfo.CurrentCulture ;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            model = Substitute.For<ICommandParCalcModel>();
            testModel = new CommandParCalcViewModel(model);
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
            Assert.IsInstanceOfType(testModel, typeof(CommandParCalcViewModel));
            Assert.IsInstanceOfType(testModel, typeof(ICommandParCalcViewModel));
        }

        [TestMethod()]
        public void SetupTest2()
        {
            var testModel2 = new CommandParCalcViewModel();
            Assert.IsNotNull(testModel2);
            Assert.IsInstanceOfType(testModel2, typeof(CommandParCalcViewModel));
            Assert.IsInstanceOfType(testModel2, typeof(ICommandParCalcViewModel));
        }


        [TestMethod()]
        [DataRow("Hello World !", @"PropChg(CommandParCalcViewModel, Greeting) = $Hello World !\r\n")]
        [DataRow("Dada", @"PropChg(CommandParCalcViewModel, Greeting) = $Dada\r\n")]

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
        [DataRow("Hello World !", @"PropChg(CommandParCalcViewModel, Title) = $Hello World !\r\n")]
        [DataRow("menu grande", @"PropChg(CommandParCalcViewModel, Title) = $menu grande\r\n")]
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
        [DataRow("now", @"PropChg(CommandParCalcViewModel, Now) = $01/14/2025 00:00:00\r\n")]
        [DataRow("title", @"PropChg(CommandParCalcViewModel, Title) = $Main Menu\r\n")]
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
        [DataRow(nameof(CommandParCalcViewModel.ActionsCommand), "Action:", @"PropChg(CommandParCalcViewModel, Greeting) = $Action:\r\n")]
        [DataRow(nameof(CommandParCalcViewModel.ConfigCommand), "Config:", @"PropChg(CommandParCalcViewModel, Greeting) = $Config:\r\n")]
        [DataRow(nameof(CommandParCalcViewModel.HomeCommand), "Hello, Avalonia!", @"PropChg(CommandParCalcViewModel, Greeting) = $Hello, Avalonia!\r\n")]
        [DataRow(nameof(CommandParCalcViewModel.MacrosCommand), "Macros:", @"PropChg(CommandParCalcViewModel, Greeting) = $Macros:\r\n")]
        [DataRow(nameof(CommandParCalcViewModel.HistoryCommand), "History:", @"PropChg(CommandParCalcViewModel, Greeting) = $History:\r\n")]
        [DataRow(nameof(CommandParCalcViewModel.ProcessCommand), "Process:", @"PropChg(CommandParCalcViewModel, Greeting) = $Process:\r\n")]
        [DataRow(nameof(CommandParCalcViewModel.ReportsCommand), "Reports:", @"PropChg(CommandParCalcViewModel, Greeting) = $Reports:\r\n")]
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
