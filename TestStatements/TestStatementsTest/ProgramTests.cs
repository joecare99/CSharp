using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private readonly string cExpMain = @"======================================================================
## Example for dynamic class
======================================================================

+----------------------------------------------------------
| Show Customers
+----------------------------------------------------------
Customer: Preston, Chris  
Customer: Hines, Patrick  
Customer: Cameron, Maria  
Customer: Seubert, Roxanne  
Customer: Adolphi, Stephan  
Customer: Koch, Paul  
----------------------------

+----------------------------------------------------------
| Show Customers with Contains
+----------------------------------------------------------
List of customers and suppliers  
Customer: Preston, Chris  
Customer: Hines, Patrick  
Customer: Cameron, Maria  
Customer: Seubert, Roxanne  
Customer: Adolphi, Stephan  
Customer: Koch, Paul  
----------------------------

+----------------------------------------------------------
| Show Suppliers
+----------------------------------------------------------
Supplier: Lucerne Publishing (https://www.lucernepublishing.com/)  
Supplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   
Supplier: Fabrikam, Inc. (https://www.fabrikam.com/)   
Supplier: Proseware, Inc. (http://www.proseware.com/)   
----------------------------

+----------------------------------------------------------
| Show Supplier with Contains
+----------------------------------------------------------
List of customers and suppliers  
Supplier: Lucerne Publishing (https://www.lucernepublishing.com/)  
Supplier: Graphic Design Institute (https://www.graphicdesigninstitute.com/)   
Supplier: Fabrikam, Inc. (https://www.fabrikam.com/)   
Supplier: Proseware, Inc. (http://www.proseware.com/)   
----------------------------
======================================================================
## 2. Example for dynamic class
======================================================================
strName
strStreet
strCity
strPLZ
strCountry
----------------------------
P. Mustermann
Somestreet 35
12345 Somplace
Somewhere
----------------------------
Name:	P. Mustermann
Street:	Somestreet 35
PLZ:	12345
City:	Somplace
Country:	Somewhere
----------------------------
P. Musterfrau
Someotherstreet 1
54321 Anyplace
Nowhere
----------------------------
Name:	P. Musterfrau
Street:	Someotherstreet 1
PLZ:	54321
City:	Anyplace
Country:	Nowhere
----------------------------
======================================================================
## 2b. Example for static class
======================================================================
P. Mustermann
Somestreet 35
12345 Somplace
Somewhere
----------------------------
Name:	P. Mustermann
Street:	Somestreet 35
PLZ:	12345
City:	Somplace
Country:	Somewhere
----------------------------
P. Musterfrau
Someotherstreet 1
54321 Anyplace
Nowhere
----------------------------
Name:	P. Musterfrau
Street:	Someotherstreet 1
PLZ:	54321
City:	Anyplace
Country:	Nowhere
----------------------------";
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