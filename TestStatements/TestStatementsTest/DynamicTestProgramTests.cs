using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.ConsoleAsserts;

namespace DynamicSample.Tests
{
    [TestClass()]
    public class DynamicTestProgramTests : TestConsole
    {
        private readonly string cExpMain =
            "======================================================================\r\n## Example for dynamic class\r\n======================================================================\r\n\r\n+----------------------------------------------------------\r\n| Show Customers\r\n+----------------------------------------------------------\r\nCustomer: Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------\r\n\r\n+----------------------------------------------------------\r\n| Show Customers with Contains\r\n+----------------------------------------------------------\r\nList of customers and suppliers  \r\nCustomer: Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------\r\n\r\n+----------------------------------------------------------\r\n| Show Suppliers\r\n+----------------------------------------------------------\r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------\r\n\r\n+----------------------------------------------------------\r\n| Show Supplier with Contains\r\n+----------------------------------------------------------\r\nList of customers and suppliers  \r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------";
        private readonly string cExpShowCustomerContains =
            "+----------------------------------------------------------\r\n| Show Customers with Contains\r\n+----------------------------------------------------------\r\nList of customers and suppliers  \r\nCustomer: Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------";
        private readonly string cExpShowCustomer =
            "+----------------------------------------------------------\r\n| Show Customers\r\n+----------------------------------------------------------\r\nCustomer: Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------";
        private readonly string cExpShowSupplierContains =
            "+----------------------------------------------------------\r\n| Show Supplier with Contains\r\n+----------------------------------------------------------\r\nList of customers and suppliers  \r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------";
        private readonly string cExpShowSupplier =
            "+----------------------------------------------------------\r\n| Show Suppliers\r\n+----------------------------------------------------------\r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------";

        [TestMethod()]
        public void MainTest()
        {
            AssertConsoleInOutputArgs(cExpMain,"\r\n",new string[] { }, DynamicTestProgram.Main);
        }

        [TestMethod()]
        public void ShowCustomerContainsTest()
        {
            AssertConsoleOutput(cExpShowCustomerContains, DynamicTestProgram.ShowCustomerContains);
        }

        [TestMethod()]
        public void ShowCustomerTest()
        {
            AssertConsoleOutput(cExpShowCustomer, DynamicTestProgram.ShowCustomer);
        }

        [TestMethod()]
        public void ShowSupplierContainsTest()
        {
            AssertConsoleOutput(cExpShowSupplierContains, DynamicTestProgram.ShowSupplierContains);
        }

        [TestMethod()]
        public void ShowSupplierTest()
        {
            AssertConsoleOutput(cExpShowSupplier, DynamicTestProgram.ShowSupplier);
        }
    }
}