using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class SwitchStatement3Tests : ConsoleTestsBase
    {
        [TestMethod()]
        public void ShowTollCollectorTest()
        {
            var tc = new SwitchStatement3();
            AssertConsoleOutput(@"The toll for a car is 2,00
The toll for a taxi is 3,50
The toll for a bus is 5,00
The toll for a 2.Car is 2,00
The toll for a truck is 10,00
Caught an argument exception when using the wrong type
Caught an argument exception when using null
Car 	with 1P. and a weight of TestStatements.Anweisungen.Cart	Cost: 2,00$	Totals:  toll: 2,00$ 	 Persons:1	 Weight:0,00t
Taxi 	with 2P. and a weight of TestStatements.Anweisungen.Taxit	Cost: 3,50$	Totals:  toll: 5,50$ 	 Persons:3	 Weight:0,00t
Taxi 	with 3P. and a weight of TestStatements.Anweisungen.Taxit	Cost: 3,50$	Totals:  toll: 9,00$ 	 Persons:6	 Weight:0,00t
Car 	with 2P. and a weight of TestStatements.Anweisungen.Cart	Cost: 2,00$	Totals:  toll: 11,00$ 	 Persons:8	 Weight:0,00t
Taxi 	with 2P. and a weight of TestStatements.Anweisungen.Taxit	Cost: 3,50$	Totals:  toll: 14,50$ 	 Persons:10	 Weight:0,00t
Car 	with 1P. and a weight of TestStatements.Anweisungen.Cart	Cost: 2,00$	Totals:  toll: 16,50$ 	 Persons:11	 Weight:0,00t
Taxi 	with 2P. and a weight of TestStatements.Anweisungen.Taxit	Cost: 3,50$	Totals:  toll: 20,00$ 	 Persons:13	 Weight:0,00t
Caprio 	with 1P. and a weight of TestStatements.Anweisungen.SwitchStatement3+Capriot	Cost: 2,00$	Totals:  toll: 22,00$ 	 Persons:14	 Weight:0,00t
Bus 	with 16P. and a weight of TestStatements.Anweisungen.Bust	Cost: 5,00$	Totals:  toll: 27,00$ 	 Persons:30	 Weight:0,00t
Truck 	with 2P. and a weight of TestStatements.Anweisungen.Truckt	Cost: 10,00$	Totals:  toll: 37,00$ 	 Persons:32	 Weight:0,00t
Taxi 	with 2P. and a weight of TestStatements.Anweisungen.Taxit	Cost: 3,50$	Totals:  toll: 40,50$ 	 Persons:34	 Weight:0,00t
Car 	with 3P. and a weight of TestStatements.Anweisungen.Cart	Cost: 2,00$	Totals:  toll: 42,50$ 	 Persons:37	 Weight:0,00t
Car 	with 1P. and a weight of TestStatements.Anweisungen.Cart	Cost: 2,00$	Totals:  toll: 44,50$ 	 Persons:38	 Weight:0,00t
Truck 	with 1P. and a weight of TestStatements.Anweisungen.Truckt	Cost: 10,00$	Totals:  toll: 54,50$ 	 Persons:39	 Weight:0,00t
Truck 	with 2P. and a weight of TestStatements.Anweisungen.Truckt	Cost: 10,00$	Totals:  toll: 64,50$ 	 Persons:41	 Weight:0,00t
Taxi 	with 2P. and a weight of TestStatements.Anweisungen.Taxit	Cost: 3,50$	Totals:  toll: 68,00$ 	 Persons:43	 Weight:0,00t
Taxi 	with 2P. and a weight of TestStatements.Anweisungen.Taxit	Cost: 3,50$	Totals:  toll: 71,50$ 	 Persons:45	 Weight:0,00t",
    SwitchStatement3.ShowTollCollector);
        }


    }
}