using CommunityToolkit.Mvvm.ComponentModel;
using MSQBrowser.Models;
using System.Collections.ObjectModel;

namespace MSQBrowser.ViewModels
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
