// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="AsyncBreakFast1.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Threading.Tasks
{
    /// <summary>
    /// Class AsyncBreakfast.
    /// </summary>
    public class AsyncBreakfast
    {
        /// <summary>
        /// Asynchronouses the breakfast main.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void AsyncBreakfast_Main(string[] args)
        {
            var s = new Stopwatch();
            const string Title = "AsyncBreakfast 1.st";
            Console.WriteLine(Constants.Constants.Header, Title);

            s.Start();
            Coffee cup = PourCoffee();
            Console.WriteLine($"{s.Elapsed}: coffee is ready");

            Egg eggs = FryEggs(2);
            Console.WriteLine($"{s.Elapsed}: eggs are ready");

            Bacon bacon = FryBacon(3);
            Console.WriteLine($"{s.Elapsed}: bacon is ready");

            Toast toast = ToastBread(2);
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine($"{s.Elapsed}: toast is ready");

            Juice oj = PourOJ();
            Console.WriteLine($"{s.Elapsed}: oj is ready");
            Console.WriteLine($"{s.Elapsed}: Breakfast is ready!");
            s.Stop();
            Console.WriteLine($"{s.Elapsed}: Breakfast is ready!");
        }

        /// <summary>
        /// Asynchronouses the breakfast main2.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static async Task AsyncBreakfast_Main2(string[] args)
        {
            var s = new Stopwatch();
            const string Title = "AsyncBreakfast 2.nd";
            Console.WriteLine(Constants.Constants.Header, Title);
            s.Start();
            Coffee cup = PourCoffee();
            Console.WriteLine($"{s.Elapsed}: coffee is ready");

            Egg eggs = await FryEggsAsync(2);
            Console.WriteLine($"{s.Elapsed}: eggs are ready");

            Bacon bacon = await FryBaconAsync(3);
            Console.WriteLine($"{s.Elapsed}: bacon is ready");

            Toast toast = await ToastBreadAsync(2);
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine($"{s.Elapsed}: toast is ready");

            Juice oj = PourOJ();
            Console.WriteLine($"{s.Elapsed}: oj is ready");
            s.Stop();
            Console.WriteLine($"{s.Elapsed}: Breakfast is ready!");
        }

        /// <summary>
        /// Asynchronouses the breakfast main3.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static async Task AsyncBreakfast_Main3(string[] args)
        {
            var s = new Stopwatch();
            const string Title = "AsyncBreakfast 3.rd";
            Console.WriteLine(Constants.Constants.Header, Title);
            s.Start();
            Coffee cup = PourCoffee();
            Console.WriteLine($"{s.Elapsed}: coffee is ready");

            var eggsTask = FryEggsAsync(2);
            var baconTask = FryBaconAsync(3);
            var toastTask = MakeToastWithButterAndJamAsync(2);

            var eggs = await eggsTask;
            Console.WriteLine($"{s.Elapsed}: eggs are ready");

            var bacon = await baconTask;
            Console.WriteLine($"{s.Elapsed}: bacon is ready");

            var toast = await toastTask;
            Console.WriteLine($"{s.Elapsed}: toast is ready");

            Juice oj = PourOJ();
            Console.WriteLine($"{s.Elapsed}: oj is ready");
            s.Stop();
            Console.WriteLine($"{s.Elapsed}: Breakfast is ready!");
        }

        /// <summary>
        /// Asynchronouses the breakfast main4.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static async Task AsyncBreakfast_Main4(string[] args)
        {
            var s = new Stopwatch();
            const string Title = "AsyncBreakfast 4.th";
            Console.WriteLine(Constants.Constants.Header, Title);
            s.Start();

            Coffee cup = PourCoffee();
            Console.WriteLine($"{s.Elapsed}: coffee is ready");

            var eggsTask = FryEggsAsync(2);
            var baconTask = FryBaconAsync(3);
            var toastTask = MakeToastWithButterAndJamAsync(2);

            var breakfastTasks = new List<Task> { eggsTask, baconTask, toastTask };
            while (breakfastTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(breakfastTasks);
                if (finishedTask == eggsTask)
                {
                    Console.WriteLine($"{s.Elapsed}: eggs are ready");
                }
                else if (finishedTask == baconTask)
                {
                    Console.WriteLine($"{s.Elapsed}: bacon is ready");
                }
                else if (finishedTask == toastTask)
                {
                    Console.WriteLine($"{s.Elapsed}: toast is ready");
                }
                breakfastTasks.Remove(finishedTask);
            }

            Juice oj = PourOJ();
            Console.WriteLine($"{s.Elapsed}: oj is ready");
            s.Stop();
            Console.WriteLine($"{s.Elapsed}: Breakfast is ready!");
        }

        /// <summary>
        /// Make toast with butter and jam as an asynchronous operation.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A Task&lt;Toast&gt; representing the asynchronous operation.</returns>
        private static async Task<Toast> MakeToastWithButterAndJamAsync(int number)
        {
            var toast = await ToastBreadAsync(number);
            ApplyButter(toast);
            ApplyJam(toast);

            return toast;
        }

        /// <summary>
        /// Toast bread as an asynchronous operation.
        /// </summary>
        /// <param name="slices">The slices.</param>
        /// <returns>A Task&lt;Toast&gt; representing the asynchronous operation.</returns>
        private static async Task<Toast> ToastBreadAsync(int slices)
        {
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("Putting a slice of bread in the toaster");
            }
            Console.WriteLine("Start toasting...");
            await Task.Delay(3000);
            Console.WriteLine("Remove toast from toaster");

            return new Toast();
        }

        /// <summary>
        /// Fry bacon as an asynchronous operation.
        /// </summary>
        /// <param name="slices">The slices.</param>
        /// <returns>A Task&lt;Bacon&gt; representing the asynchronous operation.</returns>
        private static async Task<Bacon> FryBaconAsync(int slices)
        {

            Console.WriteLine($"putting {slices} slices of bacon in the pan");
            Console.WriteLine("cooking first side of bacon...");
            await Task.Delay(3000);
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("flipping a slice of bacon");
            }
            Console.WriteLine("cooking the second side of bacon...");
            await Task.Delay(3000);
            Console.WriteLine("Put bacon on plate");

            return new Bacon();
        }

        /// <summary>
        /// Fry eggs as an asynchronous operation.
        /// </summary>
        /// <param name="howMany">The how many.</param>
        /// <returns>A Task&lt;Egg&gt; representing the asynchronous operation.</returns>
        private static async Task<Egg> FryEggsAsync(int howMany)
        {
            Console.WriteLine("Warming the egg pan...");
            await Task.Delay(3000);
            Console.WriteLine($"cracking {howMany} eggs");
            Console.WriteLine("cooking the eggs ...");
            await Task.Delay(3000);
            Console.WriteLine("Put eggs on plate");

            return new Egg();
        }

        /// <summary>
        /// Pours the oj.
        /// </summary>
        /// <returns>Juice.</returns>
        private static Juice PourOJ()
        {
            Console.WriteLine("Pouring orange juice");
            return new Juice();
        }

        /// <summary>
        /// Applies the jam.
        /// </summary>
        /// <param name="toast">The toast.</param>
        private static void ApplyJam(Toast toast) =>
            Console.WriteLine("Putting jam on the toast");

        /// <summary>
        /// Applies the butter.
        /// </summary>
        /// <param name="toast">The toast.</param>
        private static void ApplyButter(Toast toast) =>
            Console.WriteLine("Putting butter on the toast");

        /// <summary>
        /// Toasts the bread.
        /// </summary>
        /// <param name="slices">The slices.</param>
        /// <returns>Toast.</returns>
        private static Toast ToastBread(int slices)
        {
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("Putting a slice of bread in the toaster");
            }
            Console.WriteLine("Start toasting...");
            Task.Delay(3000).Wait();
            Console.WriteLine("Remove toast from toaster");

            return new Toast();
        }

        /// <summary>
        /// Fries the bacon.
        /// </summary>
        /// <param name="slices">The slices.</param>
        /// <returns>Bacon.</returns>
        private static Bacon FryBacon(int slices)
        {
            Console.WriteLine($"putting {slices} slices of bacon in the pan");
            Console.WriteLine("cooking first side of bacon...");
            Task.Delay(3000).Wait();
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("flipping a slice of bacon");
            }
            Console.WriteLine("cooking the second side of bacon...");
            Task.Delay(3000).Wait();
            Console.WriteLine("Put bacon on plate");

            return new Bacon();
        }

        /// <summary>
        /// Fries the eggs.
        /// </summary>
        /// <param name="howMany">The how many.</param>
        /// <returns>Egg.</returns>
        private static Egg FryEggs(int howMany)
        {
            Console.WriteLine("Warming the egg pan...");
            Task.Delay(3000).Wait();
            Console.WriteLine($"cracking {howMany} eggs");
            Console.WriteLine("cooking the eggs ...");
            Task.Delay(3000).Wait();
            Console.WriteLine("Put eggs on plate");

            return new Egg();
        }

        /// <summary>
        /// Pours the coffee.
        /// </summary>
        /// <returns>Coffee.</returns>
        private static Coffee PourCoffee()
        {
            Console.WriteLine("Pouring coffee");
            return new Coffee();
        }
    }

    /// <summary>
    /// Class Juice.
    /// </summary>
    internal class Juice
    {
    }

    /// <summary>
    /// Class Toast.
    /// </summary>
    internal class Toast
    {
    }

    /// <summary>
    /// Class Bacon.
    /// </summary>
    internal class Bacon
    {
    }

    /// <summary>
    /// Class Egg.
    /// </summary>
    internal class Egg
    {
    }

    /// <summary>
    /// Class Coffee.
    /// </summary>
    internal class Coffee
    {
    }
}
