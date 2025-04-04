﻿using System;
using System.Windows.Input;
using BaseLib.Interfaces;
using ConsoleDisplay.View;
using Pattern_00_Template.ViewModels;

namespace Pattern_00_Template.Views
{
    public class MainView : ICommand
    {
        public MainViewModel DataContext { get; set; } = new();
        public IConsole console { get; set; } = new MyConsole();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            console.WriteLine(DataContext.Greeting);
        }
    }
}
