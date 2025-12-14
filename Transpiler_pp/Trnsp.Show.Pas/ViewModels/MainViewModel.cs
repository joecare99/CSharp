using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Trnsp.Show.Pas.Services;

namespace Trnsp.Show.Pas.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IFileService _fileService;
        private readonly IPascalParserService _parserService;

        public ObservableCollection<CodeBlockNode> RootNodes { get; } = new();

        public IRelayCommand LoadFileCommand { get; }

        public MainViewModel(IFileService fileService, IPascalParserService parserService)
        {
            _fileService = fileService;
            _parserService = parserService;
            LoadFileCommand = new RelayCommand(LoadFile);
        }

        public MainViewModel() : this(new FileService(), new PascalParserService())
        {
        }

        private void LoadFile()
        {
            var filePath = _fileService.OpenFileDialog("Open Pascal File", "Pascal Files (*.pas;*.pp)|*.pas;*.pp|All Files (*.*)|*.*");
            if (filePath != null)
            {
                var content = _fileService.ReadAllText(filePath);
                var rootBlock = _parserService.Parse(content);
                RootNodes.Clear();
                RootNodes.Add(new CodeBlockNode(rootBlock));
            }
        }
    }
}
