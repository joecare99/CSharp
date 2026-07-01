using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.ViewModels;

namespace AvlnAhnenNew.Controls
{
    public partial class RelationshipsPageView : UserControl
    {
        public RelationshipsPageView()
        {
            InitializeComponent();
            DataContext = ((App)Avalonia.Application.Current!).Services.GetRequiredService<RelationshipsPageViewModel>();
        }
    }
}
