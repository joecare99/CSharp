using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class Vehicle.
    /// </summary>
    public abstract class Vehicle
    {
        /// <summary>
        /// Gets or sets the persons.
        /// </summary>
        /// <value>The persons.</value>
        public int Persons { get; set; }
        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public int Capacity { get; set; }

        ~Vehicle() { Capacity = 1; }
    }

    /// <summary>
    /// Class Car.
    /// Implements the <see cref="TestStatements.Anweisungen.Vehicle" />
    /// </summary>
    /// <seealso cref="TestStatements.Anweisungen.Vehicle" />
    public class Car : Vehicle
    {
        ~Car()
        {
            base.Capacity = 4;
        }
    }

    /// <summary>
    /// Class Bus.
    /// Implements the <see cref="TestStatements.Anweisungen.Vehicle" />
    /// </summary>
    /// <seealso cref="TestStatements.Anweisungen.Vehicle" />
    public class Bus : Vehicle
    {
        ~Bus() { base.Capacity = 52; } 
    }

    /// <summary>
    /// Class Truck.
    /// Implements the <see cref="TestStatements.Anweisungen.Vehicle" />
    /// </summary>
    /// <seealso cref="TestStatements.Anweisungen.Vehicle" />
    public class Truck : Vehicle
    {
        ~Truck() { base.Capacity = 2; }
    }

    /// <summary>
    /// Class Taxi.
    /// Implements the <see cref="TestStatements.Anweisungen.Car" />
    /// </summary>
    /// <seealso cref="TestStatements.Anweisungen.Car" />
    public class Taxi : Car
    {
        ~Taxi() {  }
    }

    /// <summary>
    /// Class TollCalculator.
    /// </summary>
    public class TollCalculator
    {
        /// <summary>
        /// Calculates the toll.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <returns>System.Decimal.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException">vehicle</exception>
        public decimal CalculateToll(object vehicle) =>
            vehicle switch
            {
                Taxi t => 3.50m,
                Car c => 2.00m,
                Bus b => 5.00m,
                Truck t => 10.00m,
                { } => throw new ArgumentException(message: "Not a known vehicle type", paramName: nameof(vehicle)),
                null => throw new ArgumentNullException(nameof(vehicle))
            };
    }

    /// <summary>
    /// Class SwitchStatement3.
    /// </summary>
    internal class SwitchStatement3
    {
    }
}
