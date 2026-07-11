using GenFree.Data.DB;
using GenFree.Interfaces.DB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Data.DB.Tests;

[TestClass]
public class DBImplementOleDBTests
{
    [TestMethod]
    [DataRow(typeof(IDBEngine), typeof(DBImplementOleDB.DBEngine))]
    [DataRow(typeof(IDBWorkSpace), typeof(DBImplementOleDB.CWorkSpace))]
    [DataRow(typeof(IDatabase), typeof(DBImplementOleDB.CDatabase))]
    [DataRow(typeof(IRecordset), typeof(DBImplementOleDB.Recordset))]
    [DataRow(typeof(IFieldsCollection), typeof(DBImplementOleDB.FieldsCollection))]
    [DataRow(typeof(IField), typeof(DBImplementOleDB.Field))]
    public void AddOleDB_ShouldRegisterServicesCorrectly(Type serviceType, Type implementationType)
    {
        // Arrange
        var services = Substitute.For<IServiceCollection>();

        // Act
        DBImplementOleDB.AddOleDB(services);

        // Assert
        services.Received(1).Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == serviceType &&
            descriptor.ImplementationType == implementationType &&
            (descriptor.Lifetime == ServiceLifetime.Singleton || descriptor.Lifetime == ServiceLifetime.Transient)
        ));
    }
}