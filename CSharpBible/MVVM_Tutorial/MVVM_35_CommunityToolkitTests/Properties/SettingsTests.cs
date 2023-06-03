// ***********************************************************************
// Assembly         : MVVM_35_CommunityToolkit_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="SettingsTests.cs" company="JC-Soft">
//     Copyright � JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

/// <summary>
/// The Tests namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_35_CommunityToolkit.Properties.Tests
{
    /// <summary>
    /// Defines test class SettingsTests.
    /// </summary>
    /// <autogeneratedoc />
    [TestClass()]
    public class SettingsTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test item
        /// </summary>
        /// <autogeneratedoc />
        Settings testItem;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void Init()
        {
            testItem = new();
        }

        /// <summary>
        /// Defines the test method SetupTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testItem);
            Assert.IsInstanceOfType(testItem, typeof(Settings));
            Assert.IsInstanceOfType(testItem, typeof(ApplicationSettingsBase));
        }
        /// <summary>
        /// Defines the test method DefaultInstanceTest.
        /// </summary>
        /// <autogeneratedoc />
        [TestMethod()]
        public void DefaultInstanceTest()
        {
            Assert.IsNotNull(Settings.Default);
            Assert.IsInstanceOfType(Settings.Default, typeof(Settings));
            Assert.IsInstanceOfType(Settings.Default, typeof(ApplicationSettingsBase));
        }
    }
}
