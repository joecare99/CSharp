using Avalonia.Controls;
using AvlnAhnenNew.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace AvlnAhnenNew.Controls
{
    public partial class EditPageView : UserControl, ICommitPendingEdits
    {
        public EditPageView()
        {
            InitializeComponent();
            DataContext = ((App)Avalonia.Application.Current!).Services.GetRequiredService<EditPageViewModel>();
        }

        public void CommitPendingEdits()
        {
            if (DataContext is EditPageViewModel vmEdit)
            {
                vmEdit.PersistSelectedPersonChanges();
            }
        }
    }
}
