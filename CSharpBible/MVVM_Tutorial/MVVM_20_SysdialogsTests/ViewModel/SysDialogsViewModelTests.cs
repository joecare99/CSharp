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
using CommonDialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using MVVM.ViewModel;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

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
		private readonly Font cExpMyFont0 = SystemFonts.DefaultFont;

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
		private String TestResult="";
		/// <summary>
		/// The test ret value
		/// </summary>
		private bool? TestRetValue;
		/// <summary>
		/// The test view model
		/// </summary>
		private SysDialogsViewModel testViewModel;
		/// <summary>
		/// The test new value
		/// </summary>
		public object TestNewValue;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		[TestInitialize]
		public void Init() {
			testViewModel = new SysDialogsViewModel();
			testViewModel.PropertyChanged += TestPropertyChanged;
			testViewModel.FileOpenDialog = TestDoFileOpenDlg;
			testViewModel.FileSaveAsDialog = TestDoFileSaveAsDlg;
			testViewModel.DirectoryBrowseDialog = TestDoDirectoryBrowseDlg;
			testViewModel.dColorDialog = TestDoColorDlg;
			testViewModel.dFontDialog = TestDoFontDlg;
			testViewModel.dPrintDialog = TestDoPrintDlg;
			TestResult = "";
		}

		/// <summary>
		/// Logs the test.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="_Caller">The caller.</param>
		private void LogTest(string message, [CallerMemberName] string _Caller=null) {
			TestResult += $"{_Caller}: <{message}>\r\n";
		}
		/// <summary>
		/// Tests the do print dialog.
		/// </summary>
		/// <param name="par">The par.</param>
		/// <param name="OnPrint">The on print.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoPrintDlg(ref PrintDialog par, Action<PrintDialog>? OnPrint) {
			LogTest($"{par}, {OnPrint != null}");
			if (TestRetValue ?? false) OnPrint?.Invoke(par);
			return TestRetValue;
		}

		/// <summary>
		/// Tests the do font dialog.
		/// </summary>
		/// <param name="font">The font.</param>
		/// <param name="par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoFontDlg(Font font, ref FontDialog par, Action<Font, FontDialog>? OnAccept) {
			LogTest($"{font}, {par}, {OnAccept != null}");
			if (TestRetValue ?? false) OnAccept?.Invoke(TestNewValue as Font, par);
			return TestRetValue;
		}

		/// <summary>
		/// Tests the do color dialog.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoColorDlg(Color color, ref ColorDialog par, Action<Color, ColorDialog>? OnAccept) {
			LogTest($"{color}, {par}, {OnAccept != null}");
			if (TestRetValue ?? false) OnAccept?.Invoke(TestNewValue as Color? ?? Color.Empty, par);
			return TestRetValue;
		}

		/// <summary>
		/// Tests the do directory browse dialog.
		/// </summary>
		/// <param name="Filename">The filename.</param>
		/// <param name="Par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoDirectoryBrowseDlg(string Filename, ref FileDialog Par, Action<string, FileDialog>? OnAccept) {
			LogTest($"{Filename}, {Par}, {OnAccept != null}");
			if (TestRetValue ?? false) OnAccept?.Invoke(TestNewValue as String, Par);
			return TestRetValue;
		}

		/// <summary>
		/// Tests the do file save as dialog.
		/// </summary>
		/// <param name="Filename">The filename.</param>
		/// <param name="Par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoFileSaveAsDlg(string Filename, ref FileDialog Par, Action<string, FileDialog>? OnAccept) {
			LogTest($"{Filename}, {Par}, {OnAccept != null}");
			if (TestRetValue ?? false) OnAccept?.Invoke(TestNewValue as String, Par);
			return TestRetValue;
		}

		/// <summary>
		/// Tests the do file open dialog.
		/// </summary>
		/// <param name="Filename">The filename.</param>
		/// <param name="Par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool? TestDoFileOpenDlg(string Filename, ref FileDialog Par, Action<string, FileDialog>? OnAccept) {
			LogTest($"{Filename}, {Par}, {OnAccept != null}");
			if (TestRetValue ?? false) OnAccept?.Invoke(TestNewValue as String, Par);
			return TestRetValue;
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
			Assert.AreEqual("", TestResult);
			testViewModel.FileOpenName = "Test1";
			Assert.AreEqual(cExpSetFileOpenName, TestResult);
		}

        /// <summary>
        /// Defines the test method SysDialogsViewModelTest_SetFileSaveName.
        /// </summary>
        [TestMethod()]
		public void SysDialogsViewModelTest_SetFileSaveName() {
			Assert.AreEqual("", TestResult);
			testViewModel.FileSaveName = "Test1";
			Assert.AreEqual(cExpSetFileSaveName, TestResult);
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest_SetPathName.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest_SetPathName() {
			Assert.AreEqual("", TestResult);
			testViewModel.PathName = "Test1";
			Assert.AreEqual(cExpSetPathName, TestResult);
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest_SetExt.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest_SetExt() {
			Assert.AreEqual("", TestResult);
			testViewModel.Ext = "Test1";
			Assert.AreEqual(cExpSetExt, TestResult);
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest_SetMyFont.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest_SetMyFont() {
			Assert.AreEqual("", TestResult);
			testViewModel.MyFont = new Font("Arial", 12);
			Assert.AreEqual(cExpSetMyFont, TestResult);
		}

		/// <summary>
		/// Defines the test method SysDialogsViewModelTest_SetMyColor.
		/// </summary>
		[TestMethod()]
		public void SysDialogsViewModelTest_SetMyColor() {
			Assert.AreEqual("", TestResult);
			testViewModel.MyColor = Color.Blue;
			Assert.AreEqual(cExpSetMyColor, TestResult);
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
			Assert.AreEqual("", TestResult);
			testViewModel.GetType().GetProperty(PropName)?.SetValue(testViewModel, value, null);
			Assert.AreEqual(Exp, TestResult);
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
		[DataRow("OpenDirectoryBrowseDialogCommand", false ,"Test2", "TestDoDirectoryBrowseDlg: <<Path>, Microsoft.Win32.OpenFileDialog: Title: , FileName: <Path>, True>\r\n")]
		[DataRow("OpenDirectoryBrowseDialogCommand", true,"Test2", "TestDoDirectoryBrowseDlg: <<Path>, Microsoft.Win32.OpenFileDialog: Title: , FileName: <Path>, True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, PathName>\r\n")]
		[DataRow("OpenDirectoryBrowseDialogCommand", null, "Test2", "TestDoDirectoryBrowseDlg: <<Path>, Microsoft.Win32.OpenFileDialog: Title: , FileName: <Path>, True>\r\n")]
		[DataRow("OpenColorDialogCommand", false,"Test2", "TestDoColorDlg: <Color [White], System.Windows.Forms.ColorDialog,  Color: Color [White], True>\r\n")]
		[DataRow("OpenColorDialogCommand", true,"Test2", "TestDoColorDlg: <Color [White], System.Windows.Forms.ColorDialog,  Color: Color [White], True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyColor>\r\n")]
		[DataRow("OpenColorDialogCommand", null,"Test2", "TestDoColorDlg: <Color [White], System.Windows.Forms.ColorDialog,  Color: Color [White], True>\r\n")]
		[DataRow("OpenFontDialogCommand", false, "Test2", "TestDoFontDlg: <[Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], CommonDialogs.FontDialog,  Font: [Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], True>\r\n")]
		[DataRow("OpenFontDialogCommand", true, "Test2", "TestDoFontDlg: <[Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], CommonDialogs.FontDialog,  Font: [Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], True>\r\nTestPropertyChanged: <MVVM_20_Sysdialogs.ViewModel.SysDialogsViewModel, MyFont>\r\n")]
		[DataRow("OpenFontDialogCommand", null, "Test2", "TestDoFontDlg: <[Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], CommonDialogs.FontDialog,  Font: [Font: Name=Microsoft Sans Serif, Size=8,25, Units=3, GdiCharSet=0, GdiVerticalFont=False], True>\r\n")]
		[DataRow("OpenPrintDialogCommand", false, "Test2", "TestDoPrintDlg: <System.Windows.Controls.PrintDialog, True>\r\n")]
		[DataRow("OpenPrintDialogCommand", true, "Test2", "TestDoPrintDlg: <System.Windows.Controls.PrintDialog, True>\r\n")]
		[DataRow("OpenPrintDialogCommand", null, "Test2", "TestDoPrintDlg: <System.Windows.Controls.PrintDialog, True>\r\n")]
		public void SysDialogsViewModelTest_Command(string Command, bool? tr ,object value, string Exp) {
			Assert.AreEqual("", TestResult);
			DelegateCommand d= testViewModel.GetType().GetProperty(Command).GetValue(testViewModel, null) as DelegateCommand;
			TestRetValue = tr;
			TestNewValue = value;
			d.Execute(new object[] { });
			Assert.AreEqual(Exp, TestResult);
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
            Assert.AreEqual("", TestResult);
            DelegateCommand d = viewModel.GetType().GetProperty(Command).GetValue(viewModel, null) as DelegateCommand;
            TestRetValue = tr;
            TestNewValue = value;
            d.Execute(new object[] { });
            Assert.AreEqual("", TestResult);
        }
    }
}
