﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ItemsControlTut3.ViewModel.Tests
{
    public class BaseTest
    {
        /// <summary>
        /// The debug result
        /// </summary>
        /// <autogeneratedoc />
        protected string DebugResult = "";

        /// <summary>
        /// Handles the <see cref="E:ItemPropChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <autogeneratedoc />
        protected void OnItemPropChanged(object? sender, PropertyChangedEventArgs e)
            => DoLog($"OnPropChanged: o:{sender}, p:{e.PropertyName}:{sender?.GetType().GetProperty(e.PropertyName)?.GetValue(sender)}");

        protected void DoLog(string line)
            => DebugResult += $"{line}{Environment.NewLine}";

        protected void ClearResults()
            => DebugResult = "";

        protected void TestToDoItems(ETestAction eTest, object o, TodoItem testToDoItem)
        {
            switch (eTest)
            {
                case ETestAction.SetTitle:
                    testToDoItem.Title = o as string ?? "";
                    break;
                case ETestAction.SetCompletion:
                    testToDoItem.Completion = (int)o;
                    Assert.AreEqual((int)o, testToDoItem.Completion);
                    break;
                case ETestAction.DoStep:
                    testToDoItem.Step.Execute(testToDoItem);
                    Assert.AreEqual((int)o, testToDoItem.Completion);
                    break;
                case ETestAction.DoStep2:
                    testToDoItem.Step.Execute(testToDoItem);
                    testToDoItem.Step.Execute(testToDoItem);
                    Assert.AreEqual((int)o, testToDoItem.Completion);
                    break;
                case ETestAction.DoDo:
                    testToDoItem.Do.Execute(testToDoItem);
                    Assert.AreEqual((int)o, testToDoItem.Completion);
                    break;
            }
        }

        public enum ETestAction
        {
            SetTitle,
            SetCompletion,
            DoStep,
            DoStep2,
            DoDo,
        }
    }

    [TestClass()]
    public class TodoItemTests: BaseTest
    {
        private TodoItem TestToDoItem;
        
        private int ciStep = 10;

        [TestInitialize()]
        public void Init()
        {
            TestToDoItem = new TodoItem() { Title = "TestItem", Completion = 10, Do = new DelegateCommand(DoCommand), Step = new DelegateCommand(StepCommand) };
            TestToDoItem.PropertyChanged += OnItemPropChanged;
            ClearResults();
        }


        private void StepCommand(object? obj)
        {
            DoLog($"StepCommand: o:{obj}");
            if (obj is TodoItem todo)
                todo.Completion += ciStep;
        }

        private void DoCommand(object? obj)
        {
            DoLog($"DoCommand: o:{obj}");
            if (obj is TodoItem todo)
                todo.Completion = 100;
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(TestToDoItem);
            Assert.IsInstanceOfType(TestToDoItem,typeof(TodoItem));
            Assert.AreEqual(TestToDoItem, TestToDoItem.This);
        }

        [DataTestMethod]
        [DataRow("Title",ETestAction.SetTitle,"ABC",new string[] { "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title:ABC\r\n" })]
        [DataRow("Title1", ETestAction.SetTitle, "TestItem", new string[] { "" })]
        [DataRow("Title2", ETestAction.SetTitle, "TestItem2", new string[] { "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title:TestItem2\r\n" })]
        [DataRow("Completion", ETestAction.SetCompletion, 57, new string[] { "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:57\r\n" })]
        [DataRow("Completion", ETestAction.SetCompletion, 10, new string[] { "" })]
        [DataRow("Completion", ETestAction.SetCompletion, 20, new string[] { "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:20\r\n" })]
        [DataRow("DoStep", ETestAction.DoStep, 20, new string[] { "StepCommand: o:ItemsControlTut3.ViewModel.TodoItem\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:20\r\n" })]
        [DataRow("DoStep2", ETestAction.DoStep2, 30, new string[] { "StepCommand: o:ItemsControlTut3.ViewModel.TodoItem\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:20\r\nStepCommand: o:ItemsControlTut3.ViewModel.TodoItem\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:30\r\n" })]
        [DataRow("DoDo", ETestAction.DoDo, 100, new string[] { "DoCommand: o:ItemsControlTut3.ViewModel.TodoItem\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:100\r\n" })]
        public void TodoItemTest(string name,ETestAction eTest,object o,string[] sExp)
        {
            TestToDoItems(eTest, o, TestToDoItem);
            Assert.AreEqual(sExp[0], DebugResult);
        }

        [DataTestMethod]
        [DataRow("Title", ETestAction.SetTitle, "ABC", new string[] { "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title:ABC\r\n" })]
        [DataRow("Title1", ETestAction.SetTitle, "TestItem", new string[] { "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title:TestItem\r\n" })]
        [DataRow("Title2", ETestAction.SetTitle, "TestItem2", new string[] { "" })]
        [DataRow("Completion", ETestAction.SetCompletion, 57, new string[] { "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:57\r\n" })]
        [DataRow("Completion", ETestAction.SetCompletion, 10, new string[] { "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:10\r\n" })]
        [DataRow("Completion", ETestAction.SetCompletion, 20, new string[] { "" })]
        [DataRow("DoStep", ETestAction.DoStep, 30, new string[] { "StepCommand: o:ItemsControlTut3.ViewModel.TodoItem\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:30\r\n" })]
        [DataRow("DoStep2", ETestAction.DoStep2, 40, new string[] { "StepCommand: o:ItemsControlTut3.ViewModel.TodoItem\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:30\r\nStepCommand: o:ItemsControlTut3.ViewModel.TodoItem\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:40\r\n" })]
        [DataRow("DoDo", ETestAction.DoDo, 100, new string[] { "DoCommand: o:ItemsControlTut3.ViewModel.TodoItem\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion:100\r\n" })]
        public void TodoItemTest2(string name, ETestAction eTest, object o, string[] sExp)
        {
            var testToDoItem = new TodoItem() { Title = "TestItem2", Completion = 20, Do = new DelegateCommand(DoCommand), Step = new DelegateCommand(StepCommand) };
            testToDoItem.PropertyChanged += OnItemPropChanged;
            TestToDoItems(eTest, o, testToDoItem);
            Assert.AreEqual(sExp[0], DebugResult);
        }
    }

    [TestClass()]
    public class MainWindowViewModelTests:BaseTest
    {
        private MainWindowViewModel TestMainWindowViewModel;
        private string DebugResult="";
        private readonly string cAddTodoExp= "OnCollectionChanged: o:System.Collections.ObjectModel.ObservableCollection`1[ItemsControlTut3.ViewModel.TodoItem], p:System.Collections.Specialized.SingleItemReadOnlyList, io:-1, in:3\r\n";

        private void OnItemPropChanged(object? sender, PropertyChangedEventArgs e)
        {
            DebugResult += $"OnPropChanged: o:{sender}, p:{e.PropertyName}{Environment.NewLine}";
        }
        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            DebugResult += $"OnCollectionChanged: o:{sender}, p:{e.NewItems}, io:{e.OldStartingIndex}, in:{e.NewStartingIndex}{Environment.NewLine}";
        }

        [TestInitialize()]
        public void Init()
        {
            TestMainWindowViewModel = new MainWindowViewModel();
            TestMainWindowViewModel.PropertyChanged += OnItemPropChanged;
            TestMainWindowViewModel.TodoList.CollectionChanged += OnCollectionChanged;
            foreach (var item in TestMainWindowViewModel.TodoList)
                item.PropertyChanged += OnItemPropChanged;
            DebugResult = "";
        }


        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(TestMainWindowViewModel);
            Assert.IsInstanceOfType(TestMainWindowViewModel, typeof(MainWindowViewModel));
            Assert.IsNotNull(TestMainWindowViewModel.TodoList);
            Assert.IsInstanceOfType(TestMainWindowViewModel.TodoList,typeof(ObservableCollection<TodoItem>));
            foreach(var item in TestMainWindowViewModel.TodoList)
               Assert.IsInstanceOfType(item, typeof(TodoItem));          
        }

        [DataTestMethod]
        [DataRow("Title-0", 0, ETestAction.SetTitle, "ABC", new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title\r\n"})]
        [DataRow("Title-1", 1, ETestAction.SetTitle, "ABC", new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title\r\n"})]
        [DataRow("Title-2", 2, ETestAction.SetTitle, "ABC", new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title\r\n"})]
        [DataRow("Title1-0",0, ETestAction.SetTitle, "Wash the car", new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title\r\n"})]
        [DataRow("Title1-1",1, ETestAction.SetTitle, "Wash the car", new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title\r\n"})]
        [DataRow("Title1-2",2, ETestAction.SetTitle, "Wash the car", new string[]{ ""})]
        [DataRow("Title2-0",0, ETestAction.SetTitle, "Learn C#", new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title\r\n"})]
        [DataRow("Title2-1",1, ETestAction.SetTitle, "Learn C#", new string[]{ ""})]
        [DataRow("Title2-2",2, ETestAction.SetTitle, "Learn C#", new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Title\r\n"})]
        [DataRow("Completion-0-57", 0, ETestAction.SetCompletion, 57, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("Completion-0-10", 0, ETestAction.SetCompletion, 10, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("Completion-0-50", 0, ETestAction.SetCompletion, 50, new string[]{ ""})]
        [DataRow("Completion-0-90", 0, ETestAction.SetCompletion, 90, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("Completion-1-57", 1, ETestAction.SetCompletion, 57, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("Completion-1-10", 1, ETestAction.SetCompletion, 10, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("Completion-1-50", 1, ETestAction.SetCompletion, 50, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("Completion-1-90", 1, ETestAction.SetCompletion, 90, new string[]{ ""})]
        [DataRow("Completion-2-57", 2, ETestAction.SetCompletion, 57, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("Completion-2-10", 2, ETestAction.SetCompletion, 10, new string[]{ ""})]
        [DataRow("Completion-2-50", 2, ETestAction.SetCompletion, 50, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("Completion-2-90", 2, ETestAction.SetCompletion, 90, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("DoStep" , 0, ETestAction.DoStep, 60, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("DoStep2", 0, ETestAction.DoStep2, 70, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("DoDo"   , 0, ETestAction.DoDo, 100, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("DoStep" , 1, ETestAction.DoStep, 100, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("DoStep2", 1, ETestAction.DoStep2, 100, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("DoDo"   , 1, ETestAction.DoDo, 100, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("DoStep" , 2, ETestAction.DoStep, 20, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("DoStep2", 2, ETestAction.DoStep2, 30, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\nOnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        [DataRow("DoDo"   , 2, ETestAction.DoDo, 100, new string[]{ "OnPropChanged: o:ItemsControlTut3.ViewModel.TodoItem, p:Completion\r\n"})]
        public void TodoItemTest(string name,int iEl, ETestAction eTest, object o, string[] sExp)
        {
            TestToDoItems(eTest, o, TestMainWindowViewModel.TodoList[iEl]);
            Assert.AreEqual(sExp[0], DebugResult);
        }

        [TestMethod()]
        public void MainWindowViewModelTest()
        {
            TestMainWindowViewModel.AddTodo("Drive Home", 20);
            Assert.AreEqual(cAddTodoExp, DebugResult);
        }
    }
}