using CommunityToolkit.Mvvm.ComponentModel;
using MdbBrowser.Models;
using System.Collections.ObjectModel;

namespace MdbBrowser.ViewModels
{
    public partial class CategorizedDBMetadata : ObservableObject
    {
        [ObservableProperty]
        private string _category = "";

        [ObservableProperty]
        private DBMetaData? _this = null;

        [ObservableProperty]
        private ObservableCollection<CategorizedDBMetadata> _entries = new();
    }
}
