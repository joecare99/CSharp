using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatementsNew.Anweisungen
{
    public abstract class Vehicle
    {
        /// <summary>Gets or sets the persons (including the driver).</summary>
        /// <value>
        ///   <para>
        /// The number persons.
        /// </para>
        /// </value>
        public int Persons { get; set; } = 1;
        /// <summary>Gets or sets the capacity of persons (incl. the driver).</summary>
        /// <value>The maximum capacity of all persons.</value>
        public int Capacity { get; set; }
        /// <summary>Gets or sets the fee.
        /// [€]</summary>
        /// <value>The akkumulated fee.</value>
        public decimal fee { get; set; } = 0m;
        /// <summary>Gets or sets the actual weight [t].</summary>
        /// <value>The weight [t].</value>
        public double Weight { get; set; }
        /// <summary>Gets or sets the actual weight [t].</summary>
        /// <value>The weight [t].</value>
        public double WeightClass { get; set; }
        public Vehicle():this(0) { }
        public Vehicle(int passenger) { Capacity = 1; Persons = passenger + 1; }
    }

    public class Car : Vehicle
    {
        public Car(int passenger):base(passenger)
        {
            Capacity = 4;
            Weight = 1.5;
            WeightClass = 2.7;
        }
        public Car() : this(0) { }
    }

    public class Bus : Vehicle
    {
        public Bus(int passenger=5):base(passenger) { Capacity = 52; WeightClass = 27.0; }
    }

    public class Truck : Vehicle
    {
        public Truck() : this(0) { } 
        public Truck(int passenger):base(passenger) { Capacity = 2; WeightClass = 40.0; }
}

    public class DeliveryTruck : Truck
    {
        public DeliveryTruck() : this(0) { }
        public DeliveryTruck(int passenger) : base(passenger) { WeightClass = 7.5; }
    }

    public class LightTruck : Truck
    {
        public LightTruck() : this(0) { }
        public LightTruck(int passenger) : base(passenger) { WeightClass = 27; }
    }

    public class Taxi : Car
    {
        public Taxi(int passenger) : base(passenger) { }
        public Taxi() : this(1) { }

    }

    public class TollCalculator
    {
        public decimal CalculateToll(object vehicle) =>
            vehicle switch
            {
                Taxi { Persons: 1 } t => 3.50m + 0.5m,
                Taxi { Persons: 2 } t => 3.50m,
                Taxi { Persons: 3 } t => 3.50m - 0.5m,
                Taxi  t => 3.50m - 1.0m,
                Car { Persons: 1 } c => 2.00m + 0.5m,
                Car { Persons: 2 } c => 2.00m,
                Car { Persons: 3 } c => 2.00m - 0.5m,
                Car  => 2.00m + 1.0m,
                Bus b when ((double)b.Persons  / (double)b.Capacity) < 0.50 => 5.00m + 2.00m,
                Bus b when ((double)b.Persons / (double)b.Capacity) > 0.90 => 5.00m - 1.00m,
                Bus => 5.00m,
                Truck t when (t.WeightClass > 30) => 10.00m + 5.00m,
                Truck t when (t.WeightClass < 10) => 10.00m - 2.00m,
                Truck => 10.00m,
                { } => throw new ArgumentException(message: "Not a known vehicle type", paramName: nameof(vehicle)),
                null => throw new ArgumentNullException(nameof(vehicle))
            };
    }

    public class SwitchStatement3
    {
        private class Caprio : Car
        {
            public Caprio(int passenger=0):base(passenger) { Capacity = 2; WeightClass = 1.2;Weight = 1.0; }
        }

        public static void ShowTollCollector()
        {
            var tollCalc = new TollCalculator();

            var car = new Car();
            var taxi = new Taxi(1);
            var bus = new Bus();
            var car2 = new Caprio();
            var truck = new Truck();

            var cars = new object[] {
                new Car(),
                taxi,
                new Taxi(2),
                new Car(1),
                taxi,
                car,
                taxi,
                new Caprio(),
                new Bus(15),
                new Truck(1),
                taxi,
                new Car(2),
                car,
                new Truck(0),
                new Truck(1),
                taxi,
                new Taxi(1)
            };

            Console.WriteLine($"The toll for a car is {tollCalc.CalculateToll(car)}");
            Console.WriteLine($"The toll for a taxi is {tollCalc.CalculateToll(taxi)}");
            Console.WriteLine($"The toll for a bus is {tollCalc.CalculateToll(bus)}");
            Console.WriteLine($"The toll for a 2.Car is {tollCalc.CalculateToll(car2)}");
            Console.WriteLine($"The toll for a truck is {tollCalc.CalculateToll(truck)}");

            try
            {
                tollCalc.CalculateToll("this will fail");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Caught an argument exception when using the wrong type");
            }
            try
            {
                tollCalc.CalculateToll(null!);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Caught an argument exception when using null");
            }

            var fee = 0m;
            var feeSum = 0m;
            var WeightSum = 0.0;
            var Persons = 0;
            foreach (var c in cars)
            {
                feeSum += fee = tollCalc.CalculateToll(c);
                if (c is Vehicle v)
                {
                    WeightSum += v.Weight;
                    Persons += v.Persons;
                    v.fee += fee;
                    Console.Write($"{c.GetType().Name} \twith {v.Persons}P. and a weight of {v.Weight}t\tCost: {tollCalc.CalculateToll(c)}$");
                }
                Console.WriteLine($"\tTotals:  toll: {feeSum}$ \t Persons:{Persons}\t Weight:{WeightSum:0.00}t");

            }
        }
    }
}
