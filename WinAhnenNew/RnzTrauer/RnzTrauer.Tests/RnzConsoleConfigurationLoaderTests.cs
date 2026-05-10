using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Console.Configuration;
using RnzTrauer.Core;
using System;
using System.Collections.Generic;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class RnzConsoleConfigurationLoaderTests
{
    [TestMethod]
    public void ApplySecrets_OverridesSensitiveValues_FromSectionKeys()
    {
        var xConfig = new RnzConfig
        {
            User = "app-user",
            Password = new(),
            DBuser = "app-db-user",
            DBpass = new(),
            DBhost = "db-host",
            DB = "db-name"
        };
        xConfig.Password.AppendChar('a'); // Placeholder to ensure it's not empty
        xConfig.DBpass.AppendChar('b'); // Placeholder to ensure it's not empty
        var xSecrets = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RnzConfig:User"] = "secret-user",
                ["RnzConfig:Password"] = "secret-password",
                ["RnzConfig:DBpass"] = "secret-db-password"
            })
            .Build();

        RnzConsoleConfigurationLoader.ApplySecrets(xConfig, xSecrets);

        Assert.AreEqual("secret-user", xConfig.User);
        Assert.AreEqual("secret-password", new System.Net.NetworkCredential(string.Empty, xConfig.Password).Password);
        Assert.AreEqual("app-db-user", xConfig.DBuser);
        Assert.AreEqual("secret-db-password", new System.Net.NetworkCredential(string.Empty, xConfig.DBpass).Password);
        Assert.AreEqual("db-host", xConfig.DBhost);
        Assert.AreEqual("db-name", xConfig.DB);
    }

    [TestMethod]
    public void ApplySecrets_UsesFlatKeys_WhenSectionKeysAreMissing()
    {
        var xConfig = new RnzConfig
        {
            User = string.Empty,
            Password = new(),
            DBuser = "app-db-user",
            DBpass =  new(),
            DBhost = "app-db-host",
            DB = "app-db"
        };
        var xSecrets = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [nameof(RnzConfig.User)] = "flat-user",
                [nameof(RnzConfig.Password)] = "flat-password",
                [nameof(RnzConfig.DBuser)] = "flat-db-user",
                [nameof(RnzConfig.DBpass)] = "flat-db-password"
            })
            .Build();

        RnzConsoleConfigurationLoader.ApplySecrets(xConfig, xSecrets);

        Assert.AreEqual("flat-user", xConfig.User);
        Assert.AreEqual("flat-password", new System.Net.NetworkCredential(string.Empty, xConfig.Password).Password);
        Assert.AreEqual("flat-db-user", xConfig.DBuser);
        Assert.AreEqual("flat-db-password", new System.Net.NetworkCredential(string.Empty, xConfig.DBpass).Password);
        Assert.AreEqual("app-db-host", xConfig.DBhost);
        Assert.AreEqual("app-db", xConfig.DB);
    }

    [TestMethod]
    public void Validate_Throws_WhenRequiredValuesAreMissing()
    {
        var xConfig = new RnzConfig
        {
            Url = "https://example.invalid",
            Title = "RNZ",
            LocalPath = "C:/temp",
            DBhost = "db-host",
            DB = "db-name"
        };

        InvalidOperationException? xException = null;

        try
        {
            RnzConsoleConfigurationLoader.Validate(xConfig);
            Assert.Fail("Expected InvalidOperationException was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            xException = ex;
        }

        Assert.IsNotNull(xException);
        StringAssert.Contains(xException.Message, nameof(RnzConfig.User));
        StringAssert.Contains(xException.Message, nameof(RnzConfig.Password));
        StringAssert.Contains(xException.Message, nameof(RnzConfig.DBuser));
    }
}
