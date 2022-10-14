// ***********************************************************************
// Assembly         : ListBinding_netTests
// Author           : Mir
// Created          : 06-17-2022
//
// Last Modified By : Mir
// Last Modified On : 08-26-2022
// ***********************************************************************
// <copyright file="PersonTests.cs" company="ListBinding_netTests">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
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
        public void PersonTest2(int aId, string aFirstname, string aLastname, string aTitle, string ExpFullname)
        {
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
        [DataRow(4, "", "", "3", ", ,3")]
        [DataRow(5, "1", "2", "", "2, 1")]
        [DataRow(6, "", "2", "3", "2, , 3")]
        [DataRow(7, "1", "", "3", ", 1, 3")]
        public void PersonTest3(int aId, string ExpFirstname, string ExpLastname, string ExpTitle, string aFullname)
        {
            var person = new Person(aFullname)
            {
                Id = aId
            };
            Assert.AreEqual(ExpLastname, person.LastName, "Lastname");
            Assert.AreEqual(ExpFirstname, person.FirstName, "Firstname");
            Assert.AreEqual(ExpTitle, person.Title,"Title");
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
        /// Gets the object data test.
        /// </summary>
        /// <param name="aId">a identifier.</param>
        /// <param name="aFirstname">a firstname.</param>
        /// <param name="aLastname">a lastname.</param>
        /// <param name="aTitle">a title.</param>
        /// <param name="ExpFullname">The exp fullname.</param>
        [DataTestMethod()]
        [DataRow(0, "", "", "",   new[] { "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>0</Id><FirstName /><LastName /><Title /></Person>",
                                          "<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>0</Id><FirstName /><LastName /><Title /></Person>"  })]
        [DataRow(1, "1", "2", "3",new[] {"﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>1</Id><FirstName>1</FirstName><LastName>2</LastName><Title>3</Title></Person>",
                                         "<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>1</Id><FirstName>1</FirstName><LastName>2</LastName><Title>3</Title></Person>" })]
        [DataRow(2, "1", "", "",  new[] { "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>2</Id><FirstName>1</FirstName><LastName /><Title /></Person>" })]
        [DataRow(3, "", "2", "",  new[] { "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>3</Id><FirstName /><LastName>2</LastName><Title /></Person>" })]
        [DataRow(4, "", "", "3",  new[] { "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>4</Id><FirstName /><LastName /><Title>3</Title></Person>" })]
        [DataRow(5, "1", "2", "", new[] { "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>5</Id><FirstName>1</FirstName><LastName>2</LastName><Title /></Person>" })]
        [DataRow(6, "", "2", "3", new[] { "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>6</Id><FirstName /><LastName>2</LastName><Title>3</Title></Person>" })]
        [DataRow(7, "1", "", "3", new[] { "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>7</Id><FirstName>1</FirstName><LastName /><Title>3</Title></Person>" })]
        [DataRow(8, "Joe", "Care", "Prof. Dr.", new[] { "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><Person xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id>8</Id><FirstName>Joe</FirstName><LastName>Care</LastName><Title>Prof. Dr.</Title></Person>" })]
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
            AssertAreEqual(ExpFullname[0], actual);
        }

        private void AssertAreEqual(string v, string actual)
        {
            if (string.IsNullOrEmpty(v) && string.IsNullOrEmpty(actual)) return;
            if (v == actual) return;
            if (v.Contains(Environment.NewLine))
            {
                var vs= v.Split(Environment.NewLine);
                var acs=actual.Split(Environment.NewLine);
                Assert.AreEqual(vs.Length, acs.Length, $".Lines");
                for (int i = 0; i < vs.Length; i++)
                    Assert.AreEqual(vs[i], acs[i], $".Line{i}");    
            }
            else if (v.Contains("<"))
            {
                var vs = v.Split("<");
                var acs = actual.Split("<");
                Assert.AreEqual(vs.Length, acs.Length,".Tags");
                for (int i = 0; i < vs.Length; i++)
                    Assert.AreEqual(vs[i], acs[i],$".Tag{i}");
            }
            else if (v.Length> 15)
                for (int i = 0; i < v.Length-10; i++)
                   Assert.AreEqual(v.Substring(i,10), actual.Substring(i, 10));
            else Assert.AreEqual(v, actual);
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
        public void CompleteDeserializationTest(int aId, string aFirstname, string aLastname, string aTitle, string[] ExpFullname)
        {
            object? p=default;
            var formatter = new XmlSerializer(typeof(Person));
            using (var stream = new MemoryStream())
            {
                var b = Encoding.UTF8.GetBytes(ExpFullname[0]);
                stream.Write(b, 0, b.Length);
                stream.Seek(0L, SeekOrigin.Begin);
                p = formatter.Deserialize(stream);
            }
            Assert.IsInstanceOfType(p, typeof(Person));
            if (p is Person pp)
            {
                Assert.AreEqual(aId, pp.Id,$"{DisplayNameAttribute.Default.DisplayName}.ID");
                Assert.AreEqual(aFirstname, pp.FirstName, $"{DisplayNameAttribute.Default.DisplayName}.FirstName");
                Assert.AreEqual(aLastname, pp.LastName, $"{DisplayNameAttribute.Default.DisplayName}.LastName");
                Assert.AreEqual(aTitle, pp.Title, $"{DisplayNameAttribute.Default.DisplayName}.Title");
            }
        }
    }
}
