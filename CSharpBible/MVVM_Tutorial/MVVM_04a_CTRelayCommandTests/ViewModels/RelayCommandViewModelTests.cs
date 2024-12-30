using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using static BaseLib.Helper.TestHelper;

namespace MVVM_04a_CTRelayCommand.ViewModels.Tests;

[TestClass()]
public class RelayCommandViewModelTests : BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    RelayCommandViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testModel = new();
        testModel.PropertyChanged += OnVMPropertyChanged;
        testModel.PropertyChanging += OnVMPropertyChanging;
        testModel.ClearCommand.CanExecuteChanged += OnCanExChanged;
        ClearLog();
    }

    [TestMethod]
    public void RelayCommandViewModelTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(RelayCommandViewModel));
        Assert.IsNotNull(testModel.ClearCommand);
        Assert.IsInstanceOfType(testModel.ClearCommand, typeof(RelayCommand));
        Assert.AreEqual("Dave", testModel.Firstname);
        Assert.AreEqual("Dev", testModel.Lastname);
        Assert.AreEqual("Dev, Dave", testModel.Fullname);
        Assert.AreEqual("", DebugLog);
    }

    [DataTestMethod()]
    [DataRow("", new string[] { "","Dev, ", @"PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=Dave
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=Dev, 
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    [DataRow("Peter", new string[] { "Peter", "Dev, Peter", @"PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=Dave
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=Peter
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=Dev, Peter
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    [DataRow("Steve\tEugene", new string[] { "Eugene", "Dev, Eugene", @"PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=Dave
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=Steve
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=Dev, Steve
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=Steve
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=Eugene
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=Dev, Eugene
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    public void FirstnameTest(string name,  string[] asExp)
    {
        // Arrange
        var asName=name.Split('\t');
        // Act    
        foreach (var item in asName) 
            testModel.Firstname = item;
        // Assert
        Assert.AreEqual(asExp[0], testModel.Firstname,"Firstname");
        Assert.AreEqual(asExp[1], testModel.Fullname, "Fullname");
        AssertAreEqual(asExp[2], DebugLog, "DebugOut");
    }

    [DataTestMethod()]
    [DataRow("", new string[] { "", ", Dave", @"PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=Dev
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=, Dave
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    [DataRow("Miller", new string[] { "Miller", "Miller, Dave", @"PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=Dev
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=Miller
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=Miller, Dave
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    [DataRow("Fry\tWebb", new string[] { "Webb", "Webb, Dave", @"PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=Dev
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=Fry
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=Fry, Dave
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=Fry
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=Webb
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=Webb, Dave
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
" })]
    public void LastnameTest(string name, string[] asExp)
    {
        // Arrange
        var asName = name.Split('\t');
        // Act    
        foreach (var item in asName)
            testModel.Lastname = item;
        // Assert
        Assert.AreEqual(asExp[0], testModel.Lastname, "Firstname");
        Assert.AreEqual(asExp[1], testModel.Fullname, "Fullname");
        AssertAreEqual(asExp[2], DebugLog, "DebugOut");
    }

    [TestMethod]
    public void ClearCommandExecuteTest()
    {
        testModel.ClearCommand.Execute(null);
        Assert.AreEqual("", testModel.Firstname);
        Assert.AreEqual("", testModel.Lastname);
        Assert.AreEqual(", ", testModel.Fullname);
        AssertAreEqual(@"PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=Dave
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Firstname)=
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=Dev, 
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=True
PropChgn(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=Dev
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Lastname)=
PropChg(MVVM_04a_CTRelayCommand.ViewModels.RelayCommandViewModel,Fullname)=, 
CanExChanged(CommunityToolkit.Mvvm.Input.RelayCommand)=False
", DebugLog);
    }

}
