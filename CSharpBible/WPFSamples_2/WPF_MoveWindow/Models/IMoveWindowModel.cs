using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WPF_MoveWindow.Models
{
    public interface IMoveWindowModel: INotifyPropertyChanged
    {
        System.Windows.Point TargetLocation { get; set; }
        public bool EnableKoorInput { get; }

        IRelayCommand MoveBtnCommand { get; }

        ObservableCollection<string> FeedBackList { get; }
    }
}