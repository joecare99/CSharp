using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System.Collections.Generic;
using System;
using MVVM.View.Extension;
using MVVM_06_Converters_4.Model;
using NSubstitute;

namespace MVVM_06_Converters_4.ViewModels.Tests
{
    [TestClass()]
    public class PlotFrameViewModelTests: BaseTestViewModel<PlotFrameViewModel>
    {
        private Func<Type, object> _grsOld;
        private IAGVModel? _model;

        [TestInitialize]
        public override void Init()
        {
            _grsOld = IoC.GetReqSrv;
            IoC.GetReqSrv = (t) => t switch
            {
                Type _t when _t == typeof(IAGVModel) => _model ??= Substitute.For<IAGVModel>(),
                _ => throw new System.NotImplementedException($"No Service for {t}")
            };
            base.Init();
        }

        [TestMethod()]
        public void PlotFrameViewModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsNotNull(_model);
        }

        protected override Dictionary<string, object?> GetDefaultData() 
            => base.GetDefaultData();
    }
}