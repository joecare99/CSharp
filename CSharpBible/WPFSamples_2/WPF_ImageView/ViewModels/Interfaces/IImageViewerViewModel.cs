using System.Collections;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace ImageView.ViewModels.Interfaces;

public interface IImageViewerViewModel: INotifyPropertyChanged
{
    ArrayList ImageFiles { get; }

    int SelectedImage { get; set; }

    object Image { get; }

    string ImageSize { get; }
    string ImageFormat { get; }
    string FileSize { get; }
}