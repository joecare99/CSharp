// ***********************************************************************
// Assembly         : ItemsControlTut3_net
// Author           : Mir
// Created          : 08-14-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="ItemsControlTut3_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ItemsControlTut3.ViewModel
{
    /// <summary>
    /// Class TodoItem.
    /// Extends the <see cref="NotificationObject" />
    /// </summary>
    /// <seealso cref="NotificationObject" />
    public class TodoItem : NotificationObject
    {
        #region Properties
        #region private properties
        /// <summary>
        /// Storage for completion
        /// </summary>
        private int _completion = 0;
        /// <summary>
        /// Storage for title
        /// </summary>
        private string _Title = "";
        #endregion
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title of the task.</value>
        public string Title { get => _Title; set => SetProperty(ref _Title, value); }
        /// <summary>
        /// Gets or sets the completion [%].
        /// </summary>
        /// <value>The completion of the task [%].</value>
        public int Completion { get => _completion; set => SetProperty(ref _completion, value); }
        /// <summary>
        /// Gets or sets the do-Command.
        /// </summary>
        /// <value>The do.</value>
        public DelegateCommand Do { get; set; }

        /// <summary>
        /// Gets or sets the Step-Command.
        /// </summary>
        /// <value>The do.</value>
        public DelegateCommand Step { get; set; }

        /// <summary>
        /// Gets the this.
        /// </summary>
        /// <value>The this.</value>
        public object This => this;
        #endregion

    }

    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class MainWindowViewModel: BaseViewModel
    {
        #region Properties
        #region private properties
        private string _newItem="";
        #endregion

        /// <summary>
        /// Gets or sets the todo list.
        /// </summary>
        /// <value>The todo list.</value>
        public ObservableCollection<TodoItem> TodoList { get ; set; }
        /// <summary>
        /// Gets or sets the add item command.
        /// </summary>
        /// <value>The add item command.</value>
        public DelegateCommand AddCommand { get; set; }
        /// <summary>Title of the new item.</summary>
        /// <value>The title of the new item.</value>
        public string NewItem { get=>_newItem; set=>SetProperty(ref _newItem,value); }
        #endregion
        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            TodoList = new ObservableCollection<TodoItem>();
            TodoList.Add(new TodoItem() { Title = "Complete this WPF tutorial", Completion = 50,Do= new DelegateCommand(DoAction), Step = new DelegateCommand(StepAction) });
            TodoList.Add(new TodoItem() { Title = "Learn C#", Completion = 90, Do = new DelegateCommand(DoAction), Step = new DelegateCommand(StepAction) });
            TodoList.Add(new TodoItem() { Title = "Wash the car", Completion = 10, Do = new DelegateCommand(DoAction), Step = new DelegateCommand(StepAction) });

            AddCommand = new DelegateCommand((o)=> { AddTodo(NewItem, 0); NewItem = ""; },(o)=>!String.IsNullOrEmpty(NewItem));
            AddPropertyDependency(nameof(AddCommand),nameof(NewItem));
        }

        /// <summary>
        /// Executes the "Step" action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void StepAction(object? obj)
        {
            if (obj is TodoItem todo)
            {
                if (todo.Completion < 90)
                    todo.Completion += 10;
                else
                    todo.Completion = 100;
            }
        }

        /// <summary>
        /// Executes the "Do" action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void DoAction(object? obj)
        {
            if (obj is TodoItem todo)
            {
                todo.Completion = 100;
            }
        }

        /// <summary>Adds an item to the the todo-List.</summary>
        /// <param name="sTitle">The title.</param>
        /// <param name="iCompl">The completion-percentage.</param>
        public void AddTodo(string sTitle, int iCompl)
        {
            TodoList.Add(new TodoItem() { Title = sTitle, Completion = iCompl, Do = new DelegateCommand(DoAction), Step = new DelegateCommand(StepAction) });
        }
        #endregion
    }
}
