using AA28_DataGridExt.Model;
using System;

namespace AA28_DataGridExt.Tests.Models;

[TestClass]
public class PersonTests
{
    [TestMethod]
    public void PropertiesRoundTripTest()
    {
        var birthday = new DateTime(2001, 1, 1);
        var department = new Department { Id = 2, Name = "Sales" };
        var person = new Person
        {
            Id = 7,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            Birthday = birthday,
            Department = department,
        };

        Assert.AreEqual(7, person.Id);
        Assert.AreEqual("Jane", person.FirstName);
        Assert.AreEqual("Doe", person.LastName);
        Assert.AreEqual("jane.doe@example.com", person.Email);
        Assert.AreEqual(birthday, person.Birthday);
        Assert.AreSame(department, person.Department);
    }

    [TestMethod]
    public void BirthdayOffsetRoundTripSynchronizesBirthdayTest()
    {
        var birthdayOffset = new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var person = new Person();

        person.BirthdayOffset = birthdayOffset;

        Assert.AreEqual(birthdayOffset.DateTime, person.Birthday);
        Assert.AreEqual(birthdayOffset, person.BirthdayOffset);
    }
}
