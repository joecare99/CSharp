﻿namespace MVVM_09_DialogBoxes.View
{
    public interface IDialogWindow
    {
        object DataContext { get; }

        bool? ShowDialog();
    }
}
