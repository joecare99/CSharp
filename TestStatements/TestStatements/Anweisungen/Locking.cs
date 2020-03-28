using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen
{
    public class Account
    {
        private decimal balance;
        public decimal Balance{ get =>balance; }

        public Account()
        {
            balance = 0;
        }

        private readonly object sync = new object();
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

    /// <summary>Test the lock statement</summary>
    public class Locking
    {
        /// <summary>Does the lock test.</summary>
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
