// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 07-21-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Interface.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.ClassesAndObjects {
    /// <summary>
    /// Interface IInterface1
    /// </summary>
    public interface IInterface1
    {
        /// <summary>
        /// Quacks this instance.
        /// </summary>
        void Quack();
    }

    /// <summary>
    /// Interface IInterface2
    /// </summary>
    public interface IInterface2 {
        /// <summary>
        /// Moves this instance.
        /// </summary>
        void Move();
	}

    /// <summary>
    /// Interface IInterface3
    /// Extends the <see cref="TestStatements.ClassesAndObjects.IInterface2" />
    /// </summary>
    /// <seealso cref="TestStatements.ClassesAndObjects.IInterface2" />
    public interface IInterface3 : IInterface2
	{
        /// <summary>
        /// Rolls this instance.
        /// </summary>
        void Roll();
	}

    /// <summary>
    /// Class Class1.
    /// </summary>
    public class Class1
    {
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "I am Class1";
        }
    }

    /// <summary>
    /// Class Class2.
    /// Implements the <see cref="TestStatements.ClassesAndObjects.Class1" />
    /// Implements the <see cref="TestStatements.ClassesAndObjects.IInterface1" />
    /// </summary>
    /// <seealso cref="TestStatements.ClassesAndObjects.Class1" />
    /// <seealso cref="TestStatements.ClassesAndObjects.IInterface1" />
    public class Class2: Class1 , IInterface1
    {
        /// <summary>
        /// Quacks this instance.
        /// </summary>
        public void Quack()
        {
            Console.WriteLine("Quack !");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "I am Class2";
        }
    }

    /// <summary>
    /// Class Class3.
    /// Implements the <see cref="TestStatements.ClassesAndObjects.Class1" />
    /// Implements the <see cref="TestStatements.ClassesAndObjects.IInterface2" />
    /// </summary>
    /// <seealso cref="TestStatements.ClassesAndObjects.Class1" />
    /// <seealso cref="TestStatements.ClassesAndObjects.IInterface2" />
    public class Class3 : Class1, IInterface2
    {
        /// <summary>
        /// Moves this instance.
        /// </summary>
        public void Move()
        {
            Console.WriteLine("Moving ...");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "I am Class3";
        }
    }

    /// <summary>
    /// Class Class4.
    /// Implements the <see cref="TestStatements.ClassesAndObjects.Class1" />
    /// Implements the <see cref="TestStatements.ClassesAndObjects.IInterface3" />
    /// </summary>
    /// <seealso cref="TestStatements.ClassesAndObjects.Class1" />
    /// <seealso cref="TestStatements.ClassesAndObjects.IInterface3" />
    public class Class4 : Class1, IInterface3
    {
        /// <summary>
        /// Moves this instance.
        /// </summary>
        public void Move()
        {
            Console.WriteLine("Moving ...");
        }

        /// <summary>
        /// Rolls this instance.
        /// </summary>
        public void Roll()
        {
            Console.WriteLine("Rolling ...");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "I am Class4";
        }
    }

    /// <summary>
    /// Class Class5.
    /// Implements the <see cref="TestStatements.ClassesAndObjects.Class4" />
    /// Implements the <see cref="TestStatements.ClassesAndObjects.IInterface1" />
    /// </summary>
    /// <seealso cref="TestStatements.ClassesAndObjects.Class4" />
    /// <seealso cref="TestStatements.ClassesAndObjects.IInterface1" />
    public class Class5 : Class4, IInterface1
    {
        /// <summary>
        /// Quacks this instance.
        /// </summary>
        public void Quack()
        {
            Console.WriteLine("Quack !");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "I am Class4";
        }
    }

    /// <summary>
    /// Class InterfaceTest.
    /// </summary>
    public static class InterfaceTest {
        /// <summary>
        /// The classes
        /// </summary>
        static Class1[] classes = new Class1[] { new Class1(), new Class2(), new Class3(), new Class4(), new Class5() };

        /// <summary>
        /// Runs this instance.
        /// </summary>
        static public void Run()
        {
        foreach (var c in classes)
            {
                Console.WriteLine(c.ToString());
                if (c is IInterface1 i)
                {
                    Console.WriteLine("It can Quack ...");
                    i.Quack();
                }
                else
                    Console.WriteLine("not able to Quack ...");
                if (c is IInterface2 i2)
                {
                    Console.WriteLine("It can move ...");
                    i2.Move();
                }
                else Console.WriteLine("Not able to move ...");
                if (c is IInterface3 i3)
                {
                    Console.WriteLine("It can roll ...");
                    i3.Roll();
                }
                else
                    Console.WriteLine("Not able to roll ...");
                Console.WriteLine("=============================");
            }
        }

	}
}
