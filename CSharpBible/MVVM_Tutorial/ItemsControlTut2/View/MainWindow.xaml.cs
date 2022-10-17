// ***********************************************************************
// Assembly         : ItemsControlTut2
// Author           : Mir
// Created          : 06-16-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="ItemsControlTut2">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Windows;

namespace ItemsControlTut2.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow()
        {
            InitializeComponent();

			List<TodoItem> items = new List<TodoItem>();
			items.Add(new TodoItem() { Title = "Complete this WPF tutorial", Completion = 45 });
			items.Add(new TodoItem() { Title = "Learn C#", Completion = 80 });
			items.Add(new TodoItem() { Title = "Wash the car", Completion = 0 });

			icTodoList.ItemsSource = items;

		}
	}

	/// <summary>
	/// Class TodoItem.
	/// </summary>
	public class TodoItem {
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
	}
}
