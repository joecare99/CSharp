﻿using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.View.Extension;
using MVVM_22_WpfCap.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVVM_22_WpfCap.Tests
{
    internal class TestApp : App
    {
        public void DoStartUp()
        {
            OnStartup(null!);
        }
    }
    [TestClass()]
    public class AppTests 
    {
        static TestApp app = new();
        private Func<Type, object?> _gsold = null!;
        private Func<Type, object> _grsold = null!;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <autogeneratedoc />
        [TestInitialize]
        public void Init()
        {
            _gsold = IoC.GetSrv;
            _grsold = IoC.GetReqSrv;
        }

        [TestCleanup]
        public void CleanUp()
        {
            IoC.GetSrv = _gsold;
            IoC.GetReqSrv = _grsold;
        }

        [TestMethod]
        public void AppTest()
        {
            Assert.IsNotNull(app);
        }

        [TestMethod]
        public void AppTest2()
        {
            app.DoStartUp();
            Assert.IsNotNull(IoC.GetReqSrv(typeof(IWpfCapModel)));
            Assert.IsNull(IoC.GetSrv(typeof(App)));
        }
    }
}