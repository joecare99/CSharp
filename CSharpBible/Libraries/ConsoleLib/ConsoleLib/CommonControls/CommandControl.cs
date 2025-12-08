// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : AI Assistant
// Created          : 09-26-2025
// ***********************************************************************
// Base class for controls that execute an ICommand when clicked
using System;
using System.Windows.Input;

namespace ConsoleLib.CommonControls
{
    public class CommandControl : Control
    {
        private ICommand? _command;

        public virtual ICommand? Command
        {
            get => _command;
            set
            {
                if (_command == value) return;
                if (_command != null)
                    _command.CanExecuteChanged -= Command_CanExecuteChanged;
                _command = value;
                if (_command != null)
                    _command.CanExecuteChanged += Command_CanExecuteChanged;
                UpdateCanExecute();
            }
        }

        protected virtual void Command_CanExecuteChanged(object? sender, EventArgs e) => UpdateCanExecute();

        protected void UpdateCanExecute()
        {
            if (_command == null)
            {
                Enabled = true; // no command -> free clickable
            }
            else
            {
                Enabled = _command.CanExecute(Tag);
            }
        }

        public override void Click()
        {
            if (!Enabled) return; // ignore clicks when disabled
            if (_command != null && _command.CanExecute(Tag))
                _command.Execute(Tag);
            base.Click(); // raise OnClick event first
        }
    }
}
