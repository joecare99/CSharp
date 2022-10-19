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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ItemsControlTut4.ViewModel
{
    /// <summary>
    /// Class TodoItem.
    /// Implements the <see cref="INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class TodoItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the completion.
        /// </summary>
        /// <value>The completion.</value>
        public int Completion { get; set; }
        /// <summary>
        /// Gets or sets the do.
        /// </summary>
        /// <value>The do.</value>
        public DelegateCommand Do { get; set; }
        /// <summary>
        /// Gets the this.
        /// </summary>
        /// <value>The this.</value>
        public object This => this;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="name">The name.</param>
        public void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            if (!string.IsNullOrEmpty(name))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class MainWindowViewModel: BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            TodoList = new ObservableCollection<TodoItem>();
            TodoList.Add(new TodoItem() { Title = "Complete this WPF tutorial", Completion = 50,Do= new DelegateCommand(DoAction) });
            TodoList.Add(new TodoItem() { Title = "Learn C#", Completion = 90, Do = new DelegateCommand(DoAction) });
            TodoList.Add(new TodoItem() { Title = "Wash the car", Completion = 20, Do = new DelegateCommand(DoAction) });
			TodoList.Add(new TodoItem() { Title = "Drive Home", Completion = 10, Do = new DelegateCommand(DoAction) });
		}

		/// <summary>
		/// Does the action.
		/// </summary>
		/// <param name="obj">The object.</param>
		private void DoAction(object? obj)
        {
            if (obj is TodoItem todo)
            {
                todo.Completion = 100;
                todo.NotifyPropertyChanged(nameof(todo.Completion));
            }
        }

        /// <summary>
        /// Gets or sets the todo list.
        /// </summary>
        /// <value>The todo list.</value>
        public ObservableCollection<TodoItem> TodoList { get ; set; }

    }
}
