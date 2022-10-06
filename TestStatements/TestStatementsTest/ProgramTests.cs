﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.UnitTesting;

namespace DynamicSample.Tests
{
    /// <summary>
    /// Defines test class DynamicClassTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class DynamicClassTests : ConsoleTestsBase
    {
        private readonly string cExpMain =
            "======================================================================\r\n## Example for dynamic class\r\n=======" +
            "===============================================================\r\n\r\n+-----------------------------------------" +
            "-----------------\r\n| Show Customers\r\n+----------------------------------------------------------\r\nCustomer:" +
            " Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\n" +
            "Customer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------\r\n\r\n+------------------" +
            "----------------------------------------\r\n| Show Customers with Contains\r\n+----------------------------------" +
            "------------------------\r\nList of customers and suppliers  \r\nCustomer: Preston, Chris  \r\nCustomer: Hines, P" +
            "atrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustome" +
            "r: Koch, Paul  \r\n----------------------------\r\n\r\n+---------------------------------------------------------" +
            "-\r\n| Show Suppliers\r\n+----------------------------------------------------------\r\nSupplier: Lucerne Publish" +
            "ing (https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstit" +
            "ute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www." +
            "proseware.com/)   \r\n----------------------------\r\n\r\n+------------------------------------------------------" +
            "----\r\n| Show Supplier with Contains\r\n+----------------------------------------------------------\r\nList of c" +
            "ustomers and suppliers  \r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier: Grap" +
            "hic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrik" +
            "am.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------";
        private readonly string cExpShowCustomerContains =
            "+----------------------------------------------------------\r\n| Show Customers with Contains\r\n+---------------" +
            "-------------------------------------------\r\nList of customers and suppliers  \r\nCustomer: Preston, Chris  \r" +
            "\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, " +
            "Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------";
        private readonly string cExpShowCustomer =
            "+----------------------------------------------------------\r\n| Show Customers\r\n+-----------------------------" +
            "-----------------------------\r\nCustomer: Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, M" +
            "aria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n-------------" +
            "---------------";
        private readonly string cExpShowSupplierContains =
            "+----------------------------------------------------------\r\n| Show Supplier with Contains\r\n+----------------" +
            "------------------------------------------\r\nList of customers and suppliers  \r\nSupplier: Lucerne Publishing (" +
            "https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstitute.c" +
            "om/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.prose" +
            "ware.com/)   \r\n----------------------------";
        private readonly string cExpShowSupplier =
            "+----------------------------------------------------------\r\n| Show Suppliers\r\n+-----------------------------" +
            "-----------------------------\r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier:" +
            " Graphic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.f" +
            "abrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------";

        /// <summary>
        /// Defines the test method MainTest.
        /// </summary>
        [TestMethod()]
        public void MainTest()
        {
            AssertConsoleInOutputArgs(cExpMain,"\r\n",new string[] { },DynamicTestProgram.Main);
        }

        /// <summary>
        /// Defines the test method ShowCustomerContainsTest.
        /// </summary>
        [TestMethod()]
        public void ShowCustomerContainsTest()
        {
            AssertConsoleOutput(cExpShowCustomerContains, DynamicTestProgram.ShowCustomerContains);
        }

        /// <summary>
        /// Defines the test method ShowCustomerTest.
        /// </summary>
        [TestMethod()]
        public void ShowCustomerTest()
        {
            AssertConsoleOutput(cExpShowCustomer, DynamicTestProgram.ShowCustomer);
        }

        /// <summary>
        /// Defines the test method ShowSupplierContainsTest.
        /// </summary>
        [TestMethod()]
        public void ShowSupplierContainsTest()
        {
            AssertConsoleOutput(cExpShowSupplierContains, DynamicTestProgram.ShowSupplierContains);
        }

        /// <summary>
        /// Defines the test method ShowSupplierTest.
        /// </summary>
        [TestMethod()]
        public void ShowSupplierTest()
        {
            AssertConsoleOutput(cExpShowSupplier, DynamicTestProgram.ShowSupplier);
        }
    }
}