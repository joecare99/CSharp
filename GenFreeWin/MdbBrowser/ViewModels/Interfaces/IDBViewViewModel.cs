using MdbBrowser.Models;
using MdbBrowser.Models.Interfaces;
using System.ComponentModel;

namespace MdbBrowser.ViewModels.Interfaces
{
    public interface IDBViewViewModel
    {
        public DBMetaData? SelectedEntry { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public IDBModel? dBModel { get; }
    }
}