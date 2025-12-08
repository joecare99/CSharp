using System.Collections;
using System.ComponentModel;

namespace ImageView.Models.Interfaces;

public interface IImageViewerModel: INotifyPropertyChanged
{
    ArrayList ImageFiles { get; }
}