﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using MVVM.View.Extension;
using MdbBrowser.ViewModels.Interfaces;
using NSubstitute;
using BaseLib.Helper;

namespace MdbBrowser.Views.Tests
{
    [TestClass()]
    public class TableViewTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // Build the DependencyInjection container
            var builder = new ServiceCollection()
               .AddTransient((s)=>Substitute.For<IDBViewViewModel>());

            IoC.Configure(builder.BuildServiceProvider());
        }

        [TestMethod()]
        public void TableViewTest()
        {
            TableView? mw = null;
            var t = new Thread(() => mw = new());
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            t.Join(); //Wait for the thread to end
            Assert.IsNotNull(mw);
            Assert.IsInstanceOfType(mw, typeof(TableView));
        }
    }
}