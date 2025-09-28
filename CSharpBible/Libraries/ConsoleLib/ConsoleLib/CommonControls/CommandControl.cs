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
        protected bool _enabled = true;
        protected ICommand? _command;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                OnEnabledChanged();
            }
        }

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
                bool can = _command.CanExecute(Tag);
                if (can != Enabled)
                    Enabled = can;
            }
        }

        protected virtual void OnEnabledChanged() => Invalidate();

        public override void Click()
        {
            if (!Enabled) return; // ignore clicks when disabled
            base.Click(); // raise OnClick event first
            if (_command != null && _command.CanExecute(Tag))
                _command.Execute(Tag);
        }
    }
}
