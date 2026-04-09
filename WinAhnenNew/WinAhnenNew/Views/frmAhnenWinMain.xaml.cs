using System;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.Messages;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for frmAhnenWinMain.xaml.
    /// </summary>
    public partial class Form1 : Window
    {
        private readonly IMessenger _messenger;
        private int _iLastSelectedTabIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            var appServices = ((App)Application.Current).Services;
            _messenger = appServices.GetRequiredService<IMessenger>();
            DataContext = appServices.GetRequiredService<FrmAhnenWinMainViewModel>();

            _messenger.Register<NavigateToEditTabMessage>(this, static (objRecipient, _) =>
            {
                if (objRecipient is Form1 wndMain)
                {
                    wndMain.PageControl1.SelectedIndex = 1;
                }
            });

            Closed += Form1_Closed;
        }

        private void Form1_Closed(object? sender, EventArgs e)
        {
            _messenger.UnregisterAll(this);
        }

        private void PageControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ReferenceEquals(e.Source, PageControl1))
            {
                return;
            }

            if (_iLastSelectedTabIndex == 1 && EditFrame.Content is EditPageView pgEdit)
            {
                pgEdit.CommitPendingEdits();
            }

            _iLastSelectedTabIndex = PageControl1.SelectedIndex;
        }
    }
}
