using Microsoft.VisualStudio.TestTools.UnitTesting;
using MdbBrowser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.ViewModel;
using MdbBrowser.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using CommonDialogs.Interfaces;
using NSubstitute;
using System.Windows;
using MdbBrowser.Models;

namespace MdbBrowser.ViewModels.Tests
{
    [TestClass()]
    public class DBViewViewModelTests : BaseTestViewModel
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        DBViewViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        private bool xDoFileResult = false;
        private string sFrResult = string.Empty;

        [TestInitialize]
        public void Init()
        {
            testModel = new DBViewViewModel();
            testModel.PropertyChanged += OnVMPropertyChanged;
            testModel.PropertyChanging += OnVMPropertyChanging;
            testModel.FileOpenDialog = OnDoFileDlg;
            testModel.FileSaveAsDialog = OnDoFileDlg;
        }

        private bool? OnDoFileDlg(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept)
        {
            DoLog($"OnDoFileDlg: {Filename},{Par} => {xDoFileResult},{sFrResult}");
            if (xDoFileResult)
            {
                Par.FileName = sFrResult;
                OnAccept?.Invoke(sFrResult, Par);
            }
            return xDoFileResult;
        }

        [TestMethod()]
        public void DBViewViewModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(DBViewViewModel));
            Assert.IsInstanceOfType(testModel, typeof(IDBViewViewModel));
        }

        [DataTestMethod()]
        [DataRow(false, null, new[] { @"" })]
        [DataRow(false, "", new[] { @"OnDoFileDlg: ,CommonDialogs.FileDialogProxy`1[Microsoft.Win32.OpenFileDialog] => False,
" })]
        [DataRow(true, "", new[] { @"OnDoFileDlg: ,CommonDialogs.FileDialogProxy`1[Microsoft.Win32.OpenFileDialog] => True,
PropChgn(MdbBrowser.ViewModels.DBViewViewModel,FileOpenName)=
PropChg(MdbBrowser.ViewModels.DBViewViewModel,FileOpenName)=
" })]
        [DataRow(true, "Resources\\mydb.mdb", new[] { @"OnDoFileDlg: ,CommonDialogs.FileDialogProxy`1[Microsoft.Win32.OpenFileDialog] => True,Resources\mydb.mdb
PropChgn(MdbBrowser.ViewModels.DBViewViewModel,FileOpenName)=
PropChg(MdbBrowser.ViewModels.DBViewViewModel,FileOpenName)=Resources\mydb.mdb
" })]
        public void OpenCommandTest(bool xAct,string sAct,string[] asExp)
        {
            if (sAct == null )
                testModel.FileOpenDialog = null;
            Assert.IsNotNull(testModel.OpenCommand);
            Assert.IsInstanceOfType(testModel.OpenCommand, typeof(IRelayCommand));
            Assert.IsTrue(testModel.OpenCommand.CanExecute(null));
            xDoFileResult = xAct;
            sFrResult = sAct;
            testModel.OpenCommand.Execute(null);
            if (xAct)
            {
                Assert.IsNotNull(testModel.dBModel);
            }
            Assert.AreEqual(asExp[0], DebugLog);
        }

        [TestMethod]
        public void CloseCommandTest()
        {
            Assert.IsNotNull(testModel.CloseCommand);
            Assert.IsInstanceOfType(testModel.CloseCommand, typeof(IRelayCommand));
            Assert.IsTrue(testModel.CloseCommand.CanExecute(null));
            testModel.CloseCommand.Execute(null);
            Assert.AreEqual("", DebugLog);
        }

        [TestMethod]
        public void ExitCommandTest()
        {
            Assert.IsNotNull(testModel.ExitCommand);
            Assert.IsInstanceOfType(testModel.ExitCommand, typeof(IRelayCommand));
            Assert.IsTrue(testModel.ExitCommand.CanExecute(null));
            testModel.ExitCommand.Execute(null);
            Assert.AreEqual("", DebugLog);
        }

        [DataTestMethod()]
        [DataRow(null, new[] { "" })]
        [DataRow(EKind.Schema, new[] { @"PropChgn(MdbBrowser.ViewModels.DBViewViewModel,SelectedEntry)=
PropChg(MdbBrowser.ViewModels.DBViewViewModel,SelectedEntry)=(N:, K:Schema D:())
PropChgn(MdbBrowser.ViewModels.DBViewViewModel,CurrentView)=
PropChg(MdbBrowser.ViewModels.DBViewViewModel,CurrentView)=/Views/SchemaView.xaml
" })]
        [DataRow(EKind.Column, new[] { @"PropChgn(MdbBrowser.ViewModels.DBViewViewModel,SelectedEntry)=
PropChg(MdbBrowser.ViewModels.DBViewViewModel,SelectedEntry)=(N:, K:Column D:())
PropChgn(MdbBrowser.ViewModels.DBViewViewModel,CurrentView)=
PropChg(MdbBrowser.ViewModels.DBViewViewModel,CurrentView)=
" })]
        [DataRow(EKind.Index, new[] { @"" })]
        public void DoSelectedItemChangedTest(EKind? kind, string[] asExp)
        {
            Assert.IsNotNull(testModel.DoSelectedItemChangedCommand);
            Assert.IsInstanceOfType(testModel.DoSelectedItemChangedCommand, typeof(IRelayCommand));
            Assert.IsTrue(testModel.DoSelectedItemChangedCommand.CanExecute(null));

            var prop = (kind != null) ? new RoutedPropertyChangedEventArgs<object>(null,
               new CategorizedDBMetadata() { This = (kind.Value != EKind.Index) ? new Models.DBMetaData() { Kind = kind.Value } : null }) : null;
            testModel.DoSelectedItemChangedCommand.Execute(prop);
            if (kind == EKind.Schema)
            {
                Assert.AreEqual(testModel, DBViewViewModel.This);
            }
            Assert.AreEqual(asExp[0], DebugLog);

        }
    }
}