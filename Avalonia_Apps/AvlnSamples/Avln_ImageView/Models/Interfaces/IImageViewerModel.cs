using System.Collections;
using System.ComponentModel;

namespace Avln_ImageView.Models.Interfaces;

public interface IImageViewerModel : INotifyPropertyChanged
{
 ArrayList ImageFiles { get; }
}
