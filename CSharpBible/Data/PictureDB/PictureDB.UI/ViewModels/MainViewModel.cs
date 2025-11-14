using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PictureDB.Base.Models;
using PictureDB.Base.Models.Interfaces;
using PictureDB.Base.Services.Interfaces;
using CommonDialogs;
using System.Windows.Threading;
using System;
using CommonDialogs.Interfaces;

namespace PictureDB.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IImageLoader _loader;
    private readonly IImageProcessor _processor;
    private readonly ILLMClient _llm;
    private readonly ICategorizer _categorizer;
    private readonly IEvaluator _evaluator;
    private readonly ISorter _sorter;
    private readonly IResultStore _store;

    public ObservableCollection<ImageResult> Results { get; } = new();

    [ObservableProperty]
    private string _folderPath = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    public Func<IFileDialog, bool?>? onShowDialog;

    public MainViewModel(IImageLoader loader, IImageProcessor processor, ILLMClient llm, ICategorizer categorizer, IEvaluator evaluator, ISorter sorter, IResultStore store)
    {
        _loader = loader;
        _processor = processor;
        _llm = llm;
        _categorizer = categorizer;
        _evaluator = evaluator;
        _sorter = sorter;
        _store = store;
    }

    // Pseudocode:
    // - Create CommonDialogs FolderBrowserDialog instance
    // - Configure description, root folder, and optionally preselect current FolderPath
    // - Show dialog (returns bool?)
    // - If user accepted (true), update FolderPath with SelectedPath
    // - Otherwise, do nothing

    [RelayCommand]
    private void Browse()
    {
        var dlg = new FolderBrowserDialog
        {
            Description = "Ordner mit Bildern wählen",
            RootFolder = Environment.SpecialFolder.MyComputer,
            ShowNewFolderButton = true,
            SelectedPath = string.IsNullOrWhiteSpace(FolderPath) ? string.Empty : FolderPath
        };

        if (onShowDialog?.Invoke(dlg) == true)
        {
            FolderPath = dlg.SelectedPath;
        }
    }

    [RelayCommand]
    private async Task AnalyzeAsync(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
        {
            Status = "Please select a folder.";
            return;
        }

        Status = "Processing...";
        Results.Clear();

        await Task.Run(async () =>
        {
            foreach (var f in _loader.LoadImages(folder))
            {
                try
                {
                    var b64 = _processor.ConvertToBase64(f);
                    var resp = await _llm.AnalyzeImageAsync(b64, "Kategorisiere und bewerte dieses Bild.");
                    var r = new ImageResult { FilePath = f, Category = _categorizer.ExtractCategory(resp), Score = _evaluator.ExtractScore(resp) };
                    App.Current.Dispatcher.Invoke(() => Results.Add(r));
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() => Status = "Error: " + ex.Message);
                }
            }

            try
            {
                var outPath = System.IO.Path.Combine(folder, "picturedb-results.json");
                _store.SaveResults(Results, outPath);
                App.Current.Dispatcher.Invoke(() => Status = "Done. Results saved to: " + outPath);
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Invoke(() => Status = "Save failed: " + ex.Message);
            }
        });
    }
}
