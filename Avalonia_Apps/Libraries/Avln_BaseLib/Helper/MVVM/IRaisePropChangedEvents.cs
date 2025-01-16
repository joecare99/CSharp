using System.ComponentModel;

namespace BaseLib.Helper.MVVM;

public interface IRaisePropChangedEvents : INotifyPropertyChanged
{
    void RaisePropertyChanged(string propertyName);
}