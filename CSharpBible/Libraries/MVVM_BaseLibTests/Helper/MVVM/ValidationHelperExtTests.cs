using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;

namespace BaseLib.Helper.MVVM.Tests
{
    [TestClass()]
    public class ValidationHelperExtTests : INotifyDataErrorInfo
    {
        private bool _xHasErrors;
        private string _sPropName;
        private List<string> _ErrList;
        private string? csExpEmpty;

        public bool HasErrors => _xHasErrors;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName == _sPropName)                
                return _ErrList;
        else
                return new List<string>();    
        }

        [DataTestMethod()]
        [DataRow("Test",false,new string[] { },null)]
        [DataRow("TTest2", true, new[] { "Error1", "Error2" }, "Error1, Error2")]
        [DataRow("TTest3", true, new[] { "Error1", "Error2", "Error3" }, "Error1, Error2, Error3")]

        public void ValidationTextTest(string sProp,bool xHasErrors, string[] Errs,string? sExp)
        {
            _xHasErrors = xHasErrors;
            _sPropName = sProp.TrimStart('T');
            _ErrList = Errs.ToList();
            Assert.AreEqual(sExp,this.ValidationText(sProp));
            Assert.AreEqual(csExpEmpty, this.ValidationText("Test"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidationTextTestNull()
        {
            ((INotifyDataErrorInfo)null).ValidationText("123");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidationTextTestNull2()
        {
            this.ValidationText(null);
        }
    }
}