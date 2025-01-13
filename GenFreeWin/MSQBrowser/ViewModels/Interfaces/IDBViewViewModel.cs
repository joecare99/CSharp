using MSQBrowser.Models;
using MSQBrowser.Models.Interfaces;
using System.ComponentModel;

namespace MSQBrowser.ViewModels.Interfaces
{
    public interface IDBViewViewModel
    {
        public DBMetaData? SelectedEntry { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public IDBModel? dBModel { get; }
    }
}
