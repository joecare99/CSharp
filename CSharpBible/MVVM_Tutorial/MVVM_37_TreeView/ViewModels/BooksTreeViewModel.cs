// ***********************************************************************
// Assembly         : MVVM_37_TreeView
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using MVVM_37_TreeView.Models;
using MVVM_37_TreeView.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MVVM_37_TreeView.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class BooksTreeViewModel : BaseViewModelCT
    {
        #region Properties
        public static Func<BooksService> GetModel { get; set; } = () => new BooksService();

        private readonly IBooksService _service;

        [ObservableProperty]
        private ObservableCollection<CategorizedBooksViewModel> _books = new();

        [ObservableProperty]
        private Book? _selectedBook = null; 
        #endregion
  
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public BooksTreeViewModel():this(GetModel())
        {
        }

        public BooksTreeViewModel(IBooksService service)
        {
            _service = service;

            var booksToAdd = _service.GetBooks()
                .GroupBy(b => b.Category)
                .Select(b => new CategorizedBooksViewModel() {
                    Category = b.Key,
                    Books = new(b.Select(b1 => new CategorizedBooksViewModel()
                    {
                        Category = $"{b1.Title} - {b1.Author}",
                        This=b1
                    }))
                });
                        
            foreach(var book in booksToAdd)
                Books.Add(book);
        }

        [RelayCommand]
        private void DoSelectedItemChanged(object? prop)
        {  
            if (prop is RoutedPropertyChangedEventArgs<object> rpcEa && rpcEa.NewValue is CategorizedBooksViewModel cbvm)
                SelectedBook = cbvm.This;
            else
                SelectedBook = null;
        }

        #endregion
    }
}
