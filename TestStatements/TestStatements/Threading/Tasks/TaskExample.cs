// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="TaskExample.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace TestStatements.Threading.Tasks
{
    /// <summary>
    /// Class TaskExample.
    /// </summary>
    public static class TaskExample
    {
        /// <summary>
        /// The urls
        /// </summary>
        private static String[] urls = {
                "www.adatum.com",
                "www.cohovineyard.com",
                "www.cohowinery.com",
                "www.jc99.de",
                "www.northwindtraders.com",
                "www.contoso.com"
            };

        /// <summary>
        /// The random
        /// </summary>
        public static Random random = new Random();

        /// <summary>
        /// Examples the main.
        /// </summary>
        public static void ExampleMain()
        {
            ExampleMain1();
            ExampleMain2();
            ExampleMain3();
            ExampleMain4();
        }
        /// <summary>
        /// Examples the main1.
        /// </summary>
        public static void ExampleMain1()
        {
            int failed = 0;
            var tasks = new List<Task>();

            Console.WriteLine("Sending Pings");

            foreach (var value in urls)
            {
                var url = value;
                tasks.Add(Task.Run(() => {
                    var png = new Ping();
                    try
                    {
                        var reply = png.Send(url);
                        if (!(reply.Status == IPStatus.Success))
                        {
                            Interlocked.Increment(ref failed);
                            throw new TimeoutException("Unable to reach " + url + ".");
                        }
                    }
                    catch (PingException)
                    {
                        Interlocked.Increment(ref failed);
                        throw;
                    }
                }));
            }
            Task t = Task.WhenAll(tasks);
            try
            {
                t.Wait();
            }
            catch { }

            if (t.Status == TaskStatus.RanToCompletion)
                Console.WriteLine("All ping attempts succeeded.");
            else if (t.Status == TaskStatus.Faulted)
                Console.WriteLine("{0} ping attempts failed", failed);
        }

        /// <summary>
        /// Examples the main2.
        /// </summary>
        public static void ExampleMain2()
        {
            Task t = ExampleMain2a();
            t.Wait();
        }
        /// <summary>
        /// Examples the main2a.
        /// </summary>
        private static async Task ExampleMain2a()
        {
            int failed = 0;
            var tasks = new List<Task>();

            Console.WriteLine("Sending Pings");

            foreach (var value in urls)
            {
                var url = value;
                tasks.Add(Task.Run(() => {
                    var png = new Ping();
                    try
                    {
                        var reply = png.Send(url);
                        if (!(reply.Status == IPStatus.Success))
                        {
                            Interlocked.Increment(ref failed);
                            throw new TimeoutException("Unable to reach " + url + ".");
                        }
                    }
                    catch (PingException)
                    {
                        Interlocked.Increment(ref failed);
                        throw;
                    }
                }));
            }
            Task t = Task.WhenAll(tasks.ToArray());
            try
            {
                await t;
            }
            catch { }

            if (t.Status == TaskStatus.RanToCompletion)
                Console.WriteLine("All ping attempts succeeded.");
            else if (t.Status == TaskStatus.Faulted)
                Console.WriteLine("{0} ping attempts failed", failed);
        }

        /// <summary>
        /// Examples the main3.
        /// </summary>
        public static void ExampleMain3()
        {
            var tasks = new List<Task<long>>();

            Console.WriteLine("Generate Numbers");

            for (int ctr = 1; ctr <= 10; ctr++)
            {
                int delayInterval = 18 * ctr;
                tasks.Add(Task.Run(async () => {
                    long total = 0;
                    await Task.Delay(delayInterval);
                    var rnd = random;
                    // Generate 1,000 random numbers.
                    for (int n = 1; n <= 1000; n++)
                        total += rnd.Next(0, 1000);
                    return total;
                }));
            }
            var continuation = Task.WhenAll(tasks);
            try
            {
                continuation.Wait();
            }
            catch (AggregateException)
            { }

            if (continuation.Status == TaskStatus.RanToCompletion)
            {
                long grandTotal = 0;
                foreach (var result in continuation.Result)
                {
                    grandTotal += result;
                    Console.WriteLine("Mean: {0:N2}, n = 1,000", result / 1000.0);
                }

                Console.WriteLine("\nMean of Means: {0:N2}, n = 10,000",
                                  grandTotal / 10000);
            }
            // Display information on faulted tasks.
            else
            {
                foreach (var t in tasks)
                {
                    Console.WriteLine("Task {0}: {1}", t.Id, t.Status);
                }
            }
        }

        /// <summary>
        /// Examples the main4.
        /// </summary>
        public static void ExampleMain4()
        {
            var tasks = new Task<long>[10];

            Console.WriteLine("Generate Numbers");

            for (int ctr = 1; ctr <= 10; ctr++)
            {
                int delayInterval = 18 * ctr;
                tasks[ctr - 1] = Task.Run(async () => {
                    long total = 0;
                    await Task.Delay(delayInterval);
                    var rnd = random;
                    // Generate 1,000 random numbers.
                    for (int n = 1; n <= 1000; n++)
                        total += rnd.Next(0, 1000);

                    return total;
                });
            }
            var continuation = Task.WhenAll(tasks);
            try
            {
                continuation.Wait();
            }
            catch (AggregateException)
            { }

            if (continuation.Status == TaskStatus.RanToCompletion)
            {
                long grandTotal = 0;
                foreach (var result in continuation.Result)
                {
                    grandTotal += result;
                    Console.WriteLine("Mean: {0:N2}, n = 1,000", result / 1000.0);
                }

                Console.WriteLine("\nMean of Means: {0:N2}, n = 10,000",
                                  grandTotal / 10000);
            }
            // Display information on faulted tasks.
            else
            {
                foreach (var t in tasks)
                    Console.WriteLine("Task {0}: {1}", t.Id, t.Status);
            }
        }
    }
    // The example displays output like the following:
    //       5 ping attempts failed
}
