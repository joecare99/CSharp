using AA28_DataGridExt.Model;

namespace AA28_DataGridExt.Tests.Models;

[TestClass]
public class DepartmentTests
{
    [TestMethod]
    public void PropertiesRoundTripTest()
    {
        var department = new Department
        {
            Id = 3,
            Name = "IT",
            Description = "Builds and supports the technical landscape.",
        };

        Assert.AreEqual(3, department.Id);
        Assert.AreEqual("IT", department.Name);
        Assert.AreEqual("Builds and supports the technical landscape.", department.Description);
    }

    [TestMethod]
    public void ToStringReturnsNameTest()
    {
        var department = new Department
        {
            Name = "Sales",
        };

        Assert.AreEqual("Sales", department.ToString());
    }
}
