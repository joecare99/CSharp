using MVVM_09_DialogBoxes.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_09_DialogBoxes.Views
{
    /// <summary>
    /// Interaktionslogik für DialogView.xaml
    /// </summary>
    public partial class DialogView : Page
    {
        public Func<IDialogWindow> NewDialogWindow = () => new DialogWindow();
        public Func<string, string, MessageBoxButton, MessageBoxResult> MessageBoxShow =
            (t, n, mbb) => MessageBox.Show(t, n, mbb);

        public DialogView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (DialogViewModel)DataContext;
            vm.DoOpenDialog = (Name, email) =>
            {
                IDialogWindow dialog = NewDialogWindow();
                var dialogViewModel = ((DialogWindowViewModel)dialog.DataContext);
                (dialogViewModel.Name, dialogViewModel.Email) = (Name, email);
                if (dialog.ShowDialog() == true)
                {
                    return (dialogViewModel.Name, dialogViewModel.Email);
                }
                else
                    return (Name, email);
            };
            vm.DoOpenMessageBox = (Title, Name) => MessageBoxShow(Title, Name, MessageBoxButton.YesNo);
        }
    }
}
