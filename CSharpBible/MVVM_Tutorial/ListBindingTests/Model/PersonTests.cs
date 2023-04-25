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
        private Person? TestPerson1,TestPerson2;
        private string DebugResult = "";

        [TestInitialize]
        public void Init()
        {
            TestPerson1 = new Person()
            {
                LastName = "2",
                FirstName = "1",
                Title = "3",
                Id = -1
            };
            TestPerson1.PropertyChanged += OnPropertyChanged;
            TestPerson2 = new Person("Mustermann", "Max", "Dr.");
            TestPerson2.PropertyChanged += OnPropertyChanged;
            ClearResults();
        }

        private void ClearResults()
        {
            DebugResult = "";
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DebugResult += $"PropChg: {sender}, P:{e.PropertyName}, V:{sender?.GetType().GetProperty(e.PropertyName)?.GetValue(sender)}{Environment.NewLine}";
        }

        /// <summary>
        /// Defines the test method SetupTest.
        /// </summary>
        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(TestPerson1);
            Assert.IsNotNull(TestPerson1);
            Assert.IsInstanceOfType(TestPerson1,typeof(Person));
            Assert.IsInstanceOfType(TestPerson1,typeof(Person));
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
            Assert.IsNotNull(person);

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

        /// <summary>
        /// test.
        /// </summary>
        /// <param name="aId">a identifier.</param>
        /// <param name="aFirstname">a firstname.</param>
        /// <param name="aLastname">a lastname.</param>
        /// <param name="aTitle">a title.</param>
        /// <param name="ExpFullname">The exp fullname.</param>
        [DataTestMethod()]
        [DataRow(null, null, null, null, new string[] { "", "" })]
        [DataRow(-1, null, null, null,new string[] { "", "PropChg: -1, Mustermann, Max, Dr., P:Id, V:-1\r\n" })]
        [DataRow(0, null, null, null, new string[] { "PropChg: 0, 2, 1, 3, P:Id, V:0\r\n", "" })]
        [DataRow(1, "1", "2", "3", new string[] { "PropChg: 1, 2, 1, 3, P:Id, V:1\r\n", "PropChg: 1, Mustermann, Max, Dr., P:Id, V:1\r\nPropChg: 1, Mustermann, 1, Dr., P:FirstName, V:1\r\nPropChg: 1, Mustermann, 1, Dr., P:FullName, V:Mustermann, 1, Dr.\r\nPropChg: 1, 2, 1, Dr., P:LastName, V:2\r\nPropChg: 1, 2, 1, Dr., P:FullName, V:2, 1, Dr.\r\nPropChg: 1, 2, 1, 3, P:Title, V:3\r\nPropChg: 1, 2, 1, 3, P:FullName, V:2, 1, 3\r\n" })]
        [DataRow(11, "Max", "Mustermann",  "Dr.", new string[] { "PropChg: 11, 2, 1, 3, P:Id, V:11\r\nPropChg: 11, 2, Max, 3, P:FirstName, V:Max\r\nPropChg: 11, 2, Max, 3, P:FullName, V:2, Max, 3\r\nPropChg: 11, Mustermann, Max, 3, P:LastName, V:Mustermann\r\nPropChg: 11, Mustermann, Max, 3, P:FullName, V:Mustermann, Max, 3\r\nPropChg: 11, Mustermann, Max, Dr., P:Title, V:Dr.\r\nPropChg: 11, Mustermann, Max, Dr., P:FullName, V:Mustermann, Max, Dr.\r\n", "PropChg: 11, Mustermann, Max, Dr., P:Id, V:11\r\n" })]
        [DataRow(2, "1", null, null, new string[] { "PropChg: 2, 2, 1, 3, P:Id, V:2\r\n", "PropChg: 2, Mustermann, Max, Dr., P:Id, V:2\r\nPropChg: 2, Mustermann, 1, Dr., P:FirstName, V:1\r\nPropChg: 2, Mustermann, 1, Dr., P:FullName, V:Mustermann, 1, Dr.\r\n" })]
        [DataRow(12, "Max", null, null, new string[] { "PropChg: 12, 2, 1, 3, P:Id, V:12\r\nPropChg: 12, 2, Max, 3, P:FirstName, V:Max\r\nPropChg: 12, 2, Max, 3, P:FullName, V:2, Max, 3\r\n", "PropChg: 12, Mustermann, Max, Dr., P:Id, V:12\r\n" })]
        [DataRow(3, null, "2", null, new string[] { "PropChg: 3, 2, 1, 3, P:Id, V:3\r\n", "PropChg: 3, Mustermann, Max, Dr., P:Id, V:3\r\nPropChg: 3, 2, Max, Dr., P:LastName, V:2\r\nPropChg: 3, 2, Max, Dr., P:FullName, V:2, Max, Dr.\r\n" })]
        [DataRow(13, null, "Mustermann", null, new string[] { "PropChg: 13, 2, 1, 3, P:Id, V:13\r\nPropChg: 13, Mustermann, 1, 3, P:LastName, V:Mustermann\r\nPropChg: 13, Mustermann, 1, 3, P:FullName, V:Mustermann, 1, 3\r\n", "PropChg: 13, Mustermann, Max, Dr., P:Id, V:13\r\n" })]
        [DataRow(4, null, null, "3", new string[] { "PropChg: 4, 2, 1, 3, P:Id, V:4\r\n", "PropChg: 4, Mustermann, Max, Dr., P:Id, V:4\r\nPropChg: 4, Mustermann, Max, 3, P:Title, V:3\r\nPropChg: 4, Mustermann, Max, 3, P:FullName, V:Mustermann, Max, 3\r\n" })]
        [DataRow(14, null, null, "Dr.", new string[] { "PropChg: 14, 2, 1, 3, P:Id, V:14\r\nPropChg: 14, 2, 1, Dr., P:Title, V:Dr.\r\nPropChg: 14, 2, 1, Dr., P:FullName, V:2, 1, Dr.\r\n", "PropChg: 14, Mustermann, Max, Dr., P:Id, V:14\r\n" })]
        [DataRow(5, "1", "2", null, new string[] { "PropChg: 5, 2, 1, 3, P:Id, V:5\r\n", "PropChg: 5, Mustermann, Max, Dr., P:Id, V:5\r\nPropChg: 5, Mustermann, 1, Dr., P:FirstName, V:1\r\nPropChg: 5, Mustermann, 1, Dr., P:FullName, V:Mustermann, 1, Dr.\r\nPropChg: 5, 2, 1, Dr., P:LastName, V:2\r\nPropChg: 5, 2, 1, Dr., P:FullName, V:2, 1, Dr.\r\n" })]
        [DataRow(15, "Max", "Mustermann",  null, new string[] { "PropChg: 15, 2, 1, 3, P:Id, V:15\r\nPropChg: 15, 2, Max, 3, P:FirstName, V:Max\r\nPropChg: 15, 2, Max, 3, P:FullName, V:2, Max, 3\r\nPropChg: 15, Mustermann, Max, 3, P:LastName, V:Mustermann\r\nPropChg: 15, Mustermann, Max, 3, P:FullName, V:Mustermann, Max, 3\r\n", "PropChg: 15, Mustermann, Max, Dr., P:Id, V:15\r\n" })]
        [DataRow(6, null, "2", "3", new string[] { "PropChg: 6, 2, 1, 3, P:Id, V:6\r\n", "PropChg: 6, Mustermann, Max, Dr., P:Id, V:6\r\nPropChg: 6, 2, Max, Dr., P:LastName, V:2\r\nPropChg: 6, 2, Max, Dr., P:FullName, V:2, Max, Dr.\r\nPropChg: 6, 2, Max, 3, P:Title, V:3\r\nPropChg: 6, 2, Max, 3, P:FullName, V:2, Max, 3\r\n" })]
        [DataRow(16, null, "Mustermann", "Dr.", new string[] { "PropChg: 16, 2, 1, 3, P:Id, V:16\r\nPropChg: 16, Mustermann, 1, 3, P:LastName, V:Mustermann\r\nPropChg: 16, Mustermann, 1, 3, P:FullName, V:Mustermann, 1, 3\r\nPropChg: 16, Mustermann, 1, Dr., P:Title, V:Dr.\r\nPropChg: 16, Mustermann, 1, Dr., P:FullName, V:Mustermann, 1, Dr.\r\n", "PropChg: 16, Mustermann, Max, Dr., P:Id, V:16\r\n" })]
        [DataRow(7, "1", null, "3", new string[] { "PropChg: 7, 2, 1, 3, P:Id, V:7\r\n", "PropChg: 7, Mustermann, Max, Dr., P:Id, V:7\r\nPropChg: 7, Mustermann, 1, Dr., P:FirstName, V:1\r\nPropChg: 7, Mustermann, 1, Dr., P:FullName, V:Mustermann, 1, Dr.\r\nPropChg: 7, Mustermann, 1, 3, P:Title, V:3\r\nPropChg: 7, Mustermann, 1, 3, P:FullName, V:Mustermann, 1, 3\r\n" })]
        [DataRow(17, "Max", null, "Dr.", new string[] { "PropChg: 17, 2, 1, 3, P:Id, V:17\r\nPropChg: 17, 2, Max, 3, P:FirstName, V:Max\r\nPropChg: 17, 2, Max, 3, P:FullName, V:2, Max, 3\r\nPropChg: 17, 2, Max, Dr., P:Title, V:Dr.\r\nPropChg: 17, 2, Max, Dr., P:FullName, V:2, Max, Dr.\r\n", "PropChg: 17, Mustermann, Max, Dr., P:Id, V:17\r\n" })]
        public void PropertyChangeTest(int? aId, string aFirstname, string aLastname, string aTitle, string[] ExpRes)
        {
            SetData(TestPerson1);
            Assert.AreEqual(ExpRes[0], DebugResult,$"{nameof(TestPerson1)}");
            ClearResults();
            SetData(TestPerson2);
            Assert.AreEqual(ExpRes[1], DebugResult, $"{nameof(TestPerson2)}");

            void SetData(Person? p)
            {
                if (p == null) return;
                if (aId != null) p.Id = aId ?? 0; // Soll die ID änderbar sein ? 
                if (aFirstname != null) p.FirstName = aFirstname;
                if (aLastname != null) p.LastName = aLastname;
                if (aTitle != null) p.Title = aTitle;
            }
        }

    }
}
