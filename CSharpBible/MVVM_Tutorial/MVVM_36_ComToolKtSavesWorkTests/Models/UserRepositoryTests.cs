using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVVM_36_ComToolKtSavesWork.Models.Tests;

[TestClass]
public class UserRepositoryTests
{
    private UserRepository testModel;

    [DataTestMethod()]
    [DataRow("Test", "123")]
    [DataRow("1", "2")]
    public void LoginTest(string name,string password)
    {
        testModel = new UserRepository();
        var user = testModel.Login(name,password);
        Assert.IsNotNull(user);
        Assert.AreEqual("DevDave",user.Username);
        Assert.AreEqual(1, user.Id);
    }
}
