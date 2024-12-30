using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace DynamicSample.Model.Tests
{
    /// <summary>
    /// Defines test class DynamicTestProgramTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class DynamicTestProgramTests : ConsoleTestsBase
    {
        private readonly string cExpMain =
			"======================================================================\r\n## Example for dynamic class\r\n======================================================================\r\n\r\n+----------------------------------------------------------\r\n| Show Customers\r\n+----------------------------------------------------------\r\nCustomer: Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------\r\n\r\n+----------------------------------------------------------\r\n| Show Customers with Contains\r\n+----------------------------------------------------------\r\nList of customers and suppliers  \r\nCustomer: Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------\r\n\r\n+----------------------------------------------------------\r\n| Show Suppliers\r\n+----------------------------------------------------------\r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------\r\n\r\n+----------------------------------------------------------\r\n| Show Supplier with Contains\r\n+----------------------------------------------------------\r\nList of customers and suppliers  \r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------\r\n======================================================================\r\n## 2. Example for dynamic class\r\n======================================================================\r\nstrName\r\nstrStreet\r\nstrCity\r\nstrPLZ\r\nstrCountry\r\n----------------------------\r\nP. Mustermann\r\nSomestreet 35\r\n12345 Somplace\r\nSomewhere\r\n----------------------------\r\nName:\tP. Mustermann\r\nStreet:\tSomestreet 35\r\nPLZ:\t12345\r\nCity:\tSomplace\r\nCountry:\tSomewhere\r\n----------------------------\r\nP. Musterfrau\r\nSomeotherstreet 1\r\n54321 Anyplace\r\nNowhere\r\n----------------------------\r\nName:\tP. Musterfrau\r\nStreet:\tSomeotherstreet 1\r\nPLZ:\t54321\r\nCity:\tAnyplace\r\nCountry:\tNowhere\r\n----------------------------\r\n======================================================================\r\n## 2b. Example for static class\r\n======================================================================\r\nP. Mustermann\r\nSomestreet 35\r\n12345 Somplace\r\nSomewhere\r\n----------------------------\r\nName:\tP. Mustermann\r\nStreet:\tSomestreet 35\r\nPLZ:\t12345\r\nCity:\tSomplace\r\nCountry:\tSomewhere\r\n----------------------------\r\nP. Musterfrau\r\nSomeotherstreet 1\r\n54321 Anyplace\r\nNowhere\r\n----------------------------\r\nName:\tP. Musterfrau\r\nStreet:\tSomeotherstreet 1\r\nPLZ:\t54321\r\nCity:\tAnyplace\r\nCountry:\tNowhere\r\n----------------------------";
        private readonly string cExpShowCustomerContains =
            "+----------------------------------------------------------\r\n| Show Customers with Contains\r\n+----------------------------------------------------------\r\nList of customers and suppliers  \r\nCustomer: Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------";
        private readonly string cExpShowCustomer =
            "+----------------------------------------------------------\r\n| Show Customers\r\n+----------------------------------------------------------\r\nCustomer: Preston, Chris  \r\nCustomer: Hines, Patrick  \r\nCustomer: Cameron, Maria  \r\nCustomer: Seubert, Roxanne  \r\nCustomer: Adolphi, Stephan  \r\nCustomer: Koch, Paul  \r\n----------------------------";
        private readonly string cExpShowSupplierContains =
            "+----------------------------------------------------------\r\n| Show Supplier with Contains\r\n+----------------------------------------------------------\r\nList of customers and suppliers  \r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------";
        private readonly string cExpShowSupplier =
            "+----------------------------------------------------------\r\n| Show Suppliers\r\n+----------------------------------------------------------\r\nSupplier: Lucerne Publishing (https://www.lucernepublishing.com/)  \r\nSupplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   \r\nSupplier: Fabrikam, Inc. (https://www.fabrikam.com/)   \r\nSupplier: Proseware, Inc. (http://www.proseware.com/)   \r\n----------------------------";

        /// <summary>
        /// Defines the test method MainTest.
        /// </summary>
        [TestMethod()]
        public void MainTest()
        {
            AssertConsoleInOutputArgs(cExpMain,"\r\n",new string[] { }, DynamicTestProgram.Main);
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
