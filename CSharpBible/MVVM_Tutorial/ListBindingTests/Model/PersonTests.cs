// ***********************************************************************
// Assembly         : ListBindingTests
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 08-13-2022
// ***********************************************************************
// <copyright file="PersonTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ListBinding.Model.Tests
{
    /// <summary>
    /// Defines test class PersonTests.
    /// </summary>
    [TestClass()]
    public class PersonTests
    {
        /// <summary>
        /// Defines the test method PersonTest.
        /// </summary>
        [TestMethod()]
        public void PersonTest()
        {
            var person = new Person()
            {
                LastName = "2",
                FirstName = "1",
                Title = "3",
                Id = -1
            };

        }

        /// <summary>
        /// Persons the full name.
        /// </summary>
        /// <param name="aId">a identifier.</param>
        /// <param name="aFirstname">a firstname.</param>
        /// <param name="aLastname">a lastname.</param>
        /// <param name="aTitle">a title.</param>
        /// <param name="ExpFullname">The exp fullname.</param>
        [DataTestMethod()]
        [DataRow(0, "", "", "", "")]
        [DataRow(1, "1", "2", "3", "2, 1, 3")]
        [DataRow(2, "1", "", "", ", 1")]
        [DataRow(3, "", "2", "", "2, ")]
        [DataRow(4, "", "", "3", ", 3")]
        [DataRow(5, "1", "2", "", "2, 1")]
        [DataRow(6, "", "2", "3", "2, , 3")]
        [DataRow(7, "1", "", "3", ", 1, 3")]
        public void PersonFullName(int aId, string aFirstname, string aLastname, string aTitle, string ExpFullname)
        {
            var person = new Person()
            {
                Id = aId,
                FirstName = aFirstname,
                LastName = aLastname,
                Title = aTitle
            };
            Assert.AreEqual(ExpFullname, person.FullName);
        }

        /// <summary>
        /// Persons the test2.
        /// </summary>
        /// <param name="aId">a identifier.</param>
        /// <param name="aFirstname">a firstname.</param>
        /// <param name="aLastname">a lastname.</param>
        /// <param name="aTitle">a title.</param>
        /// <param name="ExpFullname">The exp fullname.</param>
        [DataTestMethod()]
        [DataRow(0, "", "", "", "")]
        [DataRow(1, "1", "2", "3", "2, 1, 3")]
        [DataRow(2, "1", "", "", ", 1")]
        [DataRow(3, "", "2", "", "2, ")]
        [DataRow(4, "", "", "3", ", 3")]
        [DataRow(5, "1", "2", "", "2, 1")]
        [DataRow(6, "", "2", "3", "2, , 3")]
        [DataRow(7, "1", "", "3", ", 1, 3")]
        public void PersonTest2(int aId, string aFirstname, string aLastname, string aTitle, string ExpFullname) {
            var person = new Person(aLastname, aFirstname, aTitle)
            {
                Id = aId
            };
            Assert.AreEqual(ExpFullname, person.FullName);
        }

        /// <summary>
        /// Persons the test3.
        /// </summary>
        /// <param name="aId">a identifier.</param>
        /// <param name="ExpFirstname">The exp firstname.</param>
        /// <param name="ExpLastname">The exp lastname.</param>
        /// <param name="ExpTitle">The exp title.</param>
        /// <param name="aFullname">a fullname.</param>
        [DataTestMethod()]
        [DataRow(0, "", "", "", "")]
        [DataRow(1, "1", "2", "3", "2, 1, 3")]
        [DataRow(2, "1", "", "", ", 1")]
        [DataRow(3, "", "2", "", "2, ")]
        [DataRow(4, "", "", "3", ", , 3")] //!! 
        [DataRow(5, "1", "2", "", "2, 1")]
        [DataRow(6, "", "2", "3", "2, , 3")]
        [DataRow(7, "1", "", "3", ", 1, 3")]
        public void PersonTest3(int aId, string ExpFirstname, string ExpLastname, string ExpTitle, string aFullname)
        {
            var person = new Person(aFullname)
            {
                Id = aId
            };
            Assert.AreEqual(ExpLastname, person.LastName);
            Assert.AreEqual(ExpFirstname, person.FirstName);
            Assert.AreEqual(ExpTitle, person.Title);
        }

        /// <summary>
        /// Gets the object data test.
        /// </summary>
        /// <param name="aId">a identifier.</param>
        /// <param name="aFirstname">a firstname.</param>
        /// <param name="aLastname">a lastname.</param>
        /// <param name="aTitle">a title.</param>
        /// <param name="ExpFullname">The exp fullname.</param>
        [DataTestMethod()]
        [DataRow(0, "", "", "", new[] { "<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Id>0</Id>\r\n  <FirstName />\r\n  <LastName />\r\n  <Title />\r\n</Person>" })]
        [DataRow(1, "1", "2", "3", new[] { "<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Id>1</Id>\r\n  <FirstName>1</FirstName>\r\n  <LastName>2</LastName>\r\n  <Title>3</Title>\r\n</Person>" })]
        [DataRow(2, "1", "", "", new[] { "<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Id>2</Id>\r\n  <FirstName>1</FirstName>\r\n  <LastName />\r\n  <Title />\r\n</Person>" })]
        [DataRow(3, "", "2", "", new[] { "<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Id>3</Id>\r\n  <FirstName />\r\n  <LastName>2</LastName>\r\n  <Title />\r\n</Person>" })]
        [DataRow(4, "", "", "3", new[] { "<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Id>4</Id>\r\n  <FirstName />\r\n  <LastName />\r\n  <Title>3</Title>\r\n</Person>" })]
        [DataRow(5, "1", "2", "", new[] { "<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Id>5</Id>\r\n  <FirstName>1</FirstName>\r\n  <LastName>2</LastName>\r\n  <Title />\r\n</Person>" })]
        [DataRow(6, "", "2", "3", new[] { "<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Id>6</Id>\r\n  <FirstName />\r\n  <LastName>2</LastName>\r\n  <Title>3</Title>\r\n</Person>" })]
        [DataRow(7, "1", "", "3", new[] { "<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Id>7</Id>\r\n  <FirstName>1</FirstName>\r\n  <LastName />\r\n  <Title>3</Title>\r\n</Person>" })]
        [DataRow(8, "Joe", "Care", "Prof. Dr.", new[] { "<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <Id>8</Id>\r\n  <FirstName>Joe</FirstName>\r\n  <LastName>Care</LastName>\r\n  <Title>Prof. Dr.</Title>\r\n</Person>" })]
        public void GetObjectDataTest(int aId, string aFirstname, string aLastname, string aTitle, string[] ExpFullname)
        { 
            var person = new Person(aLastname, aFirstname, aTitle) { Id = aId };
            var formatter = new XmlSerializer(typeof(Person));
            string actual = "";
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, person);
                stream.Seek(0L, SeekOrigin.Begin);
                var b = new byte[stream.Length];
                stream.Read(b, 0, b.Length);
                actual = Encoding.UTF8.GetString(b);
            }
            Assert.AreEqual(ExpFullname[0], actual);
        }

        /// <summary>
        /// Converts to stringtest.
        /// </summary>
        /// <param name="aId">a identifier.</param>
        /// <param name="aFirstname">a firstname.</param>
        /// <param name="aLastname">a lastname.</param>
        /// <param name="aTitle">a title.</param>
        /// <param name="ExpFullname">The exp fullname.</param>
        [DataTestMethod()]
        [DataRow(0, "", "", "", "0, ")]
        [DataRow(1, "1", "2", "3", "1, 2, 1, 3")]
        [DataRow(2, "1", "", "", "2, , 1")]
        [DataRow(3, "", "2", "", "3, 2, ")]
        [DataRow(4, "", "", "3", "4, , 3")]
        [DataRow(5, "1", "2", "", "5, 2, 1")]
        [DataRow(6, "", "2", "3", "6, 2, , 3")]
        [DataRow(7, "1", "", "3", "7, , 1, 3")]
        public void ToStringTest(int aId, string aFirstname, string aLastname, string aTitle, string ExpFullname)
        {
            var person = new Person()
            {
                Id = aId,
                FirstName = aFirstname,
                LastName = aLastname,
                Title = aTitle
            };
            Assert.AreEqual(ExpFullname, person.ToString());
        }
    }
}
