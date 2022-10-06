using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Anweisungen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class RandomExampleTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class RandomExampleTests : ConsoleTestsBase 
		{
        private readonly string cExpExampleMain1 =
            "Five random byte values:\r\n   57  151  211   68   86\r\nFive random integer values:\r\n  1.295.429.840  1.781.364.242  1.186.665.210    307.042.705  1.347.179.935\r\nFive random integers between 0 and 100:\r\n      76      77      17      54      87\r\nFive random integers between 50 and 100:\r\n      60      61      77      72      96\r\nFive Doubles.\r\n   0,291   0,297   0,562   0,904   0,170\r\nFive Doubles between 0 and 5.\r\n   3,944   1,333   0,320   3,720   1,907";
        private readonly string cExpExampleMain2 =
            "Suggested pet name of the day: \r\n   For a male:     Rufus\r\n   For a female:   Princess";
        private readonly string cExpExampleMain3 =
            "First Series:\r\n   75   19   35  150   93   62  127   65   69  216\r\n  156  124   94   35  255   63  170  187  142  102\r\n   56   70   69  155  209  240  127  109  125  235\r\n   50  234   10   65  149  137  198  150  138   24\r\n   86  133  185   86  245  189   22  167   94    5\r\n   92  212    7  174  235    4  205  136  197  108\r\n  190   17  196   89  165  178  113   29  141  117\r\n  121   19   48  118   15  179  141  239  166   19\r\n  217  216   15  119  143   94  227   92   85  144\r\n  188   62  208   29   89   69  192   96  177   66\r\n\r\nSecond Series:\r\n   75   19   35  150   93   62  127   65   69  216\r\n  156  124   94   35  255   63  170  187  142  102\r\n   56   70   69  155  209  240  127  109  125  235\r\n   50  234   10   65  149  137  198  150  138   24\r\n   86  133  185   86  245  189   22  167   94    5\r\n   92  212    7  174  235    4  205  136  197  108\r\n  190   17  196   89  165  178  113   29  141  117\r\n  121   19   48  118   15  179  141  239  166   19\r\n  217  216   15  119  143   94  227   92   85  144\r\n  188   62  208   29   89   69  192   96  177   66";

        /// <summary>
        /// Defines the test method ExampleMain1Test.
        /// </summary>
        [TestMethod()]
        public void ExampleMain1Test()
        {
            RandomExample.rand = new Random(100101);
            AssertConsoleOutput(cExpExampleMain1, RandomExample.ExampleMain1);
        }

        /// <summary>
        /// Defines the test method ExampleMain2Test.
        /// </summary>
        [TestMethod()]
        public void ExampleMain2Test()
        {
            RandomExample.rand = new Random(100101);
            AssertConsoleOutput(cExpExampleMain2, RandomExample.ExampleMain2);
        }

        /// <summary>
        /// Defines the test method ExampleMain3Test.
        /// </summary>
        [TestMethod()]
        public void ExampleMain3Test()
        {
            RandomExample.rand = new Random(100101);
            AssertConsoleOutput(cExpExampleMain3, RandomExample.ExampleMain3);
        }
    }
}
