﻿// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-20-2022
// ***********************************************************************
// <copyright file="StopWatchExample.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TestStatements.Diagnostics
{
    /// <summary>
    /// Class StopWatchExample.
    /// </summary>
    public static class StopWatchExample
    {
        public static Func<IStopwatch> GetStopwatch { get; set; }= () => new StopwatchProxy();
        public static Func<IStopwatch> GetStopwatch2 { get; set; }= () => new StopwatchProxy();
        public static Action<int> ThreadSleep { get; set; }=(i) => Thread.Sleep(i);

        /// <summary>
        /// Examples the main.
        /// </summary>
        public static void ExampleMain()
        {
            ExampleMain1();
            ExampleMain2();
        }
        /// <summary>
        /// Examples the main1.
        /// </summary>
        public static void ExampleMain1()
        {
            Console.WriteLine("This will take aprox. 10s");
            var stopWatch = GetStopwatch();
            stopWatch.Start();
            ThreadSleep(10000);
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        /// <summary>
        /// Examples the main2.
        /// </summary>
        public static void ExampleMain2()
        {
            DisplayTimerProperties();

            Console.WriteLine("This will take some time");
            Console.WriteLine();
            Console.WriteLine("Press the Enter key to begin:");
            Console.ReadLine();
            Console.WriteLine();

            TimeOperations();
        }

        /// <summary>
        /// Displays the timer properties.
        /// </summary>
        public static void DisplayTimerProperties()
        {
            var _dic = new Dictionary<bool, string>()
            {
                { true,"Operations timed using the system's high-resolution performance counter." },
                { false, "Operations timed using the DateTime class." }
            };

            // Display the timer frequency and resolution.
            Console.WriteLine(_dic[Stopwatch.IsHighResolution]);

            long frequency = Stopwatch.Frequency;
            Console.WriteLine("  Timer frequency in ticks per second = {0}",
                frequency);
            long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
            Console.WriteLine("  Timer is accurate within {0} nanoseconds",
                nanosecPerTick);
        }

        /// <summary>
        /// Times the operations.
        /// </summary>
        public static void TimeOperations()
        {
            long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
            const long numIterations = 1000;

            // Define the operation title names.
            String[] operationNames = {"Operation: Int32.Parse(\"0\")",
                                           "Operation: Int32.TryParse(\"0\")",
                                           "Operation: Int32.Parse(\"a\")",
                                           "Operation: Int32.TryParse(\"a\")"};

            // Time four different implementations for parsing 
            // an integer from a string. 

            for (int operation = 0; operation <= 3; operation++)
            {
                // Define variables for operation statistics.
                long numTicks = 0;
                long numRollovers = 0;
                long maxTicks = 0;
                long minTicks = Int64.MaxValue;
                int indexFastest = -1;
                int indexSlowest = -1;
                long milliSec = 0;

                var time10kOperations = GetStopwatch2();
                time10kOperations.Start();

                // Run the current operation 10001 times.
                // The first execution time will be tossed
                // out, since it can skew the average time.

                for (int i = 0; i <= numIterations; i++)
                {
                    long ticksThisTime = 0;
                    int inputNum;
                    IStopwatch timePerParse = GetStopwatch();

                    Func<(bool ok, int)> f = (operation) switch {
                        0 => () => (true, Int32.Parse("0")),
                        1 => () => (Int32.TryParse("0", out int i), i),
                        2 => () => (true, Int32.Parse("a")),
                        _ => () => (Int32.TryParse("a", out int i), i)
                    };

                    bool ok;
                    switch (operation)
                    {
                        case 0:
                        case 2:
                            // Parse a valid integer using
                            // a try-catch statement.

                            // Start a new stopwatch timer.
                            timePerParse.Reset();
                            timePerParse.Start();

                            try
                            {
                                (_,inputNum) = f();
                            }
                            catch (FormatException)
                            {
                                inputNum = 0;
                            }

                            // Stop the timer, and save the
                            // elapsed ticks for the operation.

                            timePerParse.Stop();
                            ticksThisTime = timePerParse.ElapsedTicks;
                            break;
                        case 1:
                        case 3:
                            // Parse a valid integer using
                            // the TryParse statement.

                            // Start a new stopwatch timer.
                            timePerParse.Reset();
                            timePerParse.Start();

                            (ok, inputNum) = f();
                            if (ok)
                            {
                                inputNum = 0;
                            }

                            // Stop the timer, and save the
                            // elapsed ticks for the operation.
                            timePerParse.Stop();
                            ticksThisTime = timePerParse.ElapsedTicks;
                            break;
                       
                   /*     default:
                            break;*/
                    }

                    // Skip over the time for the first operation,
                    // just in case it caused a one-time
                    // performance hit.
                    if (i == 0)
                    {
                        time10kOperations.Reset();
                        time10kOperations.Start();
                    }
                    else
                    {

                        // Update operation statistics
                        // for iterations 1-10000.
                        if (maxTicks < ticksThisTime)
                        {
                            indexSlowest = i;
                            maxTicks = ticksThisTime;
                        }
                        if (minTicks > ticksThisTime)
                        {
                            indexFastest = i;
                            minTicks = ticksThisTime;
                        }
                        numTicks += ticksThisTime;
                        if (numTicks < ticksThisTime)
                        {
                            // Keep track of rollovers.
                            numRollovers++;
                        }
                    }
                }

                // Display the statistics for 10000 iterations.

                time10kOperations.Stop();
                milliSec = time10kOperations.ElapsedMilliseconds;

                Console.WriteLine();
                Console.WriteLine("{0} Summary:", operationNames[operation]);
                Console.WriteLine("  Slowest time:  #{0}/{1} = {2} ticks",
                    indexSlowest, numIterations, maxTicks);
                Console.WriteLine("  Fastest time:  #{0}/{1} = {2} ticks",
                    indexFastest, numIterations, minTicks);
                Console.WriteLine("  Average time:  {0} ticks = {1} nanoseconds",
                    numTicks / numIterations,
                    (numTicks * nanosecPerTick) / numIterations);
                Console.WriteLine("  Total time looping through {0} operations: {1} milliseconds",
                    numIterations, milliSec);
            }
        }
    }
}
