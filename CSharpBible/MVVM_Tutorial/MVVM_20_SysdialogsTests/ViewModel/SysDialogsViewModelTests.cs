// ***********************************************************************
// Assembly         : MVVM_20_SysdialogsTests
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-11-2022
// ***********************************************************************
// <copyright file="SysDialogsViewModelTests.cs" company="MVVM_20_SysdialogsTests">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommonDialogs.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MVVM_20_Sysdialogs.ViewModel.Tests
{
    /// <summary>
    /// Defines test class SysDialogsViewModelTests.
    /// </summary>
    [TestClass()]
	public class SysDialogsViewModelTests {
		/// <summary>
		/// The c exp file open name0
		/// </summary>
		private readonly string cExpFileOpenName0="<Open>";
		/// <summary>
		/// The c exp file save name0
		/// </summary>
		private readonly string cExpFileSaveName0 = "<Save>";
		/// <summary>
		/// The c exp path name0
		/// </summary>
		private readonly string cExpPathName0 = "<Path>";
		/// <summary>
		/// The c exp ext0
		/// </summary>
		private readonly string cExpExt0 = ".txt";
		/// <summary>
		/// The c exp my color0
		/// </summary>
		private readonly Color cExpMyColor0 = Color.White;
		/// <summary>
		/// The c exp my font0
		/// </summary>
		private readonly Font cExpMyFont0 = new Font("Microsoft Sans Serif", 8.25f);

		/// <summary>
		/// The c exp set file open name
		/// </summary>
		private readonly string cExpSetFileOpenName = "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, FileOpenName>\r\n";
		/// <summary>
		/// The c exp set file save name
		/// </summary>
		private readonly string cExpSetFileSaveName = "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, FileSaveName>\r\n";
		/// <summary>
		/// The c exp set path name
		/// </summary>
		private readonly string cExpSetPathName = "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, PathName>\r\n";
		/// <summary>
		/// The c exp set ext
		/// </summary>
		private readonly string cExpSetExt = "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, Ext>\r\n";
		/// <summary>
		/// The c exp set my color
		/// </summary>
		private readonly string cExpSetMyColor = "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyColor>\r\n";
		/// <summary>
		/// The c exp set my font
		/// </summary>
		private readonly string cExpSetMyFont = "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyFont>\r\n";

		/// <summary>
		/// The test result
		/// </summary>
		private String _testResult="";
		/// <summary>
		/// The test ret value
		/// </summary>
		private bool? _testRetValue;
        /// <summary>
        /// The test view model
        /// </summary>
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private SysDialogsViewModel _testViewModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test new value
        /// </summary>
        public object? testNewValue;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		[TestInitialize]
		public void Init() {
            _testViewModel = new SysDialogsViewModel
            {
                FileOpenDialog = TestDoFileOpenDlg,
                FileSaveAsDialog = TestDoFileSaveAsDlg,
                DirectoryBrowseDialog = TestDoDirectoryBrowseDlg,
                dColorDialog = TestDoColorDlg,
                dFontDialog = TestDoFontDlg,
                dPrintDialog = TestDoPrintDlg
            };
            _testViewModel.PropertyChanged += TestPropertyChanged;
			_testResult = "";
		}

		/// <summary>
		/// Logs the test.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="_Caller">The caller.</param>
		private void LogTest(string message, [CallerMemberName] string _Caller="") {
			_testResult += $"{_Caller}: <{message}>\r\n";
		}
		/// <summary>
		/// Tests the do print dialog.
		/// </summary>
		/// <param name="par">The par.</param>
		/// <param name="OnPrint">The on print.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoPrintDlg(IPrintDialog par, Action<IPrintDialog>? OnPrint) {
			LogTest($"{par}, {OnPrint != null}");
			if (_testRetValue ?? false) OnPrint?.Invoke(par);
			return _testRetValue;
		}

		/// <summary>
		/// Tests the do font dialog.
		/// </summary>
		/// <param name="font">The font.</param>
		/// <param name="par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoFontDlg(Font font, IFontDialog par, Action<Font, IFontDialog>? OnAccept) {
			LogTest($"{font}, {par}, {OnAccept != null}");
			if (_testRetValue ?? false) OnAccept?.Invoke(testNewValue as Font ?? SystemFonts.DefaultFont, par);
			return _testRetValue;
		}

		/// <summary>
		/// Tests the do color dialog.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoColorDlg(Color color, IColorDialog par, Action<Color, IColorDialog>? OnAccept) {
			LogTest($"{color}, {par}, {OnAccept != null}");
			if (_testRetValue ?? false) OnAccept?.Invoke(testNewValue as Color? ?? Color.Empty, par);
			return _testRetValue;
		}

		/// <summary>
		/// Tests the do directory browse dialog.
		/// </summary>
		/// <param name="Filename">The filename.</param>
		/// <param name="Par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoDirectoryBrowseDlg(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept) {
			LogTest($"{Filename}, {Par}, {OnAccept != null}");
			if (_testRetValue ?? false) OnAccept?.Invoke(testNewValue as String ?? "", Par);
			return _testRetValue;
		}

		/// <summary>
		/// Tests the do file save as dialog.
		/// </summary>
		/// <param name="Filename">The filename.</param>
		/// <param name="Par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoFileSaveAsDlg(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept) {
			LogTest($"{Filename}, {Par}, {OnAccept != null}");
			if (_testRetValue ?? false) OnAccept?.Invoke(testNewValue as String ?? "", Par);
			return _testRetValue;
		}

		/// <summary>
		/// Tests the do file open dialog.
		/// </summary>
		/// <param name="Filename">The filename.</param>
		/// <param name="Par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoFileOpenDlg(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept) {
			LogTest($"{Filename}, {Par}, {OnAccept != null}");
			if (_testRetValue ?? false) OnAccept?.Invoke(testNewValue as String??"", Par);
			return _testRetValue;
		}

		/// <summary>
		/// Tests the property changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
		private void TestPropertyChanged(object? sender, PropertyChangedEventArgs e) {
			LogTest($"{sender}, {e.PropertyName}");
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest() {
			SysDialogsViewModel viewModel = new SysDialogsViewModel();
			Assert.IsNotNull(viewModel);
			Assert.IsInstanceOfType(viewModel, typeof(SysDialogsViewModel));
			Assert.AreEqual(cExpFileOpenName0, viewModel.FileOpenName, ".FileOpenName");
			Assert.AreEqual(cExpFileSaveName0, viewModel.FileSaveName, ".FileSaveName");
			Assert.AreEqual(cExpPathName0, viewModel.PathName, ".PathName");
			Assert.AreEqual(cExpExt0, viewModel.Ext, ".Ext");
			Assert.AreEqual(cExpMyColor0, viewModel.MyColor, ".MyColor");
			Assert.AreEqual(cExpMyFont0, viewModel.MyFont, ".MyFont");
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest_SetFileOpenName.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest_SetFileOpenName() {
			Assert.AreEqual("", _testResult);
			_testViewModel.FileOpenName = "Test1";
			Assert.AreEqual(cExpSetFileOpenName, _testResult);
		}

        /// <summary>
        /// Defines the test method SysDialogsViewModelTest_SetFileSaveName.
        /// </summary>
        [TestMethod()]
		public void SysDialogsViewModelTest_SetFileSaveName() {
			Assert.AreEqual("", _testResult);
			_testViewModel.FileSaveName = "Test1";
			Assert.AreEqual(cExpSetFileSaveName, _testResult);
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest_SetPathName.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest_SetPathName() {
			Assert.AreEqual("", _testResult);
			_testViewModel.PathName = "Test1";
			Assert.AreEqual(cExpSetPathName, _testResult);
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest_SetExt.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest_SetExt() {
			Assert.AreEqual("", _testResult);
			_testViewModel.Ext = "Test1";
			Assert.AreEqual(cExpSetExt, _testResult);
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest_SetMyFont.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest_SetMyFont() {
			Assert.AreEqual("", _testResult);
			_testViewModel.MyFont = new Font("Arial", 12);
			Assert.AreEqual(cExpSetMyFont, _testResult);
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest_SetMyColor.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest_SetMyColor() {
			Assert.AreEqual("", _testResult);
			_testViewModel.MyColor = Color.Blue;
			Assert.AreEqual(cExpSetMyColor, _testResult);
		}

		/// <summary>
		/// Systems the dialogs view model test set property.
		/// </summary>
		/// <param name="PropName">Name of the property.</param>
		/// <param name="value">The value.</param>
		/// <param name="Exp">The exp.</param>
		[DataTestMethod()]
		[DataRow("FileOpenName","Test2", "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, FileOpenName>\r\n")]
		[DataRow("FileSaveName", "Test2", "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, FileSaveName>\r\n")]
		[DataRow("PathName", "Test2", "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, PathName>\r\n")]
		[DataRow("Ext", "Test2", "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, Ext>\r\n")]
		[DataRow("MyColor", null, "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyColor>\r\n")]
		[DataRow("MyFont", null, "TestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyFont>\r\n")]
		public void SysDialogsViewModelTest_SetProp(string PropName,object value, string Exp ) {
			Assert.AreEqual("", _testResult);
			_testViewModel.GetType().GetProperty(PropName)?.SetValue(_testViewModel, value, null);
			Assert.AreEqual(Exp, _testResult);
		}

		/// <summary>
		/// Systems the dialogs view model test command.
		/// </summary>
		/// <param name="Command">The command.</param>
		/// <param name="tr">if set to <c>true</c> [tr].</param>
		/// <param name="value">The value.</param>
		/// <param name="asExp">The exp.</param>
		[DataTestMethod()]
		[DataRow("OpenFileOpenDialogCommand", false, "", new[] { "TestDoFileOpenDlg: <<Open>, CommonDialogs.FileDialogProxy`1[Microsoft.Win32.OpenFileDialog], True>\r\n" })]
		[DataRow("OpenFileOpenDialogCommand", true, "Test3", new[] { "TestDoFileOpenDlg: <<Open>, CommonDialogs.FileDialogProxy`1[Microsoft.Win32.OpenFileDialog], True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, FileOpenName>\r\n" })]
		[DataRow("OpenFileOpenDialogCommand", null, "", new[] { "TestDoFileOpenDlg: <<Open>, CommonDialogs.FileDialogProxy`1[Microsoft.Win32.OpenFileDialog], True>\r\n" })]
		[DataRow("OpenFileSaveAsDialogCommand", false, "Test2", new[] { "TestDoFileSaveAsDlg: <<Save>, CommonDialogs.FileDialogProxy`1[Microsoft.Win32.SaveFileDialog], True>\r\n" })]
		[DataRow("OpenFileSaveAsDialogCommand", true, "Test2", new[] { "TestDoFileSaveAsDlg: <<Save>, CommonDialogs.FileDialogProxy`1[Microsoft.Win32.SaveFileDialog], True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, FileSaveName>\r\n" })]
		[DataRow("OpenFileSaveAsDialogCommand", null, "Test2", new[] { "TestDoFileSaveAsDlg: <<Save>, CommonDialogs.FileDialogProxy`1[Microsoft.Win32.SaveFileDialog], True>\r\n" })]
		[DataRow("OpenDirectoryBrowseDialogCommand", false ,"Test2", new[] { "TestDoDirectoryBrowseDlg: <<Path>, CommonDialogs.FolderBrowserDialog, True>\r\n" })]
		[DataRow("OpenDirectoryBrowseDialogCommand", true,"Test2", new[] { "TestDoDirectoryBrowseDlg: <<Path>, CommonDialogs.FolderBrowserDialog, True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, PathName>\r\n" })]
		[DataRow("OpenDirectoryBrowseDialogCommand", null, "Test2", new[] { "TestDoDirectoryBrowseDlg: <<Path>, CommonDialogs.FolderBrowserDialog, True>\r\n" })]
		[DataRow("OpenColorDialogCommand", false,"Test2", new[] { "TestDoColorDlg: <Color [White], System.Windows.Forms.ColorDialog,  Color: Color [White], True>\r\n" })]
		[DataRow("OpenColorDialogCommand", true,"Test2", new[] { "TestDoColorDlg: <Color [White], System.Windows.Forms.ColorDialog,  Color: Color [White], True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyColor>\r\n" })]
		[DataRow("OpenColorDialogCommand", null,"Test2", new[] { "TestDoColorDlg: <Color [White], System.Windows.Forms.ColorDialog,  Color: Color [White], True>\r\n" })]
		[DataRow("OpenFontDialogCommand", false, "Test2", new[] { "TestDoFontDlg: <[Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=1, GdiVerticalFont=False], CommonDialogs.FontDialog,  Font: [Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=1, GdiVerticalFont=False], True>\r\n" })]
		[DataRow("OpenFontDialogCommand", true, "Test2", new[] { "TestDoFontDlg: <[Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=1, GdiVerticalFont=False], CommonDialogs.FontDialog,  Font: [Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=1, GdiVerticalFont=False], True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyFont>\r\n" })]
		[DataRow("OpenFontDialogCommand", null, "Test2", new[] { "TestDoFontDlg: <[Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=1, GdiVerticalFont=False], CommonDialogs.FontDialog,  Font: [Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=1, GdiVerticalFont=False], True>\r\n" })]
		[DataRow("OpenPrintDialogCommand", false, "Test2", new[] { "TestDoPrintDlg: <CommonDialogs.PrintDialog, True>\r\n" })]
		[DataRow("OpenPrintDialogCommand", true, "Test2", new[] { "TestDoPrintDlg: <CommonDialogs.PrintDialog, True>\r\n" })]
		[DataRow("OpenPrintDialogCommand", null, "Test2", new[] { "TestDoPrintDlg: <CommonDialogs.PrintDialog, True>\r\n" })]
		public void SysDialogsViewModelTest_Command(string Command, bool? tr ,object value, string[] asExp) {
			Assert.AreEqual("", _testResult);
			DelegateCommand? d= _testViewModel.GetType()?.GetProperty(Command)?.GetValue(_testViewModel, null) as DelegateCommand;
			_testRetValue = tr;
			testNewValue = value;
			d?.Execute(Array.Empty<object>());
			Assert.AreEqual(asExp[0], _testResult);
		}
        /// <summary>
        /// Systems the dialogs view model test command.
        /// </summary>
        /// <param name="Command">The command.</param>
        /// <param name="tr">if set to <c>true</c> [tr].</param>
        /// <param name="value">The value.</param>
        /// <param name="Exp">The exp.</param>
        [DataTestMethod()]
        [DataRow("OpenFileOpenDialogCommand", false, "", "TestDoFileOpenDlg: <<Open>, Microsoft.Win32.OpenFileDialog: Title: , FileName: <Open>, True>\r\n")]
        [DataRow("OpenFileOpenDialogCommand", true, "Test3", "TestDoFileOpenDlg: <<Open>, Microsoft.Win32.OpenFileDialog: Title: , FileName: <Open>, True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, FileOpenName>\r\n")]
        [DataRow("OpenFileOpenDialogCommand", null, "", "TestDoFileOpenDlg: <<Open>, Microsoft.Win32.OpenFileDialog: Title: , FileName: <Open>, True>\r\n")]
        [DataRow("OpenFileSaveAsDialogCommand", false, "Test2", "TestDoFileSaveAsDlg: <<Save>, Microsoft.Win32.SaveFileDialog: Title: , FileName: <Save>, True>\r\n")]
        [DataRow("OpenFileSaveAsDialogCommand", true, "Test2", "TestDoFileSaveAsDlg: <<Save>, Microsoft.Win32.SaveFileDialog: Title: , FileName: <Save>, True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, FileSaveName>\r\n")]
        [DataRow("OpenFileSaveAsDialogCommand", null, "Test2", "TestDoFileSaveAsDlg: <<Save>, Microsoft.Win32.SaveFileDialog: Title: , FileName: <Save>, True>\r\n")]
        [DataRow("OpenDirectoryBrowseDialogCommand", false, "Test2", "TestDoDirectoryBrowseDlg: <<Path>, Microsoft.Win32.OpenFileDialog: Title: , FileName: <Path>, True>\r\n")]
        [DataRow("OpenDirectoryBrowseDialogCommand", true, "Test2", "TestDoDirectoryBrowseDlg: <<Path>, Microsoft.Win32.OpenFileDialog: Title: , FileName: <Path>, True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, PathName>\r\n")]
        [DataRow("OpenDirectoryBrowseDialogCommand", null, "Test2", "TestDoDirectoryBrowseDlg: <<Path>, Microsoft.Win32.OpenFileDialog: Title: , FileName: <Path>, True>\r\n")]
        [DataRow("OpenColorDialogCommand", false, "Test2", "TestDoColorDlg: <Color [White], System.Windows.Forms.ColorDialog,  Color: Color [White], True>\r\n")]
        [DataRow("OpenColorDialogCommand", true, "Test2", "TestDoColorDlg: <Color [White], System.Windows.Forms.ColorDialog,  Color: Color [White], True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyColor>\r\n")]
        [DataRow("OpenColorDialogCommand", null, "Test2", "TestDoColorDlg: <Color [White], System.Windows.Forms.ColorDialog,  Color: Color [White], True>\r\n")]
        [DataRow("OpenFontDialogCommand", false, "Test2", "TestDoFontDlg: <[Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], CommonDialogs.FontDialog,  Font: [Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], True>\r\n")]
        [DataRow("OpenFontDialogCommand", true, "Test2", "TestDoFontDlg: <[Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], CommonDialogs.FontDialog,  Font: [Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyFont>\r\n")]
        [DataRow("OpenFontDialogCommand", null, "Test2", "TestDoFontDlg: <[Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], CommonDialogs.FontDialog,  Font: [Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], True>\r\n")]
        [DataRow("OpenPrintDialogCommand", false, "Test2", "TestDoPrintDlg: <System.Windows.Controls.PrintDialog, True>\r\n")]
        [DataRow("OpenPrintDialogCommand", true, "Test2", "TestDoPrintDlg: <System.Windows.Controls.PrintDialog, True>\r\n")]
        [DataRow("OpenPrintDialogCommand", null, "Test2", "TestDoPrintDlg: <System.Windows.Controls.PrintDialog, True>\r\n")]
        public void SysDialogsViewModelTest_Command2(string Command, bool? tr, object value, string Exp)
        {
			var viewModel = new SysDialogsViewModel();
            viewModel.PropertyChanged += TestPropertyChanged;
			//viewModel.FileOpenDialog = null;
            Assert.AreEqual("", _testResult);
            DelegateCommand? d = viewModel.GetType()?.GetProperty(Command)?.GetValue(viewModel, null) as DelegateCommand;
            _testRetValue = tr;
            testNewValue = value;
            d?.Execute(Array.Empty<object>());
            Assert.AreEqual("", _testResult);
        }
    }
}
