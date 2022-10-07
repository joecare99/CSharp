// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Locking.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    /// <summary>
    /// Class Account.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// The balance
        /// </summary>
        private decimal balance;
        /// <summary>
        /// Gets the balance.
        /// </summary>
        /// <value>The balance.</value>
        public decimal Balance{ get =>balance; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Account" /> class.
        /// </summary>
        public Account()
        {
            balance = 0;
        }

        /// <summary>
        /// The synchronize
        /// </summary>
        private readonly object sync = new object();
        /// <summary>
        /// Withdraws the specified amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <exception cref="System.Exception">Insufficient funds</exception>
        public void Withdraw(decimal amount)
        {
            lock (sync)
            {
                if (amount > balance)
                {
                    throw new Exception(
                        "Insufficient funds");
                }
                balance -= amount;
            }
        }
    }

    /// <summary>
    /// Test the lock statement
    /// </summary>
    public class Locking
    {
        /// <summary>
        /// Does the lock test.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void DoLockTest(string[] args)
        {
            Account a;
            a = new Account();
            a.Withdraw(-50); 
            a.Withdraw(20);
            try
            {
                a.Withdraw(31);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
