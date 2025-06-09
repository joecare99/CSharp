using BaseLib.Helper;
using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Interfaces.DB.Tests;

[TestClass]
public class IFieldsCollectionTests
{
    [TestMethod]
    public void IFieldsCollection_ContainsField_ReturnsTrueForExistingField()
    {
        // Arrange
        var fieldsCollection = Substitute.For<IFieldsCollection>();
        (fieldsCollection["TestField"] as IHasValue).Value.Returns(1,2,3);
        (fieldsCollection[2] as IHasValue).Value.Returns(1.1,2.2,3.3);
        (fieldsCollection[TypeCode.Double] as IHasValue).Value.Returns("1","2","3");
        // Act
        var result1 = fieldsCollection[TypeCode.Double].AsString();
        var result2 = fieldsCollection[2].AsDouble();
        var result3 = fieldsCollection["TestField"].AsInt();
        // Assert
        Assert.AreEqual("1", result1);
        Assert.AreEqual(1.1, result2);
        Assert.AreEqual(1, result3);
    }
}
