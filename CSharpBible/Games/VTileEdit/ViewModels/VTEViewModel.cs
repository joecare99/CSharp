using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
#if linux
#else
using Microsoft.Win32;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using VTileEdit.Models;
using VTileEdit.ViewModels;

namespace VTileEdit;

public partial class VTEViewModel : ObservableObject, IVTEViewModel
{
    private IVTEModel _model;

    public Func<FileDialog, bool> ShowFileDialog { get; set; }
    public VTEViewModel(IVTEModel model)
    {
        _model = model;
    }

    [RelayCommand]
    private void LoadTileDefs()
    {
        FileDialog ofd = new OpenFileDialog();
        ofd.Filter = FileDialogFilter.Build([
            ("TileDef Files", [ "tdf","tdt","tdj","tdx" ]),
            ("All Files", ["*"]),
            ("TileDef Files", ["tdf"]) ]);

        if (ShowFileDialog?.Invoke(ofd) ?? false)
        {
            using (var fs = new FileStream(ofd.FileName, FileMode.Open))
            {
                switch (Path.GetExtension(ofd.FileName))
                {
                    case ".txt":
                        _model.LoadFromStream(fs, EStreamType.Text);
                        break;
                    case ".tdf":
                        _model.LoadFromStream(fs, EStreamType.Binary);
                        break;
                    case ".tdj":
                        _model.LoadFromStream(fs, EStreamType.Json);
                        break;
                    case ".tdx":
                        _model.LoadFromStream(fs, EStreamType.Xml);
                        break;
                    case ".cs":
                        _model.LoadFromStream(fs, EStreamType.Code);
                        break;
                };
            }
        }
    }

    [RelayCommand]
    private void SaveTileDefs()
    {
        FileDialog sfd = new SaveFileDialog();
        sfd.Filter = FileDialogFilter.Build([
            ("TileDef Files", [ "tdf","tdt","tdj","tdx" ]),
            ("All Files", ["*"]),
            ("TileDef Files", ["tdf"]) ]);
        if (ShowFileDialog?.Invoke(sfd) ?? false)
        {
            using (var fs = new FileStream(sfd.FileName, FileMode.Create))
            {
                switch (Path.GetExtension(sfd.FileName))
                {
                    case ".txt":
                        _model.SaveToStream(fs, EStreamType.Text);
                        break;
                    case ".tdf":
                        _model.SaveToStream(fs, EStreamType.Binary);
                        break;
                    case ".tdj":
                        _model.SaveToStream(fs, EStreamType.Json);
                        break;
                    case ".tdx":
                        _model.SaveToStream(fs, EStreamType.Xml);
                        break;
                    case ".cs":
                        _model.SaveToStream(fs, EStreamType.Code);
                        break;
                };
            }
        }
    }
}
public static class FileDialogFilter
{
    public static string Build(IEnumerable<(string, IEnumerable<string>)> filters)
    {
        var sb = new StringBuilder();
        foreach (var filter in filters)
        {
            sb.Append(filter.Item1);
            sb.Append("|");
            sb.Append(string.Join(";", filter.Item2.Select(e => $"*.{e}")));
            sb.Append("|");
        }
        sb.Append("|");
        return sb.ToString();
    }
}
