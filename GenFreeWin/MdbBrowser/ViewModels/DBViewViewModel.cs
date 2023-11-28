using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MdbBrowser.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MdbBrowser.ViewModels
{

    public partial class DBViewViewModel : ObservableValidator
    {
        #region Delegates
        /// <summary>
        /// Delegate FileDialogHandler
        /// </summary>
        /// <param name="Filename">The filename.</param>
        /// <param name="Par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public delegate bool? FileDialogHandler(string Filename, ref FileDialog Par, Action<string, FileDialog>? OnAccept = null);
        #endregion

        #region Properties
        private DBModel _DBModel;
        [ObservableProperty]
        private string _FileOpenName;
        [ObservableProperty]
        private ObservableCollection<CategorizedDBMetadata> _dbMetaInfo = new();

        [ObservableProperty]
        private DBMetaData? _selectedEntry = null;

        /// <summary>
        /// Gets or sets the file open dialog.
        /// </summary>
        /// <value>The file open dialog.</value>
        public FileDialogHandler? FileOpenDialog { get; set; }
        /// <summary>
        /// Gets or sets the file save as dialog.
        /// </summary>
        /// <value>The file save as dialog.</value>
        public FileDialogHandler? FileSaveAsDialog { get; set; }

        #endregion


        public DBViewViewModel()
        {
        }


        [RelayCommand]
        private void Open()
        {
            // Show the dialog and get result.
            FileDialog foPar = new OpenFileDialog
            {
                FileName = FileOpenName,
                Filter = "Access Database (*.mdb)|*.mdb|All files (*.*)|*.*",
                Title = "Open Access Database",
                CheckFileExists = false,
            };
            FileOpenDialog?.Invoke(FileOpenName, ref foPar,
                (s, p) =>
                {
                    FileOpenName = s;
                    _DBModel = new DBModel(FileOpenName);
                    DbMetaInfo.Clear();

                    var entriesToAdd = _DBModel.dbMetaData
                        .GroupBy(b => b.Kind)
                        .Select(b => new CategorizedDBMetadata()
                        {
                            Category = $"{b.Key}",
                            Entries = new(b.Select(b1 => new CategorizedDBMetadata()
                            {
                                Category = $"{b1.Name}",
                                This = b1
                            }))
                        });

                    foreach (var entry in entriesToAdd)
                        DbMetaInfo.Add(entry);

                });

        }

        [RelayCommand]
        private void DoSelectedItemChanged(object? prop)
        {
            if (prop is RoutedPropertyChangedEventArgs<object> rpcEa && rpcEa.NewValue is CategorizedDBMetadata cbvm)
                SelectedEntry = cbvm.This;
            else
                SelectedEntry = null;
        }

        [RelayCommand]
        private void Close()
        {
        }

        [RelayCommand]
        private void Exit()
        {
        }
    }
}
