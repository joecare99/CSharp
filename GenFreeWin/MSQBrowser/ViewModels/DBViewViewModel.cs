using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSQBrowser.Models;
using MSQBrowser.Models.Interfaces;
using MSQBrowser.ViewModels.Interfaces;
using CommonDialogs.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using CommonDialogs;
using Microsoft.Win32;
using System.Security;
using MSQBrowser.Properties;

namespace MSQBrowser.ViewModels
{

    public partial class DBViewViewModel : ObservableValidator, IDBViewViewModel
    {
        #region Delegates
        /// <summary>
        /// Delegate FileDialogHandler
        /// </summary>
        /// <param name="Filename">The filename.</param>
        /// <param name="Par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public delegate bool? FileDialogHandler(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept = null);

        public delegate bool? DBConnDialogHandler(string[] Params,SecureString Pwd, Action<string[], SecureString>? OnAccept = null);
        #endregion

        #region Properties
        private DBModel? _DBModel;
        [ObservableProperty]
        private string _FileOpenName;
        [ObservableProperty]
        private string _CurrentView;
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

        public DBConnDialogHandler? DBConnectDialog { get; set; }

        public static IDBViewViewModel? This { get; private set; }

        public IDBModel? dBModel => _DBModel;

        #endregion


        public DBViewViewModel()
        {
        }

        [RelayCommand]
        private void Connect()
        {
            var par = new string[] { Settings.Default.ServerName, Settings.Default.User, Settings.Default.Db };
            DBConnectDialog?.Invoke(par,Settings.Default.Password, (s,p) =>
            {
                Settings.Default.ServerName = s[0];
                Settings.Default.User = s[1];
                Settings.Default.Db = s[2];
                Settings.Default.Password = p;
                Settings.Default.Save();
                _DBModel = new DBModel(s[0], s[1], p, s[2]);
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
        private void Open()
        {
            // Show the dialog and get result.
            IFileDialog foPar = new FileDialogProxy<OpenFileDialog>(new()
            {
                FileName = FileOpenName,
                Filter = "Access Database (*.mdb)|*.mdb|All files (*.*)|*.*",
                Title = "Open Access Database",
                CheckFileExists = false
            });
            FileOpenDialog?.Invoke(FileOpenName, foPar,
                (s, p) =>
                {
                    FileOpenName = s;
                    _DBModel = new DBModel("","",null,"");
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
            if (prop is RoutedPropertyChangedEventArgs<object> rpcEa 
                && rpcEa.NewValue is CategorizedDBMetadata cbvm)
            {
                SelectedEntry = cbvm.This;
                if (SelectedEntry is DBMetaData dbmd)
                    try
                    {
                        This = this;
                        if (Assembly.GetExecutingAssembly().GetType($"{Assembly.GetExecutingAssembly().GetName().Name}.Views.{dbmd.Kind}View",true,true) is Type)
                        CurrentView = $"/Views/{dbmd.Kind}View.xaml";
                    }
                    catch { CurrentView = ""; }
            }
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
            Application.Current.Shutdown();
        }
    }
}
