using Microsoft.VisualStudio.TestTools.UnitTesting;
using Galaxia.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NSubstitute;
using BaseLib.Models.Interfaces;

namespace Galaxia.Helper.Tests
{
    [TestClass()]
    public class NamingHelperTests
    {
        private IRandom rnd;
        private int _rnd = 0;

        [TestInitialize]
        public void Initialize()
        {
            _rnd = 0;
            NamingHelper.SetRandom(rnd = Substitute.For<IRandom>());
            rnd.Next(Arg.Any<int>()).Returns(x => _rnd++ %x.ArgAt<int>(0));
            NamingHelper.existingNames.Clear();
        }

        [TestMethod()]
        [DataRow("A", new string[] { "Anix", "Anarers", "Alurals", "Arols", "Arefis" })]
        [DataRow("B", new string[] { "Betix", "Betarers", "Balurals", "Birols" })]
        [DataRow("C", new string[] { "Canix", "Canarers", "Celurals", "Cirols" })]
        [DataRow("D", new string[] { "Denix", "Denarers", "Darurals", "Dolols" })]
        [DataRow("E", new string[] { "Egix", "Etarers", "Eturals", "Etols" })]
        [DataRow("F", new string[] { "Fasix", "Fotarers", "Foturals", "Fotols" })]
        [DataRow("", new string[] { "Anix", "Betarers", "Celurals", "Dolols", "Erefis", "Firanens", "Gonus", "Halirorx" })]
        public void GetStarSysNameTest(string name, string[] expected)
        {
       
            for (int i = 0; i < expected.Length; i++)
            {
                var result = NamingHelper.GetStarSysName(name);
                Assert.AreEqual(expected[i],result, $"Expected ({string.Join(", ", expected)})[{i}] but got {result}");
            }
        }
    }
}