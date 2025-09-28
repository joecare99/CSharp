// ***********************************************************************
// Assembly         : ConsoleMouseApp
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : AI Assistant
// Last Modified On : 09-27-2025
// ***********************************************************************
using BaseLib.Interfaces;
using ConsoleLib;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleMouseApp.ViewModels;
using System.ComponentModel; // added

namespace ConsoleMouseApp.Views
{
    /// <summary>
    /// View hosting console controls. MVVM refactored.
    /// </summary>
    public class ConsoleMouseView : ConsoleLib.CommonControls.Application
    {
        #region Fields
        private readonly IConsoleMouseViewModel _vm;
        private Button _btnOne;
        private Label _lblMousePos;
        private Button _btnOK;
        private Button _btnCancel;
        private MenuBar _menuBar;
        private Panel _panel3;
        private ScrollBar _vScroll;
        private ScrollBar _hScroll;
        private Label _lblScrollValues;
        private ListBox _lstNames;
        private Label _lblSelectedName;
        #endregion

        #region Ctor
        public ConsoleMouseView(IConsole console, IExtendedConsole extendedConsole, IConsoleMouseViewModel vm) : base(console, extendedConsole)
        {
            _vm = vm;
            _vm.RequestClose += Vm_RequestClose;
            _vm.PropertyChanged += Vm_PropertyChanged;

            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);

            Visible = false;
            Border = ConsoleFramework.singleBorder;
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.DarkGray;
            BoarderColor = ConsoleColor.Green;
            Dimension = cl;

            CreateMenu();
            CreateContent();
            _panel3.Visible = _vm.Panel3_Visible;
            Visible = true;
            OnMouseMove += App_MouseMove;
        }

        private void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IConsoleMouseViewModel.Panel3_Visible))
                _panel3.Visible = _vm.Panel3_Visible;
            if (e.PropertyName == nameof(IConsoleMouseViewModel.HorizontalScroll) || e.PropertyName == nameof(IConsoleMouseViewModel.VerticalScroll))
            {
                _lblScrollValues.Text = $"H:{_vm.HorizontalScroll} V:{_vm.VerticalScroll}";
                _lblScrollValues.Invalidate();
            }

        }
        #endregion

        #region Menu
        private void CreateMenu()
        {
            _menuBar = new MenuBar
            {
                Parent = this,
                ForeColor = ConsoleColor.Black,
                BackColor = ConsoleColor.Gray,
                Position = new Point(1,1),
                size = new Size(Dimension.Width-2,1)
            };

            // File Menu
            var miFile = new MenuItem { Text = "&File" };
            var filePopup = new MenuPopup();
            filePopup.AddItem(new MenuItem { Text = "&Open", Command = _vm.OpenCommand });
            filePopup.AddItem(new MenuItem { Text = "&Save", Command = _vm.SaveCommand });
            filePopup.AddItem(new MenuItem { IsSeparator = true, Text = "---" });
            filePopup.AddItem(new MenuItem { Text = "E&xit", Command = _vm.ExitCommand });
            _menuBar.AddRootItem(miFile, filePopup);

            // Help Menu
            var miHelp = new MenuItem { Text = "&Help" };
            var helpPopup = new MenuPopup();
            helpPopup.AddItem(new MenuItem { Text = "&About", Command = _vm.AboutCommand });
            _menuBar.AddRootItem(miHelp, helpPopup);
        }
        #endregion

        #region Private UI creation
        private void CreateContent()
        {
            _btnOne = new Button
            {
                Parent = this,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.Gray,
                Shadow = true,
                Position = new Point(5, 10),
                Text = "░░1░░",
                Command = _vm.OneCommand
            };

            var panel2 = CreatePanel(new Rectangle(3, 15, 30, 10));
            _panel3 = CreatePanel(new Rectangle(35, 17, 20, 5));

            _lblMousePos = new Label
            {
                Parent = this,
                ForeColor = ConsoleColor.Gray,
                ParentBackground = true,
                Position = new Point(40, 2),
                Text = "",
                size = new Size(15, 1)
            };
            // Bind label text to view model MousePosition
            _lblMousePos.Binding = (_vm, nameof(IConsoleMouseViewModel.MousePosition));

            // Add OK / Cancel buttons with commands from view model
            _btnOK = new Button
            {
                Parent = panel2,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.Gray,
                Shadow = true,
                Position = new Point(2, 2),
                Text = "░░░OK░░░",
                Command = _vm.OkCommand
            };

            _btnCancel = new Button
            {
                Parent = panel2,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.Gray,
                Shadow = true,
                Position = new Point(14, 2),
                Text = "░Cancel░",
                Command = _vm.CancelCommand
            };

            // Scrollbars
            _vScroll = new ScrollBar
            {
                Parent = this,
                Vertical = true,
                Position = new Point(Dimension.Right - 2, 2),
                size = new Size(1, Dimension.Height - 4),
                Maximum = 100,
                LargeChange = 10
            };
            _vScroll.BindValue(_vm, nameof(IConsoleMouseViewModel.VerticalScroll));

            _hScroll = new ScrollBar
            {
                Parent = this,
                Vertical = false,
                Position = new Point(2, Dimension.Bottom - 2),
                size = new Size(Dimension.Width - 4, 1),
                Maximum = 100,
                LargeChange = 10
            };
            _hScroll.BindValue(_vm, nameof(IConsoleMouseViewModel.HorizontalScroll));

            _lblScrollValues = new Label
            {
                Parent = this,
                ForeColor = ConsoleColor.Yellow,
                ParentBackground = true,
                Position = new Point(5, Dimension.Bottom - 3),
                size = new Size(20,1),
                Text = "H:0 V:0"
            };

            // ListBox for names (bind to VM collection)
            _lstNames = new ListBox
            {
                Parent = this,
                Position = new Point(50,4),
                size = new Size(18,10),
                ForeColor = ConsoleColor.Gray,
                BackColor = ConsoleColor.Black
            };
            _lstNames.ItemsSource = _vm.Names;
            _lstNames.BindSelected(_vm, nameof(IConsoleMouseViewModel.SelectedName));

            _lblSelectedName = new Label
            {
                Parent = this,
                Position = new Point(50,15),
                size = new Size(20,1),
                ForeColor = ConsoleColor.Cyan,
                ParentBackground = true,
                Text = ""
            };
            _lblSelectedName.Binding = (_vm, nameof(IConsoleMouseViewModel.SelectedName));

        }

        private Panel CreatePanel(Rectangle cl)
        {
            return new Panel
            {
                Parent = this,
                Border = ConsoleFramework.doubleBoarder,
                ForeColor = ConsoleColor.Blue,
                BackColor = ConsoleColor.DarkBlue,
                BoarderColor = ConsoleColor.Green,
                Dimension = cl,
                Shadow = true
            };
        }
        #endregion

        #region Event handlers
        private void App_MouseMove(object sender, IMouseEvent e)
        {
            _vm.MousePosition = e.MousePos.ToString();
        }

        private void Vm_RequestClose(object? sender, EventArgs e)
        {
            Stop();
        }
        #endregion
    }
}
